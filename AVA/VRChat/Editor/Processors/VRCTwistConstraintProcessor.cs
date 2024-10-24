#if AVA_VRCSDK3_FOUND
#if UNITY_EDITOR

using System.Text.RegularExpressions;
using Newtonsoft.Json.Linq;
using nna.processors;
using UnityEditor;
using UnityEngine;
using VRC.SDK3.Dynamics.Constraint.Components;

namespace nna.ava.vrchat
{
	public class VRCTwistConstraintJsonProcessor : IJsonProcessor
	{
		public const string _Type = TwistBoneJsonProcessor._Type;
		public string Type => _Type;

		public void ProcessJson(NNAContext Context, Transform Node, JObject Json)
		{
			var sourceWeight = (float)ParseUtil.GetMulkikeyOrDefault(Json, new JValue(0.5f), "w", "weight");
			Transform sourceNode = ParseUtil.HasMulkikey(Json, "s", "source") ? ParseUtil.FindNode(Node, (string)ParseUtil.GetMulkikey(Json, "s", "source")) : Node.transform.parent.parent;
			CreateVRCTwistBoneConstraint.CreateConstraint(Node, sourceNode, sourceWeight);
		}
	}
	public class VRCTwistConstraintNameProcessor : INameProcessor
	{
		public const string _Type = TwistBoneNameProcessor._Type;
		public string Type => _Type;

		public bool CanProcessName(NNAContext Context, string Name)
		{
			return Regex.IsMatch(Name, TwistBoneNameProcessor.MatchName);
		}

		public void ProcessName(NNAContext Context, Transform Node, string Name)
		{
			(var sourceNodeName, var sourceWeight) = TwistBoneNameProcessor.ParseName(Node, Name);
			Transform sourceNode = sourceNodeName != null ? ParseUtil.FindNode(Node, sourceNodeName) : Node.transform.parent.parent;
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

	[InitializeOnLoad]
	public class Register_VRCTwistConstraintProcessor
	{
		static Register_VRCTwistConstraintProcessor()
		{
			NNARegistry.RegisterJsonProcessor(new VRCTwistConstraintJsonProcessor(), VRCTwistConstraintJsonProcessor._Type, DetectorVRC.NNA_VRC_AVATAR_CONTEXT);
			NNARegistry.RegisterNameProcessor(new VRCTwistConstraintNameProcessor(), VRCTwistConstraintNameProcessor._Type, DetectorVRC.NNA_VRC_AVATAR_CONTEXT);
		}
	}
}

#endif
#endif