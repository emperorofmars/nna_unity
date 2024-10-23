#if NNA_AVA_UNIVRM0_FOUND
#if UNITY_EDITOR

using System.Linq;
using Newtonsoft.Json.Linq;
using UnityEngine;
using VRM;

namespace nna.ava.univrm0
{
	public class UNIVRM0EyeTrackingBones : IAVAFeatureUNIVRM0
	{
		public const string _Type = "ava.eyetracking";
		public string Type => _Type;

		public bool AutoDetect(NNAContext Context, Component UnityComponent, JObject Json)
		{
			var avatar = Context.Root.GetComponent<VRMMeta>();
			var animator = Context.Root.GetComponent<Animator>();
			
			// set eyebones if human
			if(animator.isHuman)
			{
				var humanEyeL = animator.avatar.humanDescription.human.FirstOrDefault(hb => hb.humanName == HumanBodyBones.LeftEye.ToString());
				var humanEyeR = animator.avatar.humanDescription.human.FirstOrDefault(hb => hb.humanName == HumanBodyBones.RightEye.ToString());
				
				if(humanEyeL.boneName != null && humanEyeR.boneName != null)
				{
					var eyeL = Context.Root.GetComponentsInChildren<Transform>().FirstOrDefault(t => t.name == humanEyeL.boneName);
					var eyeR = Context.Root.GetComponentsInChildren<Transform>().FirstOrDefault(t => t.name == humanEyeR.boneName);

					var vrmLookat = Context.Root.AddComponent<VRMLookAtBoneApplyer>();
					vrmLookat.LeftEye.Transform = eyeL;
					vrmLookat.RightEye.Transform = eyeR;
			
					// vrmLookat.VerticalDown = // i cant be arsed to look at how this nonesense is defined
					// etc

					return true;
				}
			}

			return false;
		}
	}
}

#endif
#endif