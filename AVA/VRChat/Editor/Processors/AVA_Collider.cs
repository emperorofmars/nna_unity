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
	public class AVA_Collider_VRCJsonProcessor : IJsonProcessor
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

	public class AVA_Collider_VRCSerializer : INNASerializer
	{
		public static readonly System.Type _Target = typeof(VRCPhysBoneCollider);
		public System.Type Target => _Target;

		public List<JsonSerializerResult>  Serialize(UnityEngine.Object UnityObject)
		{
			var physbone = (VRCPhysBoneCollider)UnityObject;
			var ret = new JObject {
				{"t", AVA_Collider_VRCJsonProcessor._Type},
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

			return new List<JsonSerializerResult>{new(){
				NNAType = AVA_Collider_VRCJsonProcessor._Type,
				Origin = UnityObject,
				JsonResult = ret.ToString(Newtonsoft.Json.Formatting.None),
				JsonTargetNode = physbone.rootTransform ? physbone.rootTransform.name : physbone.transform.name,
				IsJsonComplete = true,
			}};
		}
	}

	[InitializeOnLoad]
	public class Register_AVA_Collider_VRC
	{
		static Register_AVA_Collider_VRC()
		{
			NNARegistry.RegisterJsonProcessor(new AVA_Collider_VRCJsonProcessor(), DetectorVRC.NNA_VRC_AVATAR_CONTEXT);
			NNAExportRegistry.RegisterSerializer(new AVA_Collider_VRCSerializer());
		}
	}
}

#endif
#endif