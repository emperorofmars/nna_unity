#if UNITY_EDITOR
#if NNA_AVA_VRCSDK3_FOUND

using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using nna.processors;
using nna.UnityToNNAUtils;
using nna.util;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;
using VRC.SDK3.Avatars.Components;
using VRC.SDK3.Avatars.ScriptableObjects;

namespace nna.ava.vrchat
{
	public class VRC_ControllerMapping_VRCJsonProcessor : IJsonProcessor
	{
		public const string _Type = "vrc.controller_mapping";
		public string Type => _Type;
		public uint Order => AVA_Avatar_VRCProcessor._Order + 1;

		public void Process(NNAContext Context, Transform Node, JObject Json)
		{
			var avatar = Context.Root.GetComponent<VRCAvatarDescriptor>();

			if((string)Json["parameters"] is var matchParams && !string.IsNullOrWhiteSpace(matchParams))
			{
				avatar.customExpressions = true;
				var expressionParams = AssetResourceUtil.FindAsset<VRCExpressionParameters>(matchParams, true, "asset");
				if(expressionParams)
				{
					avatar.expressionParameters = expressionParams;
				}
			}
			if((string)Json["menu"] is var matchMenu && !string.IsNullOrWhiteSpace(matchMenu))
			{
				avatar.customExpressions = true;
				var expressionsMenu = AssetResourceUtil.FindAsset<VRCExpressionsMenu>(matchMenu, true, "asset");
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
		}

		private static void MatchAnimatorController(string Match, ref VRCAvatarDescriptor.CustomAnimLayer Layer)
		{
			if(!string.IsNullOrWhiteSpace(Match))
			{
				if(AssetResourceUtil.FindAsset<AnimatorController>(Match, true, "controller") is var controller && controller != null)
				{
					Layer.isDefault = false;
					Layer.isEnabled = true;
					Layer.animatorController = controller;
				}
				else
				{
					Layer.isDefault = false;
					Layer.isEnabled = false;
				}
			} // else do not modify
		}
	}

	public class VRC_ControllerMapping_VRCSerializer : INNASerializer
	{
		public System.Type Target => typeof(VRCAvatarDescriptor);

		public List<SerializerResult> Serialize(NNASerializerContext Context, UnityEngine.Object UnityObject)
		{
			var ret = new JObject {{"t", VRC_ControllerMapping_VRCJsonProcessor._Type}};
			var avatar = (VRCAvatarDescriptor)UnityObject;

			if(avatar.customizeAnimationLayers && avatar.baseAnimationLayers != null && avatar.baseAnimationLayers.Length == 5)
			{
				if(avatar.baseAnimationLayers[0].animatorController) ret.Add("base", avatar.baseAnimationLayers[0].animatorController.name);
				if(avatar.baseAnimationLayers[1].animatorController) ret.Add("additive", avatar.baseAnimationLayers[1].animatorController.name);
				if(avatar.baseAnimationLayers[2].animatorController) ret.Add("gesture", avatar.baseAnimationLayers[2].animatorController.name);
				if(avatar.baseAnimationLayers[3].animatorController) ret.Add("action", avatar.baseAnimationLayers[3].animatorController.name);
				if(avatar.baseAnimationLayers[4].animatorController) ret.Add("fx", avatar.baseAnimationLayers[4].animatorController.name);
			}
			if(avatar.customizeAnimationLayers && avatar.specialAnimationLayers != null && avatar.specialAnimationLayers.Length == 3)
			{
				if(avatar.specialAnimationLayers[0].animatorController) ret.Add("sitting", avatar.specialAnimationLayers[0].animatorController.name);
				if(avatar.specialAnimationLayers[1].animatorController) ret.Add("tpose", avatar.specialAnimationLayers[1].animatorController.name);
				if(avatar.specialAnimationLayers[2].animatorController) ret.Add("ikpose", avatar.specialAnimationLayers[2].animatorController.name);
			}
			if(avatar.customExpressions && avatar.expressionParameters)  ret.Add("parameters", avatar.expressionParameters.name);
			if(avatar.customExpressions && avatar.expressionsMenu)  ret.Add("menu", avatar.expressionsMenu.name);

			return new List<SerializerResult>{new(){
				NNAType = VRC_ControllerMapping_VRCJsonProcessor._Type,
				Origin = UnityObject,
				JsonTargetNode = "$root",
				JsonResult = ret.ToString(Newtonsoft.Json.Formatting.None),
				JsonComponentId = Context.GetId(UnityObject),
				IsJsonComplete = true,
			}};
		}
	}

	[InitializeOnLoad]
	public class Register_VRCAnimatorControllerMapping
	{
		static Register_VRCAnimatorControllerMapping()
		{
			NNARegistry.RegisterJsonProcessor(new VRC_ControllerMapping_VRCJsonProcessor(), DetectorVRC.NNA_VRC_AVATAR_CONTEXT);
			NNAExportRegistry.RegisterSerializer(new VRC_ControllerMapping_VRCSerializer());
		}
	}
}

#endif
#endif