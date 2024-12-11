#if UNITY_EDITOR
#if NNA_AVA_UNIVRM0_FOUND

using System.Linq;
using System.Text.RegularExpressions;
using Newtonsoft.Json.Linq;
using nna.ava.common;
using nna.processors;
using UnityEditor;
using UnityEngine;
using VRM;

namespace nna.ava.univrm0
{
	public class AVA_EyeTrackingBoneLimits_UNIVRM0_JsonProcessor : IJsonProcessor
	{
		public const string _Type = EyeTrackingBoneLimits._Type;
		public string Type => _Type;
		public uint Order => 0;

		public int Priority => int.MaxValue;

		public void Process(NNAContext Context, Transform Node, JObject Json)
		{
			EyeTrackingBoneLimits.ParseJsonToMessage(Context, Json);
		}
	}

	public class AVA_EyeTrackingBoneLimits_UNIVRM0_NameProcessor : INameProcessor
	{
		public const string _Type = EyeTrackingBoneLimits._Type;
		public string Type => _Type;
		public uint Order => 0;

		public int CanProcessName(NNAContext Context, string NameDefinition)
		{
			var match = Regex.Match(NameDefinition, EyeTrackingBoneLimits.MatchExpression);
			return match.Success ? match.Index : -1;
		}

		public void Process(NNAContext Context, Transform Node, string NameDefinition)
		{
			EyeTrackingBoneLimits.ParseNameDefinitionToMessage(Context, NameDefinition);
		}
	}

	public class AVAEyeTrackingBoneLimits_UNIVRM0_Processor : IGlobalProcessor
	{
		public const string _Type = EyeTrackingBoneLimits._Type;
		public string Type => _Type;
		public uint Order => AVA_Avatar_UNIVRM0Processor._Order + 1;

		public void Process(NNAContext Context)
		{
			var avatarJson = Context.GetOnlyJsonComponentByType("ava.avatar").Component;
			if(avatarJson != null
				&& avatarJson.ContainsKey("auto")
				&& !(bool)avatarJson["auto"]
				&& !EyeTrackingBoneLimits.LimitsExplicitelyDefined(Context)
			) return; // No automapping otherwise

			var avatar = Context.Root.GetComponent<VRMMeta>();
			if(!avatar) throw new NNAException("No Avatar Component created!", _Type);
			var animator = Context.Root.GetComponent<Animator>();
			if(!animator) throw new NNAException("No Animator found!", _Type);

			// set eyebones if human
			if(animator.isHuman)
			{
				(var limitsLeft, var limitsRight) = EyeTrackingBoneLimits.GetLimitsOrDefault(Context);
				Setup(Context, avatar, animator, limitsLeft, limitsRight);
			}
			else
			{
				throw new NNAException("Animator is not humanoid!", _Type);
			}
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
	public class Register_AVAEyeTrackingBoneLimits_UNIVRM0
	{
		static Register_AVAEyeTrackingBoneLimits_UNIVRM0()
		{
			NNARegistry.RegisterGlobalProcessor(new AVAEyeTrackingBoneLimits_UNIVRM0_Processor(), DetectorUNIVRM0.NNA_UNIVRM0_CONTEXT, false);
			NNARegistry.RegisterJsonProcessor(new AVA_EyeTrackingBoneLimits_UNIVRM0_JsonProcessor(), DetectorUNIVRM0.NNA_UNIVRM0_CONTEXT);
			NNARegistry.RegisterNameProcessor(new AVA_EyeTrackingBoneLimits_UNIVRM0_NameProcessor(), DetectorUNIVRM0.NNA_UNIVRM0_CONTEXT);
		}
	}
}

#endif
#endif
