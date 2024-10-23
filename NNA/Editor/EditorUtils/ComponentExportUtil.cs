#if UNITY_EDITOR

using Newtonsoft.Json.Linq;
using UnityEditor;
using UnityEngine;

namespace nna.jank
{
	public class ComponentExportUtil : EditorWindow
	{
		private Vector2 scrollPos;
		private Component selectedComponent;
		private string parsedJson = null;

		[MenuItem("NNA Tools/NNA Component Export")]
		public static void Init()
		{
			ComponentExportUtil window = EditorWindow.GetWindow(typeof(ComponentExportUtil)) as ComponentExportUtil;
			window.titleContent = new GUIContent("NNA Component Export");
			window.minSize = new Vector2(600, 700);
		}
		
		void OnGUI()
		{
			GUILayout.BeginHorizontal();
			GUILayout.Label("Select Component", EditorStyles.whiteLargeLabel, GUILayout.ExpandWidth(false));
			var newSelectedComponent = (Component)EditorGUILayout.ObjectField(
				selectedComponent,
				typeof(Component),
				true,
				GUILayout.ExpandWidth(true)
			);
			GUILayout.EndHorizontal();

			if(newSelectedComponent != selectedComponent)
			{
				selectedComponent = newSelectedComponent;
				if(selectedComponent != null)
				{
					var tmpJson = JsonUtility.ToJson(selectedComponent);
					parsedJson = JObject.Parse(tmpJson).ToString(Newtonsoft.Json.Formatting.Indented);
				}
				else
				{
					parsedJson = "";
				}
			}

			scrollPos = EditorGUILayout.BeginScrollView(scrollPos, GUILayout.Height(position.height - 30));
			parsedJson = EditorGUILayout.TextArea(parsedJson);
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