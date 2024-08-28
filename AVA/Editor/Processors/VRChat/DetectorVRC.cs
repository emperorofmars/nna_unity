#if UNITY_EDITOR

using System.IO;
using UnityEditor;
using UnityEngine;

namespace nna.ava.vrchat
{
	[InitializeOnLoad, ExecuteInEditMode]
	public class DetectorVRC
	{
		const string NNA_AVA_VRCSDK3_FOUND = "NNA_AVA_VRCSDK3_FOUND";

		static DetectorVRC()
		{
			//if(AppDomain.CurrentDomain.GetAssemblies().Any(a => a.FullName.StartsWith("VRC.SDK3.Avatars")))
			if(Directory.GetFiles(Path.GetDirectoryName(Application.dataPath), "VRCAvatarDescriptorEditor3.cs", SearchOption.AllDirectories).Length > 0)
			{
				Debug.Log("AVA: Found VRC SDK 3");
				ScriptDefinesManager.AddDefinesIfMissing(BuildTargetGroup.Standalone, NNA_AVA_VRCSDK3_FOUND);
			}
			else
			{
				Debug.Log("AVA: Didn't find VRC SDK 3");
				ScriptDefinesManager.RemoveDefines(NNA_AVA_VRCSDK3_FOUND);
			}
		}
	}
}

#endif