#if UNITY_EDITOR

using System.IO;
using UnityEditor;
using UnityEngine;
using nna.util;
using UnityEditor.Compilation;

namespace nna.ava.dynamicbones
{
	[InitializeOnLoad, ExecuteInEditMode]
	public class DetectorVRC
	{
		const string NNA_AVA_DYNAMICBONES_FOUND = "NNA_AVA_DYNAMICBONES_FOUND";
		public const string NNA_AVA_DYNAMICBONES_CONTEXT = NNARegistry.DefaultContext;

		static DetectorVRC()
		{
			if(Directory.GetFiles(Path.GetDirectoryName(Application.dataPath), "DynamicBone.cs", SearchOption.AllDirectories).Length > 0)
			{
				Debug.Log("AVA: Found DynamicBones");
				if(!ScriptDefinesManager.IsDefined(NNA_AVA_DYNAMICBONES_FOUND))
				{
					ScriptDefinesManager.AddDefinesIfMissing(BuildTargetGroup.Standalone, NNA_AVA_DYNAMICBONES_FOUND);
					CompilationPipeline.RequestScriptCompilation();
				}
			}
			else
			{
				Debug.Log("AVA: Didn't find DynamicBones");
				if(ScriptDefinesManager.IsDefined(NNA_AVA_DYNAMICBONES_FOUND))
				{
					ScriptDefinesManager.RemoveDefines(NNA_AVA_DYNAMICBONES_FOUND);
					CompilationPipeline.RequestScriptCompilation();
				}
			}
		}
	}
}

#endif
