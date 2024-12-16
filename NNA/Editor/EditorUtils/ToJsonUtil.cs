#if UNITY_EDITOR

using System;
using Newtonsoft.Json.Linq;
using UnityEditor;
using UnityEngine;

namespace nna.jank
{
	/// <summary>
	/// Editor window into which a user can drag any object. This will try to serialize it into Json with Unity's JsonUtility.
	/// </summary>
	public class NNAToJsonUtil : EditorWindow
	{
		private Vector2 scrollPos;
		private UnityEngine.Object Selected;

		private string Json;

		[MenuItem("NNA Tools/To Json Utility")]
		public static void Init()
		{
			NNAToJsonUtil window = EditorWindow.GetWindow(typeof(NNAToJsonUtil)) as NNAToJsonUtil;
			window.titleContent = new GUIContent("Convert Objects to Json");
			window.minSize = new Vector2(600, 700);
			window.Selected = null;
		}

		void OnGUI()
		{
			GUILayout.Space(5);
			GUILayout.BeginHorizontal();
			GUILayout.Label("Select Object", EditorStyles.whiteLargeLabel, GUILayout.ExpandWidth(false));
			var newSelected = (UnityEngine.Object)EditorGUILayout.ObjectField(
				Selected,
				typeof(UnityEngine.Object),
				true,
				GUILayout.ExpandWidth(true)
			);
			GUILayout.EndHorizontal();

			// Run the registered serializers on the selected object
			if(newSelected != Selected)
			{
				Selected = newSelected;
				if(Selected)
				{
					try {
						Json = JObject.Parse(JsonUtility.ToJson(Selected)).ToString(Newtonsoft.Json.Formatting.Indented);
					}
					catch(Exception)
					{

					}
				}
			}
			if(Selected && !string.IsNullOrWhiteSpace(Json))
			{
				scrollPos = EditorGUILayout.BeginScrollView(scrollPos, GUILayout.Height(position.height - 20));

				GUILayout.TextArea(Json);

				EditorGUILayout.EndScrollView();
			}
		}
	}
}

#endif
