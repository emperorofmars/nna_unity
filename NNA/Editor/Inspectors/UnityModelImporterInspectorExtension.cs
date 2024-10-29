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
			if (!editor.target || editor.target is not ModelImporter) return;
			var importer = (ModelImporter)editor.target;
			if(!importer.assetPath.ToLower().EndsWith(".nna.fbx")) return;

			if(NNAImportOptions.Parse(importer.userData) is var nnaImportOptions && nnaImportOptions == null) nnaImportOptions = new NNAImportOptions();

			var contextOptions = NNARegistry.GetAvaliableContexts();
			int selectedIndex = contextOptions.FindIndex(c => c == nnaImportOptions.SelectedContext);

			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.PrefixLabel("Enable NNA Processing");
			nnaImportOptions.NNAEnabled = EditorGUILayout.Toggle(nnaImportOptions.NNAEnabled);
			EditorGUILayout.EndHorizontal();

			if(nnaImportOptions.NNAEnabled)
			{
				EditorGUILayout.BeginHorizontal();
				EditorGUILayout.PrefixLabel("Select import context");
				selectedIndex = EditorGUILayout.Popup(selectedIndex, contextOptions.ToArray());
				EditorGUILayout.EndHorizontal();

				var newSelectedImportContext = NNARegistry.DefaultContext;
				if(selectedIndex >= 0 && selectedIndex < contextOptions.Count) newSelectedImportContext = contextOptions[selectedIndex];
				else newSelectedImportContext = NNARegistry.DefaultContext;
				nnaImportOptions.SelectedContext = newSelectedImportContext;

				EditorGUILayout.BeginHorizontal();
				EditorGUILayout.PrefixLabel("Remove NNA Json");
				nnaImportOptions.RemoveNNAJson = EditorGUILayout.Toggle(nnaImportOptions.RemoveNNAJson);
				EditorGUILayout.EndHorizontal();
			}

			if(nnaImportOptions.Modified)
			{
				importer.userData = JsonUtility.ToJson(nnaImportOptions);
				EditorUtility.SetDirty(importer);
			}
		}
	}
}

#endif
