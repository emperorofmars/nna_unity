
using System.Collections.Generic;
using nna.processors;
using UnityEngine.Animations;

namespace nna.UnityToNNAUtils
{
	public class TwistBoneExporter : INNAJsonSerializer
	{
		public static readonly System.Type _Target = typeof(RotationConstraint);
		public System.Type Target => _Target;

		public List<(string, string)>  Serialize(UnityEngine.Object UnityObject)
		{
			if(UnityObject is RotationConstraint c && c.rotationAxis == Axis.Y && c.sourceCount == 1)
			{

				var def = $"{{\"t\":\"{TwistBoneJsonProcessor._Type}\"";

				if(c.GetSource(0).sourceTransform != c.transform.parent?.parent) def += ",\"s\":\"" + c.GetSource(0).sourceTransform.name + "\"";
				if(c.weight != 0.5f) def += ",\"w\":" + c.weight;

				def += "}";

				return new List<(string, string)> {(TwistBoneJsonProcessor._Type, def)};
			}
			else return null;
		}
	}
}
