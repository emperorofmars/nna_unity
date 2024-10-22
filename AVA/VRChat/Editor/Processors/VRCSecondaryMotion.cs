#if AVA_VRCSDK3_FOUND
#if UNITY_EDITOR

using Newtonsoft.Json.Linq;
using nna.processors;
using UnityEditor;
using UnityEngine;
using VRC.SDK3.Dynamics.PhysBone.Components;

namespace nna.ava.vrchat
{
	public class VRCSecondaryMotionProcessor : IJsonProcessor
	{
		public const string _Type = "ava.secondary_motion";
		public string Type => _Type;

		public void ProcessJson(NNAContext Context, Transform Node, JObject Json)
		{
			var physbone = Node.gameObject.AddComponent<VRCPhysBone>();

			// Todo: Proper conversion from the nna secondary motion values to physbone values
		}
	}

	[InitializeOnLoad]
	public class Register_VRCSecondaryMotionProcessor
	{
		static Register_VRCSecondaryMotionProcessor()
		{
			NNARegistry.RegisterJsonProcessor(new VRCSecondaryMotionProcessor(), VRCSecondaryMotionProcessor._Type, DetectorVRC.NNA_VRC_AVATAR_CONTEXT);
		}
	}
}

#endif
#endif