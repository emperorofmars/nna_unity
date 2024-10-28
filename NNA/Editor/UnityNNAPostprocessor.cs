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
		void OnPostprocessModel(GameObject Root)
		{
			if(!assetPath.ToLower().EndsWith(".nna.fbx")) return;


			var importOptions = NNAImportOptions.Parse(assetImporter.userData);
			if(importOptions != null && importOptions.NNAEnabled)
			{
				var nnaContext = new NNAContext(Root, importOptions);
				NNAConverter.Convert(nnaContext);
				foreach(var (Name, NewObject) in nnaContext.GetNewObjects())
				{
					context.AddObjectToAsset(Name, NewObject);
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
