#if UNITY_EDITOR
#if NNA_AVA_VRCSDK3_FOUND

using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using nna.processors;
using nna.UnityToNNAUtils;
using UnityEditor;
using UnityEngine;
using VRC.SDK3.Dynamics.PhysBone.Components;

namespace nna.ava.vrchat
{
	public class VRCPhysboneColliderProcessor : IJsonProcessor
	{
		public const string _Type = "ava.collider";
		public string Type => _Type;
		public const uint _Order = 0;
		public uint Order => _Order;

		public void Process(NNAContext Context, Transform Node, JObject Json)
		{
			var collider = Node.gameObject.AddComponent<VRCPhysBoneCollider>();
			if(Json.ContainsKey("id") && ((string)Json["id"]).Length > 0) collider.name = (string)Json["id"];

			JsonUtility.FromJsonOverwrite(Json["parsed"].ToString(), collider);
		}
	}

	public class VRCPhysboneColliderExporter : INNAJsonSerializer
	{
		public static readonly System.Type _Target = typeof(VRCPhysBoneCollider);
		public System.Type Target => _Target;

		public List<(string, string)>  Serialize(UnityEngine.Object UnityObject)
		{
			var physbone = (VRCPhysBoneCollider)UnityObject;
			var ret = new JObject {
				{"t", VRCPhysboneColliderProcessor._Type},
				{"id", physbone.name}
			};

			/*var ignoreTransforms = new JArray();
			foreach(var t in physbone.ignoreTransforms) if(t) ignoreTransforms.Add(t.name);
			if(ignoreTransforms.Count > 0) ret.Add("ignoreTransforms", ignoreTransforms);

			var colliders = new JArray();
			foreach(var t in physbone.colliders) if(t) colliders.Add(t.name);
			if(colliders.Count > 0) ret.Add("colliders", colliders);*/

			var parsed = JObject.Parse(JsonUtility.ToJson(physbone));
			parsed.Remove("rootTransform");
			ret.Add("parsed", parsed);

			// handle rootTransform

			return new List<(string, string)>{(VRCPhysboneColliderProcessor._Type, ret.ToString(Newtonsoft.Json.Formatting.None))};
		}
	}

	[InitializeOnLoad]
	public class Register_VRCPhysboneCollider
	{
		static Register_VRCPhysboneCollider()
		{
			NNARegistry.RegisterJsonProcessor(new VRCPhysboneColliderProcessor(), DetectorVRC.NNA_VRC_AVATAR_CONTEXT);
			NNAJsonExportRegistry.RegisterSerializer(new VRCPhysboneColliderExporter());
		}
	}
}

#endif
#endif