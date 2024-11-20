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

		private List<JsonSerializerResult> SerializerResult = new();

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

			// Run the registered serializers on the selected object
			if(newSelected != Selected || SerializerResult == null)
			{
				Selected = newSelected;
				SerializerResult = new List<JsonSerializerResult>();
				if(Selected != null)
				{
					SerializerResult = RunJsonSerializer.Run(Selected);
				}
			}

			// Draw the results
			if(Selected && SerializerResult.Count == 0)
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
				//GUILayout.Label("In Blender create a new 'Raw Json' component on the appropriate Object or Bone, and paste the text inside.", GUILayout.ExpandWidth(false));
				
				GUILayout.Space(10);

				GUILayout.Label("TODO: Copy Setup to Clipboard");
				GUILayout.Space(10);
				DrawHLine(2, 0);

				scrollPos = EditorGUILayout.BeginScrollView(scrollPos, GUILayout.Height(position.height - 30));
				foreach(var result in SerializerResult)
				{
					GUILayout.BeginHorizontal();
						GUILayout.Label(result.NNAType, EditorStyles.whiteLargeLabel);
					GUILayout.EndHorizontal();

					GUILayout.BeginHorizontal();
						GUILayout.Space(20);
						GUILayout.BeginVertical();
							GUILayout.BeginHorizontal();
								GUILayout.Label("Origin");
								EditorGUILayout.ObjectField(result.Origin, typeof(UnityEngine.Object), true, GUILayout.Width(400));
							GUILayout.EndHorizontal();
						GUILayout.EndVertical();
						GUILayout.FlexibleSpace();
					GUILayout.EndHorizontal();

					GUILayout.Space(5);

					GUILayout.BeginHorizontal();
						GUILayout.Space(20);
						GUILayout.BeginVertical();
							GUILayout.BeginHorizontal(GUILayout.Width(250));
								GUILayout.Label("Json Component");
								if(!result.IsJsonComplete && !string.IsNullOrWhiteSpace(result.JsonResult)) GUILayout.Label("(Incomplete)");
							GUILayout.EndHorizontal();
							GUILayout.BeginHorizontal();
								GUILayout.Space(20);
								GUILayout.BeginVertical();
									if(!string.IsNullOrWhiteSpace(result.JsonResult))
									{
										GUILayout.Label("Target: " + result.JsonTargetNode);
										if(!string.IsNullOrWhiteSpace(result.DeviatingJsonType))GUILayout.Label("Json Specific Type" + result.DeviatingJsonType);
										if(GUILayout.Button("Copy to Clipboard", GUILayout.ExpandWidth(false))) GUIUtility.systemCopyBuffer = result.JsonResult;
									}
									else
									{
										GUILayout.Label("Not Serialized");
									}
								GUILayout.EndVertical();
							GUILayout.EndHorizontal();
						GUILayout.EndVertical();

						GUILayout.Space(40);

						GUILayout.BeginVertical(GUILayout.ExpandWidth(false));
							GUILayout.BeginHorizontal();
								GUILayout.Label("Name Component");
								if(!result.IsNameComplete && !string.IsNullOrWhiteSpace(result.NameResult)) GUILayout.Label("(Incomplete)");
							GUILayout.EndHorizontal();
							GUILayout.BeginHorizontal();
								GUILayout.Space(20);
								GUILayout.BeginVertical();
									if(!string.IsNullOrWhiteSpace(result.NameResult))
									{
										GUILayout.Label("Target: " + result.NameTargetNode);
										if(!string.IsNullOrWhiteSpace(result.DeviatingJsonType))GUILayout.Label("Name Specific Type" + result.DeviatingNameType);
										if(GUILayout.Button("Copy to Clipboard", GUILayout.ExpandWidth(false))) GUIUtility.systemCopyBuffer = result.NameResult;
									}
									else
									{
										GUILayout.Label("Not Serialized");
									}
								GUILayout.EndVertical();
							GUILayout.EndHorizontal();
						GUILayout.EndVertical();

						GUILayout.FlexibleSpace();
					GUILayout.EndHorizontal();

					DrawHLine(1);
				}
				EditorGUILayout.EndScrollView();
			}
		}
		
		private void DrawHLine(float Thickness = 2, float Spacers = 10) {
			GUILayout.Space(Spacers);
			EditorGUI.DrawRect(EditorGUILayout.GetControlRect(false, Thickness), Color.gray);
			GUILayout.Space(Spacers);
		}
	}
}

#endif