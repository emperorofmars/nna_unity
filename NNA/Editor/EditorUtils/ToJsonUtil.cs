#if UNITY_EDITOR

using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using nna.UnityToNNAUtils;
using UnityEditor;
using UnityEngine;

namespace nna.jank
{
	/// <summary>
	/// Editor window into which a user can drag any object. This will try to serialize it into Json, either based on manual implementations in the NNAJsonExportRegistry, or with Unity's JsonUtility as a fallback.
	/// </summary>
	public class ToJsonUtil : EditorWindow
	{
		private Vector2 scrollPos;
		private Object Selected;

		private List<(string ComponentType, string Json)> ExportJson = new();

		[MenuItem("NNA Tools/Convert Objects to Json")]
		public static void Init()
		{
			ToJsonUtil window = EditorWindow.GetWindow(typeof(ToJsonUtil)) as ToJsonUtil;
			window.titleContent = new GUIContent("Convert Objects to Json");
			window.minSize = new Vector2(600, 700);
			window.Selected = null;
		}
		
		void OnGUI()
		{
			GUILayout.BeginHorizontal();
			GUILayout.Label("Select Object", EditorStyles.whiteLargeLabel, GUILayout.ExpandWidth(false));
			var newSelected = (Object)EditorGUILayout.ObjectField(
				Selected,
				typeof(Object),
				true,
				GUILayout.ExpandWidth(true)
			);
			GUILayout.EndHorizontal();

			if(newSelected != Selected || ExportJson == null)
			{
				Selected = newSelected;
				if(Selected != null)
				{
					ExportJson = new List<(string ComponentType, string Json)>();
					foreach(var serializer in NNAJsonExportRegistry.Serializers.FindAll(s => Selected.GetType() == s.Target))
					{
						ExportJson.AddRange(serializer.Serialize(Selected));
					}
				}
			}
			scrollPos = EditorGUILayout.BeginScrollView(scrollPos, GUILayout.Height(position.height - 30));
			if(Selected && ExportJson.Count == 0)
			{
				GUILayout.Space(10);
				try
				{
					var json = JsonUtility.ToJson(Selected);
					GUILayout.Label("No Serializer detected! Fallback JsonUtility Succeded!", GUILayout.ExpandWidth(false));
					EditorGUILayout.TextArea(JObject.Parse(json).ToString(Newtonsoft.Json.Formatting.Indented));
				}
				catch(System.Exception)
				{
					GUILayout.Label("No Serializer detected! Fallback JsonUtility Failed!", GUILayout.ExpandWidth(false));
				}
			}
			else if(Selected)
			{
				GUILayout.Space(10);
				GUILayout.Label("Parsed NNA components.", GUILayout.ExpandWidth(false));
				GUILayout.Label("In Blender create a new 'Raw Json' component on the appropriate Object or Bone, and paste the text inside.", GUILayout.ExpandWidth(false));
				GUILayout.Space(10);
				foreach(var (componentType, json) in ExportJson)
				{
					GUILayout.BeginHorizontal();
					if(GUILayout.Button("Copy to Clipboard", GUILayout.ExpandWidth(false))) GUIUtility.systemCopyBuffer = json;
					GUILayout.Label(componentType, GUILayout.ExpandWidth(false));
					GUILayout.EndHorizontal();
					EditorGUILayout.TextArea(json);
					GUILayout.Space(10);
				}
			}
			EditorGUILayout.EndScrollView();
		}
	}
}

#endif