
using System.Text.RegularExpressions;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.Animations;

namespace nna.processors
{
	public class TwistBoneJsonProcessor : IJsonProcessor
	{
		public const string _Type = "c-twist";
		public string Type => _Type;

		public void ProcessJson(NNAContext Context, Transform Node, JObject Json)
		{
			var sourceWeight = (float)ParseUtil.GetMulkikeyOrDefault(Json, new JValue(0.5f), "w", "weight");
			Transform sourceNode;
			if(ParseUtil.HasMulkikey(Json, "tp", "target"))
			{
				sourceNode = ParseUtil.ResolvePath(Context.Root.transform, Node.transform, (string)ParseUtil.GetMulkikey(Json, "tp", "target"));
			}
			else
			{
				sourceNode = Node.transform.parent.parent;
			}
			CreateTwistBoneConstraint.CreateConstraint(Node, sourceNode, sourceWeight);
		}
	}

	
	public class TwistBoneNameProcessor : INameProcessor
	{
		public const string _Type = "c-twist";
		public string Type => _Type;

		public const string MatchSourceNodeName = @"^([a-zA-Z][a-zA-Z._\-|:]*)";
		public const string MatchFloat = @"(?i)([0-9]*[.][0-9]+)";
		public const string MatchLR = @"(?i)([._\-|:][lr])$";
		public const string MatchName = @"(?i)(twist)([a-zA-Z][a-zA-Z._\-|:]*)?([0-9]*[.][0-9]+)?([._\-|:][lr])?$";

		public bool CanProcessName(NNAContext Context, string Name)
		{
			return Regex.IsMatch(Name, MatchName);
		}

		public void ProcessName(NNAContext Context, Transform Node, string Name)
		{
			var match = Regex.Match(Name, MatchName);

			var matchWeight = Regex.Match(match.Value, MatchFloat);
			var sourceWeight = matchWeight.Success && matchWeight.Length > 0 ? float.Parse(matchWeight.Value) : 0.5f;

			var sourceNodeNameMatch = Regex.Match(match.Value[5 .. ], MatchSourceNodeName);
			string sourceNodeName = sourceNodeNameMatch.Success ? sourceNodeNameMatch.Value : null;

			Transform sourceNode = sourceNodeName != null ? ParseUtil.FindNode(Node, sourceNodeName) : Node.transform.parent.parent;
			CreateTwistBoneConstraint.CreateConstraint(Node, sourceNode, sourceWeight);
		}
	}

	public static class CreateTwistBoneConstraint
	{
		public static void CreateConstraint(Transform Node, Transform Source, float Weight)
		{
			var converted = Node.gameObject.AddComponent<RotationConstraint>();

			converted.weight = Weight;
			converted.rotationAxis = Axis.Y;

			var source = new ConstraintSource {
				weight = 1,
				sourceTransform = Source,
			};
			converted.AddSource(source);

			Quaternion rotationOffset = Quaternion.Inverse(source.sourceTransform.rotation) * converted.transform.rotation;
			converted.rotationOffset = rotationOffset.eulerAngles;

			converted.locked = true;
			converted.constraintActive = true;
		}
	}
}