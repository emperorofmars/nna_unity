#if UNITY_EDITOR

using System.Linq;
using System.Threading.Tasks;
using nna.ava.common;
using nna.processors;
using UnityEditor;
using UnityEngine;
using VRM;

namespace nna.ava.univrm0
{
	public class UNIVRM0EyeTrackingBones : IGlobalProcessor
	{
		public const string _Type = "ava.eyetracking";
		public string Type => _Type;

		public void Process(NNAContext Context)
		{
			var explicitAvatar = Context.GetComponent(Context.Root.transform, "ava.avatar");
			if(explicitAvatar != null && explicitAvatar.ContainsKey("auto") && !(bool)explicitAvatar["auto"]) return;
			
			var Json = Context.GetComponentOrDefault(Context.Root.transform, _Type);

			Context.AddTask(new Task(() => {
				var avatar = Context.Root.GetComponent<VRMMeta>();
				var animator = Context.Root.GetComponent<Animator>();
				
				// set eyebones if human
				if(animator.isHuman)
				{
					(var limitsLeft, var limitsRight) = EyeTrackingBoneLimits.ParseGlobal(Context);
					Setup(Context, avatar, animator, limitsLeft, limitsRight);
				}
			}));
		}

		public static void Setup(NNAContext Context, VRMMeta Avatar, Animator AnimatorHumanoid, Vector4 LimitsLeft, Vector4 LimitsRight)
		{
			var humanEyeL = AnimatorHumanoid.avatar.humanDescription.human.FirstOrDefault(hb => hb.humanName == HumanBodyBones.LeftEye.ToString());
			var humanEyeR = AnimatorHumanoid.avatar.humanDescription.human.FirstOrDefault(hb => hb.humanName == HumanBodyBones.RightEye.ToString());
			
			if(humanEyeL.boneName != null && humanEyeR.boneName != null)
			{
				var eyeL = Context.Root.GetComponentsInChildren<Transform>().FirstOrDefault(t => t.name == humanEyeL.boneName);
				var eyeR = Context.Root.GetComponentsInChildren<Transform>().FirstOrDefault(t => t.name == humanEyeR.boneName);

				var vrmLookat = Context.Root.AddComponent<VRMLookAtBoneApplyer>();
				vrmLookat.LeftEye.Transform = eyeL;
				vrmLookat.RightEye.Transform = eyeR;

				// This implementation could be wrong. The VRM documentation on this is effectively non existent: https://vrm.dev/en/univrm/lookat/lookat_bone/
				vrmLookat.VerticalUp.CurveYRangeDegree = LimitsLeft.x;
				vrmLookat.VerticalDown.CurveYRangeDegree = LimitsLeft.y;
				vrmLookat.HorizontalInner.CurveYRangeDegree = LimitsLeft.z;
				vrmLookat.HorizontalOuter.CurveYRangeDegree = LimitsLeft.w;

				return;
			}
		}
	}

	[InitializeOnLoad]
	public class Register_UNIVRM0EyeTrackingBones
	{
		static Register_UNIVRM0EyeTrackingBones()
		{
			NNARegistry.RegisterGlobalProcessor(new UNIVRM0EyeTrackingBones(), DetectorUNIVRM0.NNA_UNIVRM0_CONTEXT, true);
		}
	}
}

#endif