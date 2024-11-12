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
		private Object selected;

		private List<(string ComponentType, string Json)> ExportJson = new();

		[MenuItem("NNA Tools/Convert Objects to Json")]
		public static void Init()
		{
			ToJsonUtil window = EditorWindow.GetWindow(typeof(ToJsonUtil)) as ToJsonUtil;
			window.titleContent = new GUIContent("Convert Objects to Json");
			window.minSize = new Vector2(600, 700);
		}
		
		void OnGUI()
		{
			GUILayout.BeginHorizontal();
			GUILayout.Label("Select Object", EditorStyles.whiteLargeLabel, GUILayout.ExpandWidth(false));
			var newSelected = (Object)EditorGUILayout.ObjectField(
				selected,
				typeof(Object),
				true,
				GUILayout.ExpandWidth(true)
			);
			GUILayout.EndHorizontal();

			if(newSelected != selected)
			{
				selected = newSelected;
				if(selected != null)
				{
					ExportJson = new List<(string ComponentType, string Json)>();
					foreach(var serializer in NNAJsonExportRegistry.Serializers.FindAll(s => selected.GetType() == s.Target))
					{
						ExportJson.AddRange(serializer.Serialize(selected));
					}
				}
			}
			scrollPos = EditorGUILayout.BeginScrollView(scrollPos, GUILayout.Height(position.height - 30));
			if(selected && ExportJson.Count == 0)
			{
				try
				{
					var json = JsonUtility.ToJson(selected);
					GUILayout.Label("No Serializer detected! Fallback JsonUtility Succeded!", GUILayout.ExpandWidth(false));
					EditorGUILayout.TextArea(JObject.Parse(json).ToString(Newtonsoft.Json.Formatting.Indented));
				}
				catch(System.Exception)
				{
					GUILayout.Label("No Serializer detected! Fallback JsonUtility Failed!", GUILayout.ExpandWidth(false));
				}
			}
			else if(selected)
			{
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