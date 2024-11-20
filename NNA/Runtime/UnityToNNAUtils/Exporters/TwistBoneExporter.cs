
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using nna.processors;
using UnityEngine.Animations;

namespace nna.UnityToNNAUtils
{
	public class TwistBoneExporter : INNAJsonSerializer
	{
		public static readonly System.Type _Target = typeof(RotationConstraint);
		public System.Type Target => _Target;

		public List<JsonSerializerResult>  Serialize(UnityEngine.Object UnityObject)
		{
			if(UnityObject is RotationConstraint c && c.rotationAxis == Axis.Y && c.sourceCount == 1)
			{
				var ret = new JObject {{"t", TwistBoneJsonProcessor._Type}};
				if(c.GetSource(0).sourceTransform != c.transform.parent?.parent) ret.Add("s", c.GetSource(0).sourceTransform.name);
				if(c.weight != 0.5f) ret.Add("w", c.weight);
				return new List<JsonSerializerResult> {new(){Type=TwistBoneJsonProcessor._Type, JsonResult=ret.ToString(Newtonsoft.Json.Formatting.None)}};
			}
			else return null;
		}
	}
}
