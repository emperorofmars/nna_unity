#if UNITY_EDITOR

using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using nna.UnityToNNAUtils;
using UnityEditor;
using UnityEngine;

namespace nna.jank
{
	/// <summary>
	/// Editor window into which a user can drag any object. This will try to serialize it into Json and name definitions, either based on manual implementations in the NNAExportRegistry, or with Unity's JsonUtility as a fallback.
	/// </summary>
	public class NNAExportUtil : EditorWindow
	{
		private Vector2 scrollPos;
		private Object Selected;

		private List<SerializerResult> SerializerResult = new();
		private string SetupString = "";
		private bool JsonPreference = false;
		private string FallbackJson = "";

		[MenuItem("NNA Tools/NNA Export Utility")]
		public static void Init()
		{
			NNAExportUtil window = EditorWindow.GetWindow(typeof(NNAExportUtil)) as NNAExportUtil;
			window.titleContent = new GUIContent("Convert Objects to Json");
			window.minSize = new Vector2(600, 700);
			window.Selected = null;
		}

		void OnGUI()
		{
			GUILayout.Space(5);
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
			if(newSelected != Selected || SerializerResult == null || SerializerResult.Count == 0)
			{
				Selected = newSelected;
				SerializerResult = new List<SerializerResult>();
				if(Selected)
				{
					SerializerResult = RunNNASerializer.Run(Selected);
					SetupString = RunNNASerializer.CreateSetupString(SerializerResult, JsonPreference);
				}
				else
				{
					try
					{
						FallbackJson = JObject.Parse(JsonUtility.ToJson(Selected)).ToString(Newtonsoft.Json.Formatting.Indented);
					}
					catch(System.Exception)
					{
						FallbackJson = "No Serializer detected! Fallback JsonUtility Failed!";
					}
				}
			}

			// Draw the results
			if(Selected && SerializerResult.Count == 0)
			{
				GUILayout.Space(10);
				GUILayout.Label("JsonUtility Fallback Result", GUILayout.ExpandWidth(false));
				scrollPos = EditorGUILayout.BeginScrollView(scrollPos, GUILayout.Height(position.height - 20));
				EditorGUILayout.TextArea(FallbackJson);
				EditorGUILayout.EndScrollView();
			}
			else if(Selected)
			{
				GUILayout.Space(10);
				GUILayout.BeginHorizontal();
					GUILayout.Label("Parsed NNA Definitions", GUILayout.ExpandWidth(false));
					//GUILayout.Label("In Blender create a new 'Raw Json' component on the appropriate Object or Bone, and paste the text inside.", GUILayout.ExpandWidth(false));

					GUILayout.Space(10);

					var oldJsonPreference = JsonPreference;
					JsonPreference = GUILayout.Toggle(JsonPreference, "Prefer Json Definition");
					if(oldJsonPreference != JsonPreference)
					{
						SetupString = RunNNASerializer.CreateSetupString(SerializerResult, JsonPreference);
					}

					if(!string.IsNullOrWhiteSpace(SetupString) && GUILayout.Button("Copy Full Setup to Clipboard", GUILayout.ExpandWidth(false)))
					{
						GUIUtility.systemCopyBuffer = SetupString;
					}

					GUILayout.FlexibleSpace();
				GUILayout.EndHorizontal();

				GUILayout.Space(5);
				DrawHLine(2, 0);

				scrollPos = EditorGUILayout.BeginScrollView(scrollPos, GUILayout.Height(position.height - 67));
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
										if(GUILayout.Button("Copy to Clipboard", GUILayout.ExpandWidth(false)))
										{
											GUIUtility.systemCopyBuffer = JObject.Parse(result.JsonResult).ToString(Newtonsoft.Json.Formatting.None);
										}
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
			if(Spacers > 0) GUILayout.Space(Spacers);
			EditorGUI.DrawRect(EditorGUILayout.GetControlRect(false, Thickness), Color.gray);
			if(Spacers > 0) GUILayout.Space(Spacers);
		}
	}
}

#endif
