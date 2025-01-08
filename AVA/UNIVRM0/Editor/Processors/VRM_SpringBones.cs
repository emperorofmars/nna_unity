#if UNITY_EDITOR
#if NNA_AVA_UNIVRM0_FOUND

using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using nna.ava.common;
using nna.processors;
using nna.UnityToNNAUtils;
using UnityEditor;
using UnityEngine;
using VRM;

namespace nna.ava.univrm0
{
	public class VRM_SpringBones_VRM_JsonProcessor : IJsonProcessor
	{
		public const string _Type = "vrm.springbone";
		public string Type => _Type;
		public uint Order => Base_AVA_Collider_NameProcessor._Order + 1; // Colliders have to be parsed first
		public int Priority => 0;

		public void Process(NNAContext Context, Transform Node, JObject Json)
		{
			var targetNode = PhysicsLocationUtil.GetPhysicsNode(Context, Node, "", null, "VRM_secondary", true);
			var springBone = targetNode.gameObject.AddComponent<VRMSpringBone>();

			JsonUtility.FromJsonOverwrite(Json["parsed"].ToString(), springBone);

			var colliderGroups = new List<VRMSpringBoneColliderGroup>();

			if(Json.ContainsKey("colliders") && Json["colliders"].Type == JTokenType.Array)
			{
				var colliders = Json["colliders"];
				foreach(var id in colliders)
					foreach(var result in Context.GetResultsById((string)id))
						if(result is VRMSpringBoneColliderGroup)
							colliderGroups.Add(result as VRMSpringBoneColliderGroup);
						else
							Context.Report(new("Didn't find SpringBone Collider: " + id, NNAErrorSeverity.WARNING, _Type, Node));
			}

			springBone.ColliderGroups = colliderGroups.ToArray();
			springBone.RootBones.Add(Node);

			if(Json.ContainsKey("id")) Context.AddResultById((string)Json["id"], springBone);
		}
	}


	public class VRM_SpringBones_VRMSerializer : INNASerializer
	{
		public static readonly System.Type _Target = typeof(VRMSpringBone);
		public System.Type Target => _Target;

		public List<SerializerResult> Serialize(NNASerializerContext Context, UnityEngine.Object UnityObject)
		{
			var springBone = (VRMSpringBone)UnityObject;
			var retJson = new JObject {{"t", VRM_SpringBones_VRM_JsonProcessor._Type}};
			if(UnityObject.name.StartsWith("$nna:")) retJson.Add("id", UnityObject.name[5..]);

			var colliders = new JArray();
			foreach(var t in springBone.ColliderGroups) if(t) colliders.Add(t.name.StartsWith("$nna:") ? t.name[5..] : t.name);
			if(colliders.Count > 0) retJson.Add("colliders", colliders);

			var parsed = JObject.Parse(JsonUtility.ToJson(springBone));
			parsed.Remove("RootBones");
			parsed.Remove("ColliderGroups");

			retJson.Add("parsed", parsed);

			var ret = new List<SerializerResult>();
			foreach(var root in springBone.RootBones)
			{
				ret.Add(new() {
					NNAType = VRM_SpringBones_VRM_JsonProcessor._Type,
					Origin = UnityObject,
					JsonResult = retJson.ToString(Newtonsoft.Json.Formatting.None),
					JsonComponentId = Context.GetId(UnityObject),
					JsonTargetNode = root.name,
					IsJsonComplete = true,
					Confidence = SerializerResultConfidenceLevel.MANUAL,
				});
			}
			return ret;
		}
	}


	[InitializeOnLoad]
	public class Register_VRM_SpringBones_VRM_JsonProcessor
	{
		static Register_VRM_SpringBones_VRM_JsonProcessor()
		{
			NNARegistry.RegisterJsonProcessor(new VRM_SpringBones_VRM_JsonProcessor(), DetectorUNIVRM0.NNA_UNIVRM0_CONTEXT);
			NNAExportRegistry.RegisterSerializer(new VRM_SpringBones_VRMSerializer());
		}
	}
}

#endif
#endif
