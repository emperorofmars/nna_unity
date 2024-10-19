
using System.Text.RegularExpressions;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.Animations;

namespace nna.processors
{
	public class TwistBone : IProcessor
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
			CreateConstraint(Node, sourceNode, sourceWeight);
		}

		public bool CanProcessName(NNAContext Context, Transform Node)
		{
			return Regex.IsMatch(Node.name, @"(?i)(twist)([\w._-|:]*)([\d]*\.?[\d]*)$");
		}

		public void ProcessName(NNAContext Context, Transform Node)
		{
			var match = Regex.Match(Node.name, @"(?i)(twist)([\w._-|:]*)([\d]*\.?[\d]*)$");

			var matchWeight = Regex.Match(match.Value, @"[\d]*\.?[\d]*$");
			var sourceWeight = matchWeight.Success && matchWeight.Length > 0 ? float.Parse(matchWeight.Value) : 0.5f;

			var nameLen = match.Length - 5 - matchWeight.Length;
			var sourceNodeName = nameLen > 0 ? match.Value[5 .. (nameLen + 5)] : null;

			Transform sourceNode;
			if(sourceNodeName != null)
			{
				sourceNode = ParseUtil.FindNode(Node, sourceNodeName);
			}
			else
			{
				sourceNode = Node.transform.parent.parent;
			}
			CreateConstraint(Node, sourceNode, sourceWeight);
		}

		private void CreateConstraint(Transform Node, Transform Source, float Weight)
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