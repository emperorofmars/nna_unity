#if UNITY_EDITOR
#if NNA_AVA_VRCSDK3_FOUND

using System.Collections.Generic;
using System.Text.RegularExpressions;
using Newtonsoft.Json.Linq;
using nna.processors;
using nna.UnityToNNAUtils;
using UnityEditor;
using UnityEngine;
using VRC.SDK3.Dynamics.Constraint.Components;

namespace nna.ava.vrchat
{
	public class NNA_Twist_VRCJsonProcessor : IJsonProcessor
	{
		public const string _Type = NNA_Twist_JsonProcessor._Type;
		public string Type => _Type;
		public uint Order => 0;
		public int Priority => int.MaxValue;

		public void Process(NNAContext Context, Transform Node, JObject Json)
		{
			var sourceWeight = (float)ParseUtil.GetMulkikeyOrDefault(Json, new JValue(0.5f), "w", "weight");
			Transform sourceNode = ParseUtil.HasMulkikey(Json, "s", "source")
					? ((string)ParseUtil.GetMulkikey(Json, "s", "source")).Contains('&') ? ParseUtil.FindNode(Context.Root.transform, (string)ParseUtil.GetMulkikey(Json, "s", "source"), '&') : ParseUtil.FindNodeNearby(Node, (string)ParseUtil.GetMulkikey(Json, "s", "source"))
					: Node.transform.parent.parent;
			var converted = CreateVRCTwistBoneConstraint.CreateConstraint(Node, sourceNode, sourceWeight);
			if(Json.ContainsKey("id")) Context.AddResultById((string)Json["id"], converted);
		}
	}

	public class NNA_Twist_VRCNameProcessor : INameProcessor
	{
		public const string _Type = NNA_Twist_JsonProcessor._Type;
		public string Type => _Type;
		public uint Order => 0;

		public int CanProcessName(NNAContext Context, string Name)
		{
			var match = Regex.Match(Name, NNA_Twist_NameProcessor.Match);
			return match.Success ? match.Index : -1;
		}

		public void Process(NNAContext Context, Transform Node, string Name)
		{
			(var sourceNodeName, var sourceWeight, var startIndex) = NNA_Twist_NameProcessor.ParseName(Node, Name);
			Transform sourceNode = sourceNodeName != null
					? sourceNodeName.Contains('&') ? ParseUtil.FindNode(Context.Root.transform, sourceNodeName, '&') : ParseUtil.FindNodeNearby(Node, sourceNodeName)
					: Node.transform.parent.parent;

			var converted = CreateVRCTwistBoneConstraint.CreateConstraint(Node, sourceNode, sourceWeight);
			if(ParseUtil.GetNameComponentId(Node.name) is var componentId && componentId != null) Context.AddResultById(componentId, converted);
		}
	}

	public static class CreateVRCTwistBoneConstraint
	{
		public static VRCRotationConstraint CreateConstraint(Transform Node, Transform Source, float Weight)
		{
			var converted = Node.gameObject.AddComponent<VRCRotationConstraint>();

			converted.GlobalWeight = Weight;

			converted.AffectsRotationX = false;
			converted.AffectsRotationY = true;
			converted.AffectsRotationZ = false;

			converted.Sources.Add(new VRC.Dynamics.VRCConstraintSource(Source, 1, Vector3.zero, Vector3.zero));

			converted.RotationOffset = (Quaternion.Inverse(Source.rotation) * converted.transform.rotation).eulerAngles;

			converted.Locked = true;
			converted.IsActive = true;

			return converted;
		}
	}

	public class NNA_Twist_VRCSerializer : INNASerializer
	{
		public static readonly System.Type _Target = typeof(VRCRotationConstraint);
		public System.Type Target => _Target;

		public List<SerializerResult> Serialize(NNASerializerContext Context, UnityEngine.Object UnityObject)
		{
			if(UnityObject is VRCRotationConstraint c && c.AffectsRotationY && !c.AffectsRotationX && !c.AffectsRotationZ && c.Sources.Count == 1)
			{
				var retJson = new JObject {{"t", NNA_Twist_JsonProcessor._Type}};
				if(UnityObject.name.StartsWith("$nna:")) retJson.Add("id", UnityObject.name[5..]);
				var retName = "Twist";
				bool sourceIsSet = false;
				if(c.Sources[0].SourceTransform != c.transform.parent?.parent)
				{
					retJson.Add("s", c.Sources[0].SourceTransform.name);
					retName += c.Sources[0].SourceTransform.name;
					sourceIsSet = true;
				}
				if(c.GlobalWeight != 0.5f)
				{
					retJson.Add("w", c.GlobalWeight);
					if(sourceIsSet) retName += ",";
					retName +=System.Math.Round(c.GlobalWeight, 2);
				}
				return new List<SerializerResult> {new(){
					NNAType = NNA_Twist_JsonProcessor._Type,
					Origin = UnityObject,
					JsonResult = retJson.ToString(Newtonsoft.Json.Formatting.None),
					JsonComponentId = Context.GetId(UnityObject),
					JsonTargetNode = c.transform.name,
					IsJsonComplete = true,
					NameResult = retName,
					NameTargetNode = c.transform.name,
					IsNameComplete = true,
					Confidence = SerializerResultConfidenceLevel.MANUAL,
				}};
			}
			else return null;
		}
	}

	[InitializeOnLoad]
	public class Register_VRCTwistConstraint
	{
		static Register_VRCTwistConstraint()
		{
			NNARegistry.RegisterJsonProcessor(new NNA_Twist_VRCJsonProcessor(), DetectorVRC.NNA_VRC_AVATAR_CONTEXT);
			NNARegistry.RegisterNameProcessor(new NNA_Twist_VRCNameProcessor(), DetectorVRC.NNA_VRC_AVATAR_CONTEXT);
			NNAExportRegistry.RegisterSerializer(new NNA_Twist_VRCSerializer());
		}
	}
}

#endif
#endif
