#if UNITY_EDITOR

using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using nna.UnityToNNAUtils;
using UnityEditor;
using UnityEngine;

namespace nna.jank
{
	public class ToJsonUtil : EditorWindow
	{
		private Vector2 scrollPos;
		private Object selected;
		private string parsedJson = null;

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
					var exports = new List<string>();
					foreach(var serializer in NNAJsonExportRegistry.Serializers.FindAll(s => selected.GetType() == s.Target))
					{
						exports.AddRange(serializer.Serialize(selected));
					}
					if(exports.Count == 0)
					{
						try
						{
							var tmpJson = JsonUtility.ToJson(selected);
							parsedJson = JObject.Parse(tmpJson).ToString(Newtonsoft.Json.Formatting.Indented);
						}
						catch(System.Exception e)
						{
							parsedJson = e.Message;
						}
					}
					else
					{
						parsedJson = "";
						foreach(var e in exports) parsedJson += e + "\n\n";
					}
				}
				else
				{
					parsedJson = "";
				}
			}

			scrollPos = EditorGUILayout.BeginScrollView(scrollPos, GUILayout.Height(position.height - 30));
			EditorGUILayout.TextArea(parsedJson);
			EditorGUILayout.EndScrollView();
		}

		private void drawHLine() {
			GUILayout.Space(10);
			EditorGUI.DrawRect(EditorGUILayout.GetControlRect(false, 2), Color.gray);
			GUILayout.Space(10);
		}
	}
}

#endif