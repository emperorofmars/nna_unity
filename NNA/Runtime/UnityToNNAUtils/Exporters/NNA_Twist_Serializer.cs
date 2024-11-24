
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using nna.processors;
using UnityEngine.Animations;

namespace nna.UnityToNNAUtils
{
	public class NNA_Twist_Serializer : INNASerializer
	{
		public static readonly System.Type _Target = typeof(RotationConstraint);
		public System.Type Target => _Target;

		public List<SerializerResult> Serialize(NNASerializerContext Context, UnityEngine.Object UnityObject)
		{
			if(UnityObject is RotationConstraint c && c.rotationAxis == Axis.Y && c.sourceCount == 1)
			{
				var retJson = new JObject {{"t", NNA_Twist_JsonProcessor._Type}};
				if(UnityObject.name.StartsWith("$nna:")) retJson.Add("id", UnityObject.name[5..]);

				var retName = "Twist";
				bool sourceIsSet = false;
				if(c.GetSource(0).sourceTransform != c.transform.parent?.parent)
				{
					retJson.Add("s", c.GetSource(0).sourceTransform.name);
					retName += c.GetSource(0).sourceTransform.name;
					sourceIsSet = true;
				}
				if(c.weight != 0.5f)
				{
					retJson.Add("w", c.weight);
					if(sourceIsSet) retName += ",";
					retName +=System.Math.Round(c.weight, 2);
				}

				return new List<SerializerResult> {new(){
					NNAType = NNA_Twist_JsonProcessor._Type,
					Origin = UnityObject,
					JsonResult = retJson.ToString(Newtonsoft.Json.Formatting.None),
					JsonComponentId = Context.GetId(UnityObject),
					JsonTargetNode = c.transform.name,
					IsJsonComplete = true,
					NameResult = retName,
					NameTargetNode = c.transform.name,
					IsNameComplete = true,
					Confidence = SerializerResultConfidenceLevel.MANUAL,
				}};
			}
			else return null;
		}
	}
}
