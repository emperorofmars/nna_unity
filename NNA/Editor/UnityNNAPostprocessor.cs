#if UNITY_EDITOR

using UnityEngine;
using UnityEditor;
using nna.util;

namespace nna
{
	/// <summary>
	/// The main entry point for Unity into NNA.
	/// </summary>
	class UnityNNAPostprocessor : AssetPostprocessor
	{
#pragma warning disable IDE0051 // Remove unused private members
		void OnPostprocessModel(GameObject Root)
#pragma warning restore IDE0051 // Remove unused private members
		{
			if(NNAImportOptions.Parse(assetImporter.userData) is var nnaImportOptions && nnaImportOptions == null)
			{
				nnaImportOptions = new NNAImportOptions();
				if(!assetImporter.assetPath.ToLower().EndsWith(".nna.fbx")) nnaImportOptions.NNAEnabled = false;
			}
			if(nnaImportOptions.NNAEnabled)
			{
				var result = NNAConverter.Convert(Root, nnaImportOptions);
				foreach(var (Name, NewObject) in result.NewObjects)
				{
					context.AddObjectToAsset(Name, NewObject);
				}
				foreach(var (OldObject, NewObject) in result.Remaps)
				{
					if(NewObject)
					{
						((ModelImporter)assetImporter).AddRemap(new AssetImporter.SourceAssetIdentifier(OldObject), NewObject);
					}
					else
					{
						Debug.LogWarning($"NNA could not remap asset: {OldObject}");
					}
				}
			}
		}
	}
	
	[InitializeOnLoad, ExecuteInEditMode]
	public class NNA_DefineManager
	{
		const string NNA = "NNA";

		static NNA_DefineManager()
		{
			ScriptDefinesManager.AddDefinesIfMissing(BuildTargetGroup.Standalone, NNA);
		}
	}
}

#endif
