
#if UNITY_EDITOR

using UnityEngine;
using UnityEditor;
using nna.util;

namespace nna
{
	class UnityNNAPostprocessor : AssetPostprocessor
	{
		void OnPostprocessModel(GameObject Root)
		{
			var nnaContext = new NNAContext(Root, assetImporter.userData);
			NNAConverter.Convert(nnaContext);
			foreach(var (Name, NewObject) in nnaContext.GetNewObjects())
			{
				context.AddObjectToAsset(Name, NewObject);
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
