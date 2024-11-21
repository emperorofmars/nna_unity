#if UNITY_EDITOR
#if NNA_AVA_VRCSDK3_FOUND

using Newtonsoft.Json.Linq;
using nna.processors;
using UnityEditor;
using UnityEngine;
using VRC.SDK3.Dynamics.PhysBone.Components;

namespace nna.ava.vrchat
{
	public class AVA_SecondaryMotion_VRCJsonProcessor : IJsonProcessor
	{
		public const string _Type = "ava.secondary_motion";
		public string Type => _Type;
		public uint Order => 0;

		public void Process(NNAContext Context, Transform Node, JObject Json)
		{
			var physbone = Node.gameObject.AddComponent<VRCPhysBone>();
			if(Json.ContainsKey("id") && ((string)Json["id"]).Length > 0) physbone.name = (string)Json["id"];

			// TODO: Proper conversion from the nna secondary motion values to physbone values
		}
	}

	[InitializeOnLoad]
	public class Register_AVA_SecondaryMotion_VRC
	{
		static Register_AVA_SecondaryMotion_VRC()
		{
			NNARegistry.RegisterJsonProcessor(new AVA_SecondaryMotion_VRCJsonProcessor(), DetectorVRC.NNA_VRC_AVATAR_CONTEXT);
		}
	}
}

#endif
#endif