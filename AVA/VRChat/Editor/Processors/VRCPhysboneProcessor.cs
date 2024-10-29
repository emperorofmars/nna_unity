#if UNITY_EDITOR

using Newtonsoft.Json.Linq;
using nna.processors;
using UnityEditor;
using UnityEngine;
using VRC.SDK3.Dynamics.PhysBone.Components;

namespace nna.ava.vrchat
{
	public class VRCPhysboneProcessor : IJsonProcessor
	{
		public const string _Type = "vrc.physbone";
		public string Type => _Type;

		public void Process(NNAContext Context, Transform Node, JObject Json)
		{
			var physbone = Node.gameObject.AddComponent<VRCPhysBone>();

			if(Json.ContainsKey("pull")) physbone.pull = (float)Json["pull"];
			if(Json.ContainsKey("spring")) physbone.spring = (float)Json["spring"];
			if(Json.ContainsKey("limit_type"))
			{
				switch((string)Json["limit_type"])
				{
					case "angle":
						physbone.limitType = VRC.Dynamics.VRCPhysBoneBase.LimitType.Angle;
						if(Json.ContainsKey("max_angle")) physbone.maxAngleX = physbone.spring = (float)Json["max_angle"];
						break;
				}
			}

			// TODO: All the other properties
		}
	}

	[InitializeOnLoad]
	public class Register_VRCPhysboneProcessor
	{
		static Register_VRCPhysboneProcessor()
		{
			NNARegistry.RegisterJsonProcessor(new VRCPhysboneProcessor(), DetectorVRC.NNA_VRC_AVATAR_CONTEXT);
		}
	}
}

#endif