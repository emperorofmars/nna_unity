#if UNITY_EDITOR
#if NNA_AVA_VRCSDK3_FOUND

using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using nna.processors;
using nna.UnityToNNAUtils;
using nna.util;
using UnityEditor;
using UnityEngine;
using VRC.SDK3.Dynamics.PhysBone.Components;

namespace nna.ava.vrchat
{
	public class AVA_Collider_VRCNameProcessor : INameProcessor
	{
		public const string _Type = "ava.collider";
		public string Type => _Type;
		public const uint _Order = 0;
		public uint Order => _Order;

		public int CanProcessName(NNAContext Context, string Name)
		{
			throw new System.NotImplementedException();
		}

		public void Process(NNAContext Context, Transform Node, string Name)
		{
			throw new System.NotImplementedException();
		}
	}

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

		public List<SerializerResult> Serialize(NNASerializerContext Context, UnityEngine.Object UnityObject)
		{
			var collider = (VRCPhysBoneCollider)UnityObject;
			var retJson = new JObject {
				{"t", AVA_Collider_VRCJsonProcessor._Type},
				{"pos_offset", TRSUtil.SerializeVector3(collider.position)},
				{"rot_offset", TRSUtil.SerializeQuat(collider.rotation)}
			};
			if(UnityObject.name.StartsWith("$nna:")) retJson.Add("id", UnityObject.name[5..]);

			var parsed = JObject.Parse(JsonUtility.ToJson(collider));
			parsed.Remove("rootTransform");
			retJson.Add("parsed", parsed);

			return new List<SerializerResult>{new(){
				NNAType = AVA_Collider_VRCJsonProcessor._Type,
				Origin = UnityObject,
				JsonResult = retJson.ToString(Newtonsoft.Json.Formatting.None),
				JsonComponentId = Context.GetId(UnityObject),
				JsonTargetNode = collider.rootTransform ? collider.rootTransform.name : collider.transform.name,
				IsJsonComplete = true,
				Confidence = SerializerResultConfidenceLevel.MANUAL,
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