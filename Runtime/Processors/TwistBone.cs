
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.Animations;

namespace nna.processors
{
	public class TwistBone : IProcessor
	{
		public static readonly string _Type = "c-twist";
		public string Type => _Type;

		public void Process(NNAContext Context, GameObject NNANode, JObject Json)
		{
			var converted = NNANode.AddComponent<RotationConstraint>();
			
			var weight = (float)ParseUtil.GetMulkikeyOrDefault(Json, new JValue(0.5f), "w", "weight");
			GameObject target;
			if(ParseUtil.HasMulkikey(Json, "tp", "target"))
			{
				target = ParseUtil.ResolvePath(Context.Root, NNANode, (string)ParseUtil.GetMulkikey(Json, "tp", "target"));
			}
			else
			{
				target = NNANode.transform.parent.parent.gameObject;
			}

			converted.weight = weight;
			converted.rotationAxis = Axis.Y;

			var source = new ConstraintSource {
				weight = 1,
				sourceTransform = target.transform,
			};
			converted.AddSource(source);

			Quaternion rotationOffset = Quaternion.Inverse(source.sourceTransform.rotation) * converted.transform.rotation;
			converted.rotationOffset = rotationOffset.eulerAngles;

			converted.locked = true;
			converted.constraintActive = true;
		}
	}
}