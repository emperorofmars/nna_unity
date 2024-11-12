
using System;
using System.Collections.Generic;
using UnityEngine.Animations;

namespace nna.UnityToNNAUtils
{
	public class TwistBoneExporter : INNAJsonSerializer
	{
		public static readonly Type _Target = typeof(RotationConstraint);
		public Type Target => _Target;

		public List<string> Serialize(UnityEngine.Object UnityObject)
		{
			if(UnityObject is RotationConstraint c && c.rotationAxis == Axis.Y && c.sourceCount == 1)
			{

				var def = "{\"t\":\"c-twist\"";

				if(c.GetSource(0).sourceTransform != c.transform.parent?.parent) def += ",\"s\":\"" + c.GetSource(0).sourceTransform.name + "\"";
				if(c.weight != 0.5f) def += ",\"w\":" + c.weight;

				def += "}";

				return new List<string>(){def};
			}
			else return null;
		}
	}
}
