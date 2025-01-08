#if UNITY_EDITOR

using System.IO;
using UnityEditor;
using UnityEngine;

namespace nna.jank
{
	// Taken from: https://discussions.unity.com/t/trying-to-add-new-data-to-fbx-imports-is-absolutely-miserable/906116/7
	[InitializeOnLoad]
	public class UnityModelImporterInspectorExtension
	{
		static UnityModelImporterInspectorExtension() => Editor.finishedDefaultHeaderGUI += ShowQualityOfLifeButtons;
		static bool ShowAdvancedSettings = false;

		private static void ShowQualityOfLifeButtons(Editor editor)
		{
			if (!editor.target || editor.target is not ModelImporter) return;
			var importer = (ModelImporter)editor.target;

			if(NNAImportOptions.Parse(importer.userData) is var nnaImportOptions && nnaImportOptions == null)
			{
				nnaImportOptions = new NNAImportOptions();
				if(!importer.assetPath.ToLower().EndsWith(".nna.fbx")) nnaImportOptions.NNAEnabled = false;
			}

			var contextOptions = NNARegistry.GetAvaliableContexts();
			int selectedIndex = contextOptions.FindIndex(c => c == nnaImportOptions.SelectedContext);

			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.PrefixLabel("Enable NNA Processing");
			nnaImportOptions.NNAEnabled = EditorGUILayout.Toggle(nnaImportOptions.NNAEnabled);
			EditorGUILayout.EndHorizontal();

			if(nnaImportOptions.NNAEnabled)
			{
				EditorGUILayout.BeginHorizontal();
				EditorGUILayout.PrefixLabel("Select Import Context");
				selectedIndex = EditorGUILayout.Popup(selectedIndex, contextOptions.ToArray());
				EditorGUILayout.EndHorizontal();

				EditorGUILayout.Space(5);

				EditorGUILayout.BeginHorizontal();
				EditorGUILayout.PrefixLabel("Change Asset Mapping Directory");
				nnaImportOptions.CustomAssetMappingBaseDir = EditorGUILayout.Toggle(nnaImportOptions.CustomAssetMappingBaseDir);
				EditorGUILayout.EndHorizontal();

				if(nnaImportOptions.CustomAssetMappingBaseDir)
				{
					EditorGUILayout.BeginHorizontal();
					EditorGUILayout.PrefixLabel("Custom Asset Mapping Directory");

					EditorGUILayout.LabelField(nnaImportOptions.AssetMappingBaseDir);

					if(GUILayout.Button("Change"))
					{
						var tmpAssetMappingBaseDir = Path.Combine(Application.dataPath, nnaImportOptions.AssetMappingBaseDir);
						tmpAssetMappingBaseDir = EditorUtility.OpenFolderPanel("Select Custom Asset Mapping Directory", tmpAssetMappingBaseDir, "");
						if(tmpAssetMappingBaseDir != null && tmpAssetMappingBaseDir.Length >= Application.dataPath.Length)
						{
							nnaImportOptions.AssetMappingBaseDir = tmpAssetMappingBaseDir[Application.dataPath.Length..];
						}
					}
					EditorGUILayout.EndHorizontal();
				}

				var newSelectedImportContext = NNARegistry.DefaultContext;
				if(selectedIndex >= 0 && selectedIndex < contextOptions.Count) newSelectedImportContext = contextOptions[selectedIndex];
				else newSelectedImportContext = NNARegistry.DefaultContext;
				nnaImportOptions.SelectedContext = newSelectedImportContext;

				EditorGUILayout.Space(5);

				EditorGUILayout.BeginHorizontal();
				EditorGUILayout.PrefixLabel("Show Advanced NNA Settings");
				ShowAdvancedSettings = EditorGUILayout.Toggle(ShowAdvancedSettings);
				EditorGUILayout.EndHorizontal();

				if(ShowAdvancedSettings)
				{
					EditorGUILayout.Space(5);
					EditorGUILayout.BeginHorizontal();
					EditorGUILayout.Space(20);
					EditorGUILayout.BeginVertical();

					EditorGUILayout.BeginHorizontal();
					EditorGUILayout.PrefixLabel("Clean Up NNA Definitions");
					nnaImportOptions.RemoveNNADefinitions = EditorGUILayout.Toggle(nnaImportOptions.RemoveNNADefinitions);
					EditorGUILayout.EndHorizontal();

					EditorGUILayout.BeginHorizontal();
					EditorGUILayout.PrefixLabel("Abort On Unhandled Non-Fatal Error");
					nnaImportOptions.AbortOnException = EditorGUILayout.Toggle(nnaImportOptions.AbortOnException);
					EditorGUILayout.EndHorizontal();

					EditorGUILayout.EndVertical();
					EditorGUILayout.EndHorizontal();
				}
			}

			if(AssetDatabase.IsMainAssetAtPathLoaded(importer.assetPath))
			{
				foreach(var asset in AssetDatabase.LoadAllAssetsAtPath(importer.assetPath))
				{
					if(asset is NNAErrorList)
					{
						EditorGUILayout.Space(10);
						EditorGUILayout.LabelField("Warning: Import Encountered Errors!");
						EditorGUILayout.BeginHorizontal();
						EditorGUILayout.PrefixLabel("See The Error List Details:");
						EditorGUILayout.ObjectField(asset, typeof(NNAErrorList), false);
						EditorGUILayout.EndHorizontal();
						break;
					}
				}
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
