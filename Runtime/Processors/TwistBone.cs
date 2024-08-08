
using UnityEngine;
using UnityEngine.Animations;

namespace nna.processors
{
	public class TwistBone : IProcessor
	{
		public string Type => "twist-bone";

		public void Process(GameObject Root, GameObject NNANode, out bool Delete)
		{
			Delete = false;

			var converted = NNANode.AddComponent<RotationConstraint>();

			var properties = ParseUtil.ParseNNADefinition(Root, NNANode);
			
			var weight = properties.ContainsKey("weight") ? (float)properties["weight"].Value : 0.5f;
			Transform target = null;
			if(properties.ContainsKey("target"))
			{
				var targetValue = properties["target"].Value;
				if(targetValue is Transform) target = (Transform)targetValue;
				else if(targetValue is GameObject) target = (targetValue as GameObject).transform;
				else if(targetValue is Component) target = (targetValue as Component).transform;
			}
			else
			{
				target = NNANode.transform.parent.parent;
			}

			converted.weight = weight;
			converted.rotationAxis = Axis.Y;

			var source = new ConstraintSource {
				weight = 1,
				sourceTransform = target,
			};
			converted.AddSource(source);

			Quaternion rotationOffset = Quaternion.Inverse(source.sourceTransform.rotation) * converted.transform.rotation;
			converted.rotationOffset = rotationOffset.eulerAngles;

			converted.locked = true;
			converted.constraintActive = true;
		}
	}
}