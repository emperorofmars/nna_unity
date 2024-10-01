#if AVA_VRCSDK3_FOUND
#if UNITY_EDITOR

using Newtonsoft.Json.Linq;
using nna.processors;
using UnityEngine;
using VRC.SDK3.Avatars.Components;
using System.Threading.Tasks;
using System.Linq;
using System;
using System.Collections.Generic;
using UnityEditor;
using nna.ava.applicationconversion.vrc;

namespace nna.ava.vrchat
{
	public class AVAAvatarVRChatProcessor : IProcessor
	{
		public const string _Type = "ava.avatar";
		public string Type => _Type;

		public void Process(NNAContext Context, GameObject Target, GameObject NNANode, JObject Json)
		{
			var avatar = Context.Root.AddComponent<VRCAvatarDescriptor>();

			Context.AddTask(new Task(() => {
				Animator animator = Context.Root.GetComponent<Animator>();
				if(animator == null)
				{
					animator = Context.Root.AddComponent<Animator>();
				}
				animator.applyRootMotion = true;
				animator.updateMode = AnimatorUpdateMode.Normal;
				animator.cullingMode = AnimatorCullingMode.CullUpdateTransforms;

				if(animator.isHuman)
				{
					var humanEyeL = animator.avatar.humanDescription.human.FirstOrDefault(hb => hb.humanName == HumanBodyBones.LeftEye.ToString());
					var humanEyeR = animator.avatar.humanDescription.human.FirstOrDefault(hb => hb.humanName == HumanBodyBones.RightEye.ToString());

					avatar.customEyeLookSettings.leftEye = FindBone(animator.avatarRoot, humanEyeL.boneName);
					avatar.customEyeLookSettings.rightEye  = FindBone(animator.avatarRoot, humanEyeR.boneName);
				}
			}));
		}

		private Transform FindBone(Transform Root, string Name)
		{
			foreach(var t in Root.GetComponentsInChildren<Transform>()) if(t.name == Name) return t;
			return null;
		}
	}
	public class AVAViewportVRChatProcessor : IProcessor
	{
		public const string _Type = "ava.viewport";
		public string Type => _Type;

		public void Process(NNAContext Context, GameObject Target, GameObject NNANode, JObject Json)
		{
			Context.AddTask(new Task(() => {
				var avatar = Context.Root.GetComponent<VRCAvatarDescriptor>();
				avatar.ViewPosition = NNANode.transform.position - Context.Root.transform.position;
			}));
		}
	}
	public class AVAEyetrackingVRChatProcessor : IProcessor
	{
		public const string _Type = "ava.eyetracking";
		public string Type => _Type;
		

		public static readonly List<string> EyeLidExpressions = new List<string> {
			"eye_closed", "look_up", "look_down"
		};

		public void Process(NNAContext Context, GameObject Target, GameObject NNANode, JObject Json)
		{
			var eyelookType = (string)ParseUtil.GetMulkikeyOrDefault(Json, "r", "lkt", "look-type");
			var eyelidType = (string)ParseUtil.GetMulkikeyOrDefault(Json, "b", "ldt", "lid-type");
			var meshGo = ParseUtil.ResolvePath(Context.Root.transform, Target.transform, (string)ParseUtil.GetMulkikeyOrDefault(Json, "Body", "m", "mesh"));

			Context.AddTask(new Task(() => {
				var avatar = Context.Root.GetComponent<VRCAvatarDescriptor>();
				avatar.enableEyeLook = true;

				if(eyelookType == "r") // eye bone rotations
				{
					var up = (float)ParseUtil.GetMulkikeyOrDefault(Json, 15.0f, "u", "up");
					var down = (float)ParseUtil.GetMulkikeyOrDefault(Json, 12.0f, "d", "down");
					var inner = (float)ParseUtil.GetMulkikeyOrDefault(Json, 15.0f, "i", "inner");
					var outer = (float)ParseUtil.GetMulkikeyOrDefault(Json, 16.0f, "o", "outer");

					avatar.customEyeLookSettings.eyesLookingUp = new VRCAvatarDescriptor.CustomEyeLookSettings.EyeRotations
							{left = Quaternion.Euler(-up, 0f, 0f), right = Quaternion.Euler(-up, 0f, 0f), linked = true};
					avatar.customEyeLookSettings.eyesLookingDown = new VRCAvatarDescriptor.CustomEyeLookSettings.EyeRotations
							{left = Quaternion.Euler(down, 0f, 0f), right = Quaternion.Euler(down, 0f, 0f), linked = true};
					avatar.customEyeLookSettings.eyesLookingLeft = new VRCAvatarDescriptor.CustomEyeLookSettings.EyeRotations
							{left = Quaternion.Euler(0f, -outer, 0f), right = Quaternion.Euler(0f, -inner, 0f), linked = false};
					avatar.customEyeLookSettings.eyesLookingRight = new VRCAvatarDescriptor.CustomEyeLookSettings.EyeRotations
							{left = Quaternion.Euler(0f, inner, 0f), right = Quaternion.Euler(0f, outer, 0f), linked = false};
				}
				else if(eyelookType == "b") // eye blendshapes
				{
					throw new NotImplementedException();
				}

				if(eyelidType == "r") // eyelid rotations
				{
					avatar.customEyeLookSettings.eyelidType = VRCAvatarDescriptor.EyelidType.Blendshapes;

					throw new NotImplementedException();
				}
				else if(eyelidType == "b") // eyelid blendshapes
				{
					avatar.customEyeLookSettings.eyelidType = VRCAvatarDescriptor.EyelidType.Blendshapes;
					var smr = meshGo.GetComponent<SkinnedMeshRenderer>();
					
					avatar.customEyeLookSettings.eyelidType = VRCAvatarDescriptor.EyelidType.Blendshapes;
					avatar.customEyeLookSettings.eyelidsSkinnedMesh = smr;
					avatar.customEyeLookSettings.eyelidsBlendshapes = new int[3];
					
					// Also allow for these to be explicitely mapped in the nna component
					avatar.customEyeLookSettings.eyelidsBlendshapes[0] = GetBlendshapeIndex(smr.sharedMesh, MapEyeLidBlendshapes(smr.sharedMesh, "eye_closed"));
					avatar.customEyeLookSettings.eyelidsBlendshapes[1] = GetBlendshapeIndex(smr.sharedMesh, MapEyeLidBlendshapes(smr.sharedMesh, "look_up"));
					avatar.customEyeLookSettings.eyelidsBlendshapes[2] = GetBlendshapeIndex(smr.sharedMesh, MapEyeLidBlendshapes(smr.sharedMesh, "look_down"));
				}
			}));
		}

		private static string MapEyeLidBlendshapes(Mesh Mesh, string Name)
		{
			var compare = Name.Split('_');
			string match = null;
			for(int i = 0; i < Mesh.blendShapeCount; i++)
			{
				var bName = Mesh.GetBlendShapeName(i);
				bool matchedAll = true;
				foreach(var c in compare)
				{
					if(!bName.ToLower().Contains(c)) { matchedAll = false; break; }
				}
				if(matchedAll && (match == null || bName.Length < match.Length)) match = bName;
			}
			return match;
		}

		private static int GetBlendshapeIndex(Mesh mesh, string name)
		{
			for(int i = 0; i < mesh.blendShapeCount; i++)
			{
				var bName = mesh.GetBlendShapeName(i);
				if(bName == name) return i;
			}
			return -1;
		}
	}

	[InitializeOnLoad]
	public class Register_AVAVRChatProcessor
	{
		static Register_AVAVRChatProcessor()
		{
			NNARegistry.RegisterProcessor(new AVAAvatarVRChatProcessor(), AVAAvatarVRChatProcessor._Type, DetectorVRC.NNA_VRC_AVATAR_CONTEXT);
			NNARegistry.RegisterProcessor(new AVAViewportVRChatProcessor(), AVAViewportVRChatProcessor._Type, DetectorVRC.NNA_VRC_AVATAR_CONTEXT);
			NNARegistry.RegisterProcessor(new AVAEyetrackingVRChatProcessor(), AVAEyetrackingVRChatProcessor._Type, DetectorVRC.NNA_VRC_AVATAR_CONTEXT);
		}
	}
}

#endif
#endif
