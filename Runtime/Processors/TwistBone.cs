
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

namespace nna.processors
{
	public class TwistBone : IProcessor
	{
		public static readonly string _Type = "twist-bone";
		public string Type => _Type;

		public void Process(GameObject Root, GameObject NNANode, Dictionary<string, NNAValue> Properties)
		{
			var converted = NNANode.AddComponent<RotationConstraint>();
			
			var weight = Properties.ContainsKey("weight") ? (float)Properties["weight"].Value : 0.5f;
			Transform target = null;
			if(Properties.ContainsKey("target"))
			{
				var targetValue = Properties["target"].Value;
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