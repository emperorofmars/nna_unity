
using System.Text.RegularExpressions;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.Animations;

namespace nna.processors
{
	public class NNA_Twist_JsonProcessor : IJsonProcessor
	{
		public const string _Type = "nna.twist";
		public string Type => _Type;
		public uint Order => 0;

		public void Process(NNAContext Context, Transform Node, JObject Json)
		{
			var sourceWeight = (float)ParseUtil.GetMulkikeyOrDefault(Json, new JValue(0.5f), "w", "weight");
			Transform sourceNode = ParseUtil.HasMulkikey(Json, "s", "source")
					? ((string)ParseUtil.GetMulkikey(Json, "s", "source")).Contains('&') ? ParseUtil.FindNode(Context.Root.transform, (string)ParseUtil.GetMulkikey(Json, "s", "source"), '&') : ParseUtil.FindNodeNearby(Node, (string)ParseUtil.GetMulkikey(Json, "s", "source"))
					: Node.transform.parent.parent;
			var converted = CreateTwistBoneConstraint.CreateConstraint(Node, sourceNode, sourceWeight);

			if(Json.ContainsKey("id")) converted.name = "$nna:" + (string)Json["id"];
		}
	}

	
	public class NNA_Twist_NameProcessor : INameProcessor
	{
		public const string _Type = "nna.twist";
		public string Type => _Type;
		public uint Order => 0;
		
		public const string Match = @"(?i)twist(?<source_node_path>[a-zA-Z][a-zA-Z0-9._\-|:\s]*(\&[a-zA-Z][a-zA-Z0-9._\-|:\s]*)*)?,?(?<weight>[0-9]*[.][0-9]+)?(?<side>(([._\-|:][lr])|[._\-|:\s]?(right|left))$)?$";

		public int CanProcessName(NNAContext Context, string Name)
		{
			var match = Regex.Match(Name, Match);
			return match.Success ? match.Index : -1;
		}

		public void Process(NNAContext Context, Transform Node, string Name)
		{
			(var sourceNodeName, var sourceWeight) = ParseName(Node, Name);
			Transform sourceNode = sourceNodeName != null
					? sourceNodeName.Contains('&') ? ParseUtil.FindNode(Context.Root.transform, sourceNodeName, '&') : ParseUtil.FindNodeNearby(Node, sourceNodeName)
					: Node.transform.parent.parent;
			CreateTwistBoneConstraint.CreateConstraint(Node, sourceNode, sourceWeight);
		}
		
		public static (string SourceName, float Weight) ParseName(Transform Node, string Name)
		{
			var match = Regex.Match(Name, Match);
			var sourcePath = match.Groups["source_node_path"].Success ? match.Groups["source_node_path"].Value : null;
			var weight = match.Groups["weight"].Success ? float.Parse(match.Groups["weight"].Value) : 0.5f;
			return (sourcePath, weight);
		}
	}

	public static class CreateTwistBoneConstraint
	{
		public static RotationConstraint CreateConstraint(Transform Node, Transform Source, float Weight)
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

			return converted;
		}
	}
}