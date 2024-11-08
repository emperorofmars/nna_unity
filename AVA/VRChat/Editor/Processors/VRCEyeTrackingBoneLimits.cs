#if UNITY_EDITOR

using System.Linq;
using System.Threading.Tasks;
using nna.processors;
using UnityEditor;
using UnityEngine;
using VRC.SDK3.Avatars.Components;

namespace nna.ava.vrchat
{
	public class VRCEyeTrackingBoneLimits : IGlobalProcessor
	{
		public const string _Type = "ava.eyetracking_bone_limits";
		public string Type => _Type;

		public void Process(NNAContext Context)
		{
			var explicitAvatar = Context.GetJsonComponent(Context.Root.transform, "ava.avatar");
			if(explicitAvatar != null && explicitAvatar.ContainsKey("auto") && !(bool)explicitAvatar["auto"]) return;

			Context.AddTask(new Task(() => {
				var Json = Context.GetJsonComponent(Context.Root.transform, _Type);
				var avatar = Context.Root.GetComponent<VRCAvatarDescriptor>();
				var animator = Context.Root.GetComponent<Animator>();

				// set eyebones if human
				if(animator.isHuman)
				{
					var humanEyeL = animator.avatar.humanDescription.human.FirstOrDefault(hb => hb.humanName == HumanBodyBones.LeftEye.ToString());
					var humanEyeR = animator.avatar.humanDescription.human.FirstOrDefault(hb => hb.humanName == HumanBodyBones.RightEye.ToString());

					// eye animation based on bones
					if(humanEyeL.boneName != null && humanEyeR.boneName != null)
					{
						avatar.enableEyeLook = true;

						animator.avatarRoot.GetComponentsInChildren<Transform>().FirstOrDefault(t => t.name == humanEyeL.boneName);

						avatar.customEyeLookSettings.leftEye = animator.avatarRoot.GetComponentsInChildren<Transform>().FirstOrDefault(t => t.name == humanEyeL.boneName);
						avatar.customEyeLookSettings.rightEye = animator.avatarRoot.GetComponentsInChildren<Transform>().FirstOrDefault(t => t.name == humanEyeR.boneName);
						
						var up = (float)ParseUtil.GetMulkikeyOrDefault(Json, 15.0f, "u", "up");
						var down = (float)ParseUtil.GetMulkikeyOrDefault(Json, 12.0f, "d", "down");
						var inner = (float)ParseUtil.GetMulkikeyOrDefault(Json, 15.0f, "i", "in");
						var outer = (float)ParseUtil.GetMulkikeyOrDefault(Json, 16.0f, "o", "out");

						avatar.customEyeLookSettings.eyesLookingUp = new VRCAvatarDescriptor.CustomEyeLookSettings.EyeRotations
								{left = Quaternion.Euler(-up, 0f, 0f), right = Quaternion.Euler(-up, 0f, 0f), linked = true};
						avatar.customEyeLookSettings.eyesLookingDown = new VRCAvatarDescriptor.CustomEyeLookSettings.EyeRotations
								{left = Quaternion.Euler(down, 0f, 0f), right = Quaternion.Euler(down, 0f, 0f), linked = true};
						avatar.customEyeLookSettings.eyesLookingLeft = new VRCAvatarDescriptor.CustomEyeLookSettings.EyeRotations
								{left = Quaternion.Euler(0f, -outer, 0f), right = Quaternion.Euler(0f, -inner, 0f), linked = false};
						avatar.customEyeLookSettings.eyesLookingRight = new VRCAvatarDescriptor.CustomEyeLookSettings.EyeRotations
								{left = Quaternion.Euler(0f, inner, 0f), right = Quaternion.Euler(0f, outer, 0f), linked = false};
						
						return;
					}
				}
			}));
		}
	}

	[InitializeOnLoad]
	public class Register_VRCEyeTrackingBoneLimits
	{
		static Register_VRCEyeTrackingBoneLimits()
		{
			NNARegistry.RegisterGlobalProcessor(new VRCEyeTrackingBoneLimits(), DetectorVRC.NNA_VRC_AVATAR_CONTEXT, true);
		}
	}
}

#endif