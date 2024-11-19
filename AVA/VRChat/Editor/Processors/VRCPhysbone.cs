#if UNITY_EDITOR
#if NNA_AVA_VRCSDK3_FOUND

using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using nna.processors;
using nna.UnityToNNAUtils;
using UnityEditor;
using UnityEngine;
using VRC.Dynamics;
using VRC.SDK3.Dynamics.PhysBone.Components;

namespace nna.ava.vrchat
{
	public class VRCPhysboneProcessor : IJsonProcessor
	{
		public const string _Type = "vrc.physbone";
		public string Type => _Type;
		public uint Order => VRCPhysboneColliderProcessor._Order + 1; // Colliders have to be parsed first

		public void Process(NNAContext Context, Transform Node, JObject Json)
		{
			var physbone = Node.gameObject.AddComponent<VRCPhysBone>();
			if(Json.ContainsKey("id") && ((string)Json["id"]).Length > 0) physbone.name = (string)Json["id"];

			JsonUtility.FromJsonOverwrite(Json["parsed"].ToString(), physbone);

			if(Json.TryGetValue("ignoreTransforms", out var ignoreTransforms) && ignoreTransforms.Type != JTokenType.Array)
			{
				foreach(string name in ignoreTransforms)
				{
					var node = ParseUtil.FindNode(Context.Root.transform, name);
					physbone.ignoreTransforms.Add(node);
				}
			}
			if(Json.TryGetValue("colliders", out var colliders) && colliders.Type != JTokenType.Array)
			{
				foreach(string name in colliders)
				{
					var node = ParseUtil.FindNode(Context.Root.transform, name);
					if(node.TryGetComponent<VRCPhysBoneColliderBase>(out var collider))
						physbone.colliders.Add(collider);
				}
			}
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

			var ignoreTransforms = new JArray();
			foreach(var t in physbone.ignoreTransforms) if(t) ignoreTransforms.Add(t.name);
			if(ignoreTransforms.Count > 0) ret.Add("ignoreTransforms", ignoreTransforms);

			var colliders = new JArray();
			foreach(var t in physbone.colliders) if(t) colliders.Add(t.name);
			if(colliders.Count > 0) ret.Add("colliders", colliders);
			
			// handle rootTransform

			var parsed = JObject.Parse(JsonUtility.ToJson(physbone));
			parsed.Remove("rootTransform");
			parsed.Remove("ignoreTransforms");
			parsed.Remove("colliders");
			ret.Add("parsed", parsed);

			return new List<(string, string)>{(VRCPhysboneProcessor._Type, ret.ToString(Newtonsoft.Json.Formatting.None))};
		}
	}

	[InitializeOnLoad]
	public class Register_VRCPhysbone
	{
		static Register_VRCPhysbone()
		{
			NNARegistry.RegisterJsonProcessor(new VRCPhysboneProcessor(), DetectorVRC.NNA_VRC_AVATAR_CONTEXT);
			NNAJsonExportRegistry.RegisterSerializer(new VRCPhysboneExporter());
		}
	}
}

#endif
#endif