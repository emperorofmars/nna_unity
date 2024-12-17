using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using nna.processors;
using UnityEngine;
using UnityEngine.Animations;

namespace nna.UnityToNNAUtils
{
	public class NNA_HumanoidLimits_Serializer : INNASerializer
	{
		public static readonly System.Type _Target = typeof(Animator);
		public System.Type Target => _Target;

		public List<SerializerResult> Serialize(NNASerializerContext Context, UnityEngine.Object UnityObject)
		{
			if(UnityObject is Animator animator && animator.isHuman)
			{
				var ret = new List<SerializerResult>();

				foreach(var bone in animator.avatar.humanDescription.human)
				{
					if(bone.limit.useDefaultValues == false)
					{
						Debug.Log(bone.humanName);
						Debug.Log(bone.limit.useDefaultValues);
						Debug.Log(bone.limit.min);
						Debug.Log(bone.limit.max);

						var nameDefinition = "$HuLim";

						if(bone.limit.min.x != 0)
						{
							nameDefinition += "T" + (float)System.Math.Round(bone.limit.min.x, 2) + "," + (float)System.Math.Round(bone.limit.max.x, 2);
						}
						if(bone.limit.min.y != 0)
						{
							nameDefinition += "S" + (float)System.Math.Round(bone.limit.min.y, 2) + "," + (float)System.Math.Round(bone.limit.max.y, 2);
						}
						if(bone.limit.min.z != 0)
						{
							nameDefinition += "P" + (float)System.Math.Round(bone.limit.min.z, 2) + "," + (float)System.Math.Round(bone.limit.max.z, 2);
						}

						ret.Add(new SerializerResult(){
							NNAType = NNA_Twist_JsonProcessor._Type,
							Origin = UnityObject,
							//JsonResult = retJson.ToString(Newtonsoft.Json.Formatting.None),
							//JsonComponentId = Context.GetId(UnityObject),
							//JsonTargetNode = animator.transform.name,
							//IsJsonComplete = true,
							NameResult = nameDefinition,
							NameTargetNode = bone.boneName,
							IsNameComplete = true,
							Confidence = SerializerResultConfidenceLevel.MANUAL,
						});
					}
				}
				return ret;
			}
			else return null;
		}
	}
}
