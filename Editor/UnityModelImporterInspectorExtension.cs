
#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;

namespace nna.jank
{
	// Taken from: https://discussions.unity.com/t/trying-to-add-new-data-to-fbx-imports-is-absolutely-miserable/906116/7
	[InitializeOnLoad]
	public static class UnityModelImporterInspectorExtension
	{
		static UnityModelImporterInspectorExtension() => Editor.finishedDefaultHeaderGUI += ShowQualityOfLifeButtons;

		private static void ShowQualityOfLifeButtons(Editor editor)
		{
			if (!editor.target || editor.target is not ModelImporter)
				return;

			GUILayout.BeginHorizontal();
			if (GUILayout.Button("Testbutton A"))
				Debug.Log($"Do whatever you want here");
			if (GUILayout.Button("Testbutton B"))
				Debug.Log($"Do whatever you want here");
			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();

			GUILayout.BeginHorizontal();
			if (GUILayout.Button("Testbutton A"))
				Debug.Log($"Do whatever you want here");
			if (GUILayout.Button("Testbutton B"))
				Debug.Log($"Do whatever you want here");
			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();
		}
	}
}

#endif

