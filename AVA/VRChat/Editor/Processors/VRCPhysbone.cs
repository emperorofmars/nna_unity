#if UNITY_EDITOR

using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using nna.processors;
using nna.UnityToNNAUtils;
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

	public class VRCPhysboneExporter : INNAJsonSerializer
	{
		public static readonly System.Type _Target = typeof(VRCPhysBone);
		public System.Type Target => _Target;

		public List<(string, string)>  Serialize(UnityEngine.Object UnityObject)
		{
			var physbone = (VRCPhysBone)UnityObject;			
			var ret = new JObject {{"t", VRCPhysboneProcessor._Type}};

			ret.Add("integration_type", physbone.integrationType.ToString());
			ret.Add("pull", physbone.pull);

			return new List<(string, string)>{(VRCPhysboneProcessor._Type, ret.ToString(Newtonsoft.Json.Formatting.None))};
		}
	}

	[InitializeOnLoad]
	public class Register_VRCPhysbone
	{
		static Register_VRCPhysbone()
		{
			NNARegistry.RegisterJsonProcessor(new VRCPhysboneProcessor(), DetectorVRC.NNA_VRC_AVATAR_CONTEXT);
			//NNAJsonExportRegistry.RegisterSerializer(new VRCPhysboneExporter());
		}
	}
}

#endif