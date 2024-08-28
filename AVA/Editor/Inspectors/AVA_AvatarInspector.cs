#if UNITY_EDITOR

using System;
using System.Linq;
using nna.ava.components;
using UnityEditor;
using UnityEngine;

namespace nna.ava.tools
{
	[CustomEditor(typeof(AVA_Avatar))]
	public class AVA_AvatarInspector : Editor
	{
		private bool editPosition = false;

		public override void OnInspectorGUI()
		{
			var c = (AVA_Avatar)target;

			EditorGUI.BeginChangeCheck();

			var unityAvatar = c.gameObject.GetComponent<Animator>()?.avatar;
			if(unityAvatar == null || !unityAvatar.isHuman)
			{
				EditorGUILayout.LabelField("Avatar is not humanoid");
			}
			else
			{
				var eyeLeft = GetBoneFromUnityAvatar(HumanBodyBones.LeftEye.ToString(), unityAvatar, c.gameObject);
				var eyeRight = GetBoneFromUnityAvatar(HumanBodyBones.RightEye.ToString(), unityAvatar, c.gameObject);
				GUILayout.Space(10f);
				if(GUILayout.Button(eyeLeft != null && eyeRight != null ? "Set viewport between the eyes" : "Set viewport to head", GUILayout.ExpandWidth(false)))
				{
					SetupViewport(unityAvatar, c.gameObject);
				}
				
				GUILayout.Space(10f);
				if(!editPosition && GUILayout.Button("Edit viewport", GUILayout.ExpandWidth(false))) editPosition = true;
				else if(editPosition && GUILayout.Button("Stop editing viewport", GUILayout.ExpandWidth(false))) editPosition = false;
			}

			GUILayout.Space(20f);
			EditorGUILayout.PropertyField(serializedObject.FindProperty("ViewportParent"));
			EditorGUILayout.PropertyField(serializedObject.FindProperty("ViewportOffset"));

			if(EditorGUI.EndChangeCheck())
			{
				EditorUtility.SetDirty(c);
			}
		}

		public void OnSceneGUI()
		{
			var c = (AVA_Avatar)target;
			
			if(c.ViewportParent && editPosition)
			{
				Handles.Label(c.ViewportParent.transform.position + c.ViewportOffset, "Viewport");
				c.ViewportOffset = Handles.DoPositionHandle(c.ViewportParent.transform.position + c.ViewportOffset, Quaternion.identity) - c.ViewportParent.transform.position;
			}
		}
		
		[DrawGizmo(GizmoType.Selected)]
		public static void OnDrawGizmo(AVA_Avatar target, GizmoType gizmoType)
		{
			if(target && target.ViewportParent)
			{
				Gizmos.DrawSphere(target.ViewportParent.transform.position + target.ViewportOffset, 0.01f);
			}
		}

		private GameObject GetBoneFromUnityAvatar(string HumanoidName, Avatar UnityAvatar, GameObject Root)
		{
			var hb = UnityAvatar.humanDescription.human.FirstOrDefault(hb => hb.humanName == HumanoidName);
			if(hb.boneName != null)
			{
				foreach(var t in Root.GetComponentsInChildren<Transform>())
				{
					if(t.name == hb.boneName) return t.gameObject;
				}
			}
			return null;
		}

		private void SetupViewport(Avatar UnityAvatar, GameObject Root)
		{
			var c = (AVA_Avatar)target;
			c.ViewportParent = GetBoneFromUnityAvatar("Head", UnityAvatar, Root);
			var eyeLeft = GetBoneFromUnityAvatar(HumanBodyBones.LeftEye.ToString(), UnityAvatar, Root);
			var eyeRight = GetBoneFromUnityAvatar(HumanBodyBones.RightEye.ToString(), UnityAvatar, Root);
			if(eyeLeft && eyeRight)
			{
				c.ViewportOffset = ((eyeLeft.transform.position + eyeRight.transform.position) / 2) - c.ViewportParent.transform.position;
				c.ViewportOffset.x = Math.Abs(c.ViewportOffset.x) < 0.0001 ? 0 : c.ViewportOffset.x;
				c.ViewportOffset.y = Math.Abs(c.ViewportOffset.y) < 0.0001 ? 0 : c.ViewportOffset.y;
				c.ViewportOffset.z = Math.Abs(c.ViewportOffset.z) < 0.0001 ? 0 : c.ViewportOffset.z;
			}
			else
			{
				c.ViewportOffset = Vector3.zero;
			}
		}
	}
}

#endif