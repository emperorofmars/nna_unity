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
	public class VRC_Physbone_VRCJsonProcessor : IJsonProcessor
	{
		public const string _Type = "vrc.physbone";
		public string Type => _Type;
		public uint Order => AVA_Collider_VRCJsonProcessor._Order + 1; // Colliders have to be parsed first

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

	public class VRC_Physbone_VRCSerializer : INNASerializer
	{
		public static readonly System.Type _Target = typeof(VRCPhysBone);
		public System.Type Target => _Target;

		public List<SerializerResult> Serialize(NNASerializerContext Context, UnityEngine.Object UnityObject)
		{
			var physbone = (VRCPhysBone)UnityObject;			
			var retJson = new JObject {{"t", VRC_Physbone_VRCJsonProcessor._Type}};
			if(UnityObject.name.StartsWith("$nna:")) retJson.Add("id", UnityObject.name[5..]);

			var ignoreTransforms = new JArray();
			foreach(var t in physbone.ignoreTransforms) if(t) ignoreTransforms.Add(t.name);
			if(ignoreTransforms.Count > 0) retJson.Add("ignoreTransforms", ignoreTransforms);

			var colliders = new JArray();
			foreach(var t in physbone.colliders) if(t) colliders.Add(t.name.StartsWith("$nna:") ? t.name[5..] : t.name);
			if(colliders.Count > 0) retJson.Add("colliders", colliders);
			
			// handle rootTransform

			var parsed = JObject.Parse(JsonUtility.ToJson(physbone));
			parsed.Remove("rootTransform");
			parsed.Remove("ignoreTransforms");
			parsed.Remove("colliders");

			parsed.Remove("foldout_transforms");
			parsed.Remove("foldout_forces");
			parsed.Remove("foldout_collision");
			parsed.Remove("foldout_stretchsquish");
			parsed.Remove("foldout_limits");
			parsed.Remove("foldout_grabpose");
			parsed.Remove("foldout_options");
			parsed.Remove("foldout_gizmos");
			
			retJson.Add("parsed", parsed);

			return new List<SerializerResult>{new() {
				NNAType = VRC_Physbone_VRCJsonProcessor._Type,
				Origin = UnityObject,
				JsonResult = retJson.ToString(Newtonsoft.Json.Formatting.None),
				JsonComponentId = Context.GetId(UnityObject),
				JsonTargetNode = physbone.rootTransform ? physbone.rootTransform.name : physbone.transform.name,
				IsJsonComplete = true,
				Confidence = SerializerResultConfidenceLevel.MANUAL,
			}};
		}
	}

	[InitializeOnLoad]
	public class Register_VRC_Physbone_VRC
	{
		static Register_VRC_Physbone_VRC()
		{
			NNARegistry.RegisterJsonProcessor(new VRC_Physbone_VRCJsonProcessor(), DetectorVRC.NNA_VRC_AVATAR_CONTEXT);
			NNAExportRegistry.RegisterSerializer(new VRC_Physbone_VRCSerializer());
		}
	}
}

#endif
#endif