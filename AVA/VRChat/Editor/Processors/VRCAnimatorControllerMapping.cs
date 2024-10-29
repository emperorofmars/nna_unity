#if UNITY_EDITOR

using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using nna.processors;
using nna.util;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;
using VRC.SDK3.Avatars.Components;
using VRC.SDK3.Avatars.ScriptableObjects;

namespace nna.ava.vrchat
{
	public class VRCAnimatorControllerMapping : IJsonProcessor
	{
		public const string _Type = "vrc.controller_mapping";
		public string Type => _Type;

		public void Process(NNAContext Context, Transform Node, JObject Json)
		{
			Context.AddTask(new Task(() => {
				var avatar = Context.Root.GetComponent<VRCAvatarDescriptor>();

				if((string)Json["parameters"] is var matchParams && !string.IsNullOrWhiteSpace(matchParams))
				{
					avatar.customExpressions = true;
					var expressionParams = AssetResourceUtil.FindAsset<VRCExpressionParameters>(matchParams, true, "controller");
					if(expressionParams)
					{
						avatar.expressionParameters = expressionParams;
					}
				}
				if((string)Json["menu"] is var matchMenu && !string.IsNullOrWhiteSpace(matchMenu))
				{
					avatar.customExpressions = true;
					var expressionsMenu = AssetResourceUtil.FindAsset<VRCExpressionsMenu>(matchMenu, true, "controller");
					if(expressionsMenu)
					{
						avatar.expressionsMenu = expressionsMenu;
					}
				}

				avatar.baseAnimationLayers = new VRCAvatarDescriptor.CustomAnimLayer[] {
					new() {type = VRCAvatarDescriptor.AnimLayerType.Base, isDefault = true},
					new() {type = VRCAvatarDescriptor.AnimLayerType.Additive, isDefault = true},
					new() {type = VRCAvatarDescriptor.AnimLayerType.Gesture, isDefault = true},
					new() {type = VRCAvatarDescriptor.AnimLayerType.Action, isDefault = true},
					new() {type = VRCAvatarDescriptor.AnimLayerType.FX, isDefault = true},
				};
				avatar.specialAnimationLayers = new VRCAvatarDescriptor.CustomAnimLayer[] {
					new() {type = VRCAvatarDescriptor.AnimLayerType.Sitting, isDefault = true},
					new() {type = VRCAvatarDescriptor.AnimLayerType.TPose, isDefault = true},
					new() {type = VRCAvatarDescriptor.AnimLayerType.IKPose, isDefault = true},
				};
				avatar.customizeAnimationLayers = true;

				MatchAnimatorController((string)Json["base"], ref avatar.baseAnimationLayers[0]);
				MatchAnimatorController((string)Json["additive"], ref avatar.baseAnimationLayers[1]);
				MatchAnimatorController((string)Json["gesture"], ref avatar.baseAnimationLayers[2]);
				MatchAnimatorController((string)Json["action"], ref avatar.baseAnimationLayers[3]);
				MatchAnimatorController((string)Json["fx"], ref avatar.baseAnimationLayers[4]);

				MatchAnimatorController((string)Json["sitting"], ref avatar.specialAnimationLayers[0]);
				MatchAnimatorController((string)Json["tpose"], ref avatar.specialAnimationLayers[1]);
				MatchAnimatorController((string)Json["ikpose"], ref avatar.specialAnimationLayers[2]);
			}));
		}

		private static void MatchAnimatorController(string Match, ref VRCAvatarDescriptor.CustomAnimLayer Layer)
		{
			if(!string.IsNullOrWhiteSpace(Match))
			{
				if(AssetResourceUtil.FindAsset<AnimatorController>(Match, true, "controller") is var controllerFX && controllerFX != null)
				{
					Layer.isDefault = false;
					Layer.isEnabled = true;
					Layer.animatorController = controllerFX;
				}
				else
				{
					Layer.isDefault = false;
					Layer.isEnabled = false;
				}
			} // else do not modify
		}
	}

	[InitializeOnLoad]
	public class Register_VRCAnimatorControllerMapping
	{
		static Register_VRCAnimatorControllerMapping()
		{
			NNARegistry.RegisterJsonProcessor(new VRCAnimatorControllerMapping(), DetectorVRC.NNA_VRC_AVATAR_CONTEXT);
		}
	}
}

#endif