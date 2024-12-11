#if UNITY_EDITOR
#if NNA_AVA_VRCSDK3_FOUND

using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Newtonsoft.Json.Linq;
using nna.ava.common;
using nna.processors;
using nna.UnityToNNAUtils;
using UnityEditor;
using UnityEngine;
using VRC.SDK3.Avatars.Components;

namespace nna.ava.vrchat
{
	public class AVA_EyeTrackingBoneLimits_VRC_JsonProcessor : IJsonProcessor
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

	public class AVA_EyeTrackingBoneLimits_VRC_NameProcessor : INameProcessor
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

	public class AVA_EyeTrackingBoneLimits_VRC_Processor : IGlobalProcessor
	{
		public const string _Type = EyeTrackingBoneLimits._Type;
		public string Type => _Type;
		public uint Order => AVA_Avatar_VRCProcessor._Order + 1;

		public void Process(NNAContext Context)
		{
			var avatarJson = Context.GetOnlyJsonComponentByType("ava.avatar").Component;
			if(avatarJson != null
				&& avatarJson.ContainsKey("auto")
				&& !(bool)avatarJson["auto"]
				&& !EyeTrackingBoneLimits.LimitsExplicitelyDefined(Context)
			) return; // No automapping otherwise

			var avatar = Context.Root.GetComponent<VRCAvatarDescriptor>();
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
				throw new NNAException("Animator is not 'Humanoid'!", _Type);
			}
		}

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


	public class AVA_EyeTrackingBoneLimits_VRC_Serializer : INNASerializer
	{
		public static readonly System.Type _Target = typeof(VRCAvatarDescriptor);
		public System.Type Target => _Target;

		public List<SerializerResult> Serialize(NNASerializerContext Context, UnityEngine.Object UnityObject)
		{
			var avatar = (VRCAvatarDescriptor)UnityObject;
			if(avatar.enableEyeLook == true)
			{
				var retJson = new JObject {{"t", EyeTrackingBoneLimits._Type}};

				var linkedUpDown = FixAngle(-avatar.customEyeLookSettings.eyesLookingUp.left.eulerAngles.x) == FixAngle(-avatar.customEyeLookSettings.eyesLookingUp.right.eulerAngles.x)
						&& FixAngle(avatar.customEyeLookSettings.eyesLookingDown.left.eulerAngles.x) == FixAngle(avatar.customEyeLookSettings.eyesLookingDown.right.eulerAngles.x);
				var linkedLeftRight = FixAngle(avatar.customEyeLookSettings.eyesLookingRight.left.eulerAngles.y) == FixAngle(-avatar.customEyeLookSettings.eyesLookingLeft.right.eulerAngles.y)
						&& FixAngle(-avatar.customEyeLookSettings.eyesLookingLeft.left.eulerAngles.y) == FixAngle(avatar.customEyeLookSettings.eyesLookingRight.right.eulerAngles.y);

				retJson.Add("linked", linkedUpDown && linkedLeftRight);
				retJson.Add("left_up", FixAngle(-avatar.customEyeLookSettings.eyesLookingUp.left.eulerAngles.x));
				retJson.Add("left_down", FixAngle(avatar.customEyeLookSettings.eyesLookingDown.left.eulerAngles.x));
				retJson.Add("left_in", FixAngle(avatar.customEyeLookSettings.eyesLookingRight.left.eulerAngles.y));
				retJson.Add("left_out", FixAngle(-avatar.customEyeLookSettings.eyesLookingLeft.left.eulerAngles.y));
				if(!linkedUpDown || !linkedLeftRight)
				{
					retJson.Add("right_up", FixAngle(-avatar.customEyeLookSettings.eyesLookingUp.right.eulerAngles.x));
					retJson.Add("right_down", FixAngle(avatar.customEyeLookSettings.eyesLookingDown.right.eulerAngles.x));
					retJson.Add("right_in", FixAngle(-avatar.customEyeLookSettings.eyesLookingLeft.right.eulerAngles.y));
					retJson.Add("right_out", FixAngle(avatar.customEyeLookSettings.eyesLookingRight.right.eulerAngles.y));
				}


				string retName = null;
				if(linkedUpDown && linkedLeftRight)
				{
					retName = "$EyeBoneLimits"
						+ FixAngle(-avatar.customEyeLookSettings.eyesLookingUp.left.eulerAngles.x)
						+ "," + FixAngle(avatar.customEyeLookSettings.eyesLookingDown.left.eulerAngles.x)
						+ "," + FixAngle(avatar.customEyeLookSettings.eyesLookingRight.left.eulerAngles.y)
						+ "," + FixAngle(-avatar.customEyeLookSettings.eyesLookingLeft.left.eulerAngles.y);
				}

				return new List<SerializerResult>{new(){
					NNAType = EyeTrackingBoneLimits._Type,
					Origin = UnityObject,
					JsonTargetNode = "$root",
					JsonResult = retJson.ToString(Newtonsoft.Json.Formatting.None),
					JsonComponentId = Context.GetId(UnityObject),
					IsJsonComplete = true,
					NameResult = retName,
					NameTargetNode = "$root",
					IsNameComplete = retName != null,
					Confidence = SerializerResultConfidenceLevel.MANUAL,
				}};
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

			return (float)System.Math.Round(Angle % 360f, 2);
		}
	}

	[InitializeOnLoad]
	public class Register_AVA_EyeTrackingBoneLimits_VRC
	{
		static Register_AVA_EyeTrackingBoneLimits_VRC()
		{
			NNARegistry.RegisterGlobalProcessor(new AVA_EyeTrackingBoneLimits_VRC_Processor(), DetectorVRC.NNA_VRC_AVATAR_CONTEXT, false);
			NNARegistry.RegisterJsonProcessor(new AVA_EyeTrackingBoneLimits_VRC_JsonProcessor(), DetectorVRC.NNA_VRC_AVATAR_CONTEXT);
			NNARegistry.RegisterNameProcessor(new AVA_EyeTrackingBoneLimits_VRC_NameProcessor(), DetectorVRC.NNA_VRC_AVATAR_CONTEXT);
			NNAExportRegistry.RegisterSerializer(new AVA_EyeTrackingBoneLimits_VRC_Serializer());
		}
	}
}

#endif
#endif
