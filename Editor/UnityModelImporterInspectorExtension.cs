
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

		private static string SelectedImportContext = NNARegistry.DefaultContext;

		private static void ShowQualityOfLifeButtons(Editor editor)
		{
			if (!editor.target || editor.target is not ModelImporter)
				return;

			var contextOptions = NNARegistry.GetAvaliableContexts();
			int selectedIndex = contextOptions.FindIndex(c => c == SelectedImportContext);

			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.PrefixLabel("Select Import Context");
			selectedIndex = EditorGUILayout.Popup(selectedIndex, contextOptions.ToArray());
			EditorGUILayout.EndHorizontal();

			if(selectedIndex >= 0 && selectedIndex < contextOptions.Count) SelectedImportContext = contextOptions[selectedIndex];
			else SelectedImportContext = NNARegistry.DefaultContext;
		}
	}
}

#endif

