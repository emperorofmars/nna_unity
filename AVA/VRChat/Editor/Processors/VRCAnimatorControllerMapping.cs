#if AVA_VRCSDK3_FOUND
#if UNITY_EDITOR

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using nna.processors;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;
using VRC.SDK3.Avatars.Components;

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

				//avatar.customExpressions = true;

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

				if((string)Json["fx"] is var matchFX && !string.IsNullOrWhiteSpace(matchFX))
				{
					var resultPaths = AssetDatabase.FindAssets(matchFX)
						.Select(guid => AssetDatabase.GUIDToAssetPath(guid))
						.Where(r => r.ToLower().EndsWith(".controller") && r.StartsWith("Assets/"))
						.OrderBy(r => Path.GetFileNameWithoutExtension(r).Length);
					
					if(resultPaths.Count() > 0 && resultPaths.First() is var path)
					{
						var controllerFX = AssetDatabase.LoadAssetAtPath<AnimatorController>(path);
						avatar.baseAnimationLayers[4].isDefault = false;
						avatar.baseAnimationLayers[4].isEnabled = true;
						avatar.baseAnimationLayers[4].animatorController = controllerFX;
					}
				}

			}));
		}
	}

	[InitializeOnLoad]
	public class Register_VRCAnimatorControllerMapping
	{
		static Register_VRCAnimatorControllerMapping()
		{
			NNARegistry.RegisterJsonProcessor(new VRCAnimatorControllerMapping(), VRCAnimatorControllerMapping._Type, DetectorVRC.NNA_VRC_AVATAR_CONTEXT);
		}
	}
}

#endif
#endif