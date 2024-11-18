#if UNITY_EDITOR
#if NNA_AVA_VRCSDK3_FOUND

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using nna.ava.common;
using nna.processors;
using nna.UnityToNNAUtils;
using UnityEditor;
using UnityEngine;
using VRC.SDK3.Avatars.Components;

namespace nna.ava.vrchat
{
	public class VRCEyeTrackingBoneLimitsJson : IGlobalProcessor
	{
		public const string _Type = EyeTrackingBoneLimits._Type;
		public string Type => _Type;

		public void Process(NNAContext Context)
		{
			var explicitAvatar = Context.GetComponent(Context.Root.transform, "ava.avatar");
			if(explicitAvatar != null && explicitAvatar.ContainsKey("auto") && !(bool)explicitAvatar["auto"]) return;

			Context.AddTask(new Task(() => {
				var avatar = Context.Root.GetComponent<VRCAvatarDescriptor>();
				var animator = Context.Root.GetComponent<Animator>();

				// set eyebones if human
				if(animator.isHuman)
				{
					(var limitsLeft, var limitsRight) = EyeTrackingBoneLimits.ParseGlobal(Context);
					VRCEyeTrackingBoneLimits.Setup(Context, avatar, animator, limitsLeft, limitsRight);
				}
			}));
		}
	}

	public static class VRCEyeTrackingBoneLimits
	{
		public static void Setup(NNAContext Context, VRCAvatarDescriptor Avatar, Animator AnimatorHumanoid, Vector4 LimitsLeft, Vector4 LimitsRight)
		{
			var humanEyeL = AnimatorHumanoid.avatar.humanDescription.human.FirstOrDefault(hb => hb.humanName == HumanBodyBones.LeftEye.ToString());
			var humanEyeR = AnimatorHumanoid.avatar.humanDescription.human.FirstOrDefault(hb => hb.humanName == HumanBodyBones.RightEye.ToString());

			if(humanEyeL.boneName != null && humanEyeR.boneName != null)
			{
				Avatar.enableEyeLook = true;

				Avatar.customEyeLookSettings.leftEye = AnimatorHumanoid.avatarRoot.GetComponentsInChildren<Transform>().FirstOrDefault(t => t.name == humanEyeL.boneName);
				Avatar.customEyeLookSettings.rightEye = AnimatorHumanoid.avatarRoot.GetComponentsInChildren<Transform>().FirstOrDefault(t => t.name == humanEyeR.boneName);

				Avatar.customEyeLookSettings.eyesLookingUp = new VRCAvatarDescriptor.CustomEyeLookSettings.EyeRotations
						{left = Quaternion.Euler(-LimitsLeft.x, 0f, 0f), right = Quaternion.Euler(-LimitsRight.x, 0f, 0f), linked = Mathf.Approximately(LimitsLeft.x, LimitsRight.x)};
				Avatar.customEyeLookSettings.eyesLookingDown = new VRCAvatarDescriptor.CustomEyeLookSettings.EyeRotations
						{left = Quaternion.Euler(LimitsLeft.y, 0f, 0f), right = Quaternion.Euler(LimitsRight.y, 0f, 0f), linked = Mathf.Approximately(LimitsLeft.y, LimitsRight.y)};
				Avatar.customEyeLookSettings.eyesLookingLeft = new VRCAvatarDescriptor.CustomEyeLookSettings.EyeRotations
						{left = Quaternion.Euler(0f, -LimitsLeft.w, 0f), right = Quaternion.Euler(0f, -LimitsRight.z, 0f), linked = Mathf.Approximately(LimitsLeft.w, LimitsRight.z)};
				Avatar.customEyeLookSettings.eyesLookingRight = new VRCAvatarDescriptor.CustomEyeLookSettings.EyeRotations
						{left = Quaternion.Euler(0f, LimitsLeft.z, 0f), right = Quaternion.Euler(0f, LimitsRight.w, 0f), linked = Mathf.Approximately(LimitsLeft.z, LimitsRight.w)};
				
				return;
			}
		}
	}

	public class VRCEyeTrackingBoneLimitsExporter : INNAJsonSerializer
	{
		public static readonly System.Type _Target = typeof(VRCAvatarDescriptor);
		public System.Type Target => _Target;

		public List<(string, string)> Serialize(UnityEngine.Object UnityObject)
		{
			var avatar = (VRCAvatarDescriptor)UnityObject;			
			if(avatar.enableEyeLook == true)
			{
				var ret = new JObject {{"t", VRCEyeTrackingBoneLimitsJson._Type}};

				var linkedUpDown = FixAngle(-avatar.customEyeLookSettings.eyesLookingUp.left.eulerAngles.x) == FixAngle(-avatar.customEyeLookSettings.eyesLookingUp.right.eulerAngles.x)
						&& FixAngle(avatar.customEyeLookSettings.eyesLookingDown.left.eulerAngles.x) == FixAngle(avatar.customEyeLookSettings.eyesLookingDown.right.eulerAngles.x);
				var linkedLeftRight = FixAngle(avatar.customEyeLookSettings.eyesLookingRight.left.eulerAngles.y) == FixAngle(-avatar.customEyeLookSettings.eyesLookingLeft.right.eulerAngles.y)
						&& FixAngle(-avatar.customEyeLookSettings.eyesLookingLeft.left.eulerAngles.y) == FixAngle(avatar.customEyeLookSettings.eyesLookingRight.right.eulerAngles.y);

				ret.Add("linked", linkedUpDown && linkedLeftRight);
				ret.Add("left_up", FixAngle(-avatar.customEyeLookSettings.eyesLookingUp.left.eulerAngles.x));
				ret.Add("left_down", FixAngle(avatar.customEyeLookSettings.eyesLookingDown.left.eulerAngles.x));
				ret.Add("left_in", FixAngle(avatar.customEyeLookSettings.eyesLookingRight.left.eulerAngles.y));
				ret.Add("left_out", FixAngle(-avatar.customEyeLookSettings.eyesLookingLeft.left.eulerAngles.y));
				if(!linkedUpDown || !linkedLeftRight)
				{
					ret.Add("right_up", FixAngle(-avatar.customEyeLookSettings.eyesLookingUp.right.eulerAngles.x));
					ret.Add("right_down", FixAngle(avatar.customEyeLookSettings.eyesLookingDown.right.eulerAngles.x));
					ret.Add("right_in", FixAngle(-avatar.customEyeLookSettings.eyesLookingLeft.right.eulerAngles.y));
					ret.Add("right_out", FixAngle(avatar.customEyeLookSettings.eyesLookingRight.right.eulerAngles.y));
				}
				return new List<(string, string)>{(VRCEyeTrackingBoneLimitsJson._Type, ret.ToString(Newtonsoft.Json.Formatting.None))};
			}
			else
			{
				return null;
			}
		}

		public static float FixAngle(float Angle)
		{
			while (Angle >= 180f) Angle -= 360f;
			while (Angle <= -180f) Angle += 360f;

			return (float)Math.Round(Angle % 360f, 2);
		}
	}

	[InitializeOnLoad]
	public class Register_VRCEyeTrackingBoneLimits
	{
		static Register_VRCEyeTrackingBoneLimits()
		{
			NNARegistry.RegisterGlobalProcessor(new VRCEyeTrackingBoneLimitsJson(), DetectorVRC.NNA_VRC_AVATAR_CONTEXT, true);
			NNAJsonExportRegistry.RegisterSerializer(new VRCEyeTrackingBoneLimitsExporter());
		}
	}
}

#endif
#endif