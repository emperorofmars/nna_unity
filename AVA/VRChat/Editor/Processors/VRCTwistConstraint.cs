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
	public class VRCTwistConstraintJsonProcessor : IJsonProcessor
	{
		public const string _Type = TwistBoneJsonProcessor._Type;
		public string Type => _Type;
		public uint Order => 0;

		public void Process(NNAContext Context, Transform Node, JObject Json)
		{
			var sourceWeight = (float)ParseUtil.GetMulkikeyOrDefault(Json, new JValue(0.5f), "w", "weight");
			Transform sourceNode = ParseUtil.HasMulkikey(Json, "s", "source")
					? ((string)ParseUtil.GetMulkikey(Json, "s", "source")).Contains('&') ? ParseUtil.FindNode(Context.Root.transform, (string)ParseUtil.GetMulkikey(Json, "s", "source"), '&') : ParseUtil.FindNodeNearby(Node, (string)ParseUtil.GetMulkikey(Json, "s", "source"))
					: Node.transform.parent.parent;
			CreateVRCTwistBoneConstraint.CreateConstraint(Node, sourceNode, sourceWeight);
		}
	}
	
	public class VRCTwistConstraintNameProcessor : INameProcessor
	{
		public const string _Type = TwistBoneNameProcessor._Type;
		public string Type => _Type;
		public uint Order => 0;

		public bool CanProcessName(NNAContext Context, string Name)
		{
			return Regex.IsMatch(Name, TwistBoneNameProcessor.Match);
		}

		public void Process(NNAContext Context, Transform Node, string Name)
		{
			(var sourceNodeName, var sourceWeight) = TwistBoneNameProcessor.ParseName(Node, Name);
			Transform sourceNode = sourceNodeName != null
					? sourceNodeName.Contains('&') ? ParseUtil.FindNode(Context.Root.transform, sourceNodeName, '&') : ParseUtil.FindNodeNearby(Node, sourceNodeName)
					: Node.transform.parent.parent;

			CreateVRCTwistBoneConstraint.CreateConstraint(Node, sourceNode, sourceWeight);
		}
	}

	public static class CreateVRCTwistBoneConstraint
	{
		public static void CreateConstraint(Transform Node, Transform Source, float Weight)
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
		}
	}

	public class VRCTwistConstraintExporter : INNAJsonSerializer
	{
		public static readonly System.Type _Target = typeof(VRCRotationConstraint);
		public System.Type Target => _Target;

		public List<JsonSerializerResult>  Serialize(UnityEngine.Object UnityObject)
		{
			if(UnityObject is VRCRotationConstraint c && c.AffectsRotationY && !c.AffectsRotationX && !c.AffectsRotationZ && c.Sources.Count == 1)
			{
				var ret = new JObject {{"t", TwistBoneJsonProcessor._Type}};
				if(c.Sources[0].SourceTransform != c.transform.parent?.parent) ret.Add("s", c.Sources[0].SourceTransform.name);
				if(c.GlobalWeight != 0.5f) ret.Add("w", c.GlobalWeight);
				return new List<JsonSerializerResult> {new(){Type=TwistBoneJsonProcessor._Type, JsonResult=ret.ToString(Newtonsoft.Json.Formatting.None)}};
			}
			else return null;
		}
	}

	[InitializeOnLoad]
	public class Register_VRCTwistConstraint
	{
		static Register_VRCTwistConstraint()
		{
			NNARegistry.RegisterJsonProcessor(new VRCTwistConstraintJsonProcessor(), DetectorVRC.NNA_VRC_AVATAR_CONTEXT);
			NNARegistry.RegisterNameProcessor(new VRCTwistConstraintNameProcessor(), DetectorVRC.NNA_VRC_AVATAR_CONTEXT);
			NNAJsonExportRegistry.RegisterSerializer(new VRCTwistConstraintExporter());
		}
	}
}

#endif
#endif