#if UNITY_EDITOR
#if NNA_AVA_UNIVRM0_FOUND

using System.Linq;
using nna.processors;
using UnityEditor;
using UnityEngine;
using VRM;

namespace nna.ava.univrm0
{
	public class AVA_Avatar_UNIVRM0Processor : IGlobalProcessor
	{
		public const string _Type = "ava.avatar";
		public string Type => _Type;
		public const uint _Order = 1;
		public uint Order => _Order;

		public void Process(NNAContext Context)
		{
			var avatar = AVAUNICRM0Utils.InitAvatarDescriptor(Context);
			
			var avatarComponentJson = Context.GetJsonComponentByNode(Context.Root.transform, "ava.avatar");
			if(avatarComponentJson.ContainsKey("id")) avatar.name = "$nna:" + (string)avatarComponentJson["id"];
			
			if(Context.Root.GetComponent<VRMMeta>() == null)
			{
				AVAUNICRM0Utils.GetOrInitAnimator(Context);
			}
		}
	}

	public static class AVAUNICRM0Utils
	{
		public static VRMMeta InitAvatarDescriptor(NNAContext Context)
		{
			var vrmMetaComponent = Context.Root.AddComponent<VRMMeta>();
			var vrmMeta = ScriptableObject.CreateInstance<VRMMetaObject>();
			vrmMeta.name = "VRM_Meta";
			vrmMetaComponent.Meta = vrmMeta;

			if(Context.GetMeta())
			{
				vrmMeta.Title = Context.GetMeta().AssetName;
				vrmMeta.Author = Context.GetMeta().Author;
				vrmMeta.Version = Context.GetMeta().Version;
				vrmMeta.ContactInformation = Context.GetMeta().URL;
				vrmMeta.OtherLicenseUrl = Context.GetMeta().LicenseLink;
				vrmMeta.Reference = Context.GetMeta().DocumentationLink;
			}
			else
			{
				vrmMeta.Title = Context.Root.name;
				vrmMeta.Version = "0.0.1";
			}
			Context.AddObjectToAsset(vrmMeta.name, vrmMeta);
			
			// set viewport
			if(Context.Root.transform.GetComponentsInChildren<Transform>().FirstOrDefault(t => t.name == "ViewportFirstPerson") is var viewportNode && viewportNode != null)
			{
				var vrmFirstPerson = Context.Root.AddComponent<VRMFirstPerson>();
				vrmFirstPerson.FirstPersonBone = viewportNode.parent;
				vrmFirstPerson.FirstPersonOffset = viewportNode.transform.position;
				Context.AddTrash(viewportNode);
			}
			
			var vrmBlendshapeProxy = Context.Root.AddComponent<VRMBlendShapeProxy>();
			var vrmBlendShapeAvatar = ScriptableObject.CreateInstance<BlendShapeAvatar>();
			vrmBlendShapeAvatar.name = "VRM_BlendshapeAvatar";

			vrmBlendshapeProxy.BlendShapeAvatar = vrmBlendShapeAvatar;
			Context.AddObjectToAsset(vrmBlendShapeAvatar.name, vrmBlendShapeAvatar);

			var neutralClip = BlendshapeClipUtil.CreateEmpty(BlendShapePreset.Neutral);
			Context.AddObjectToAsset(neutralClip.name, neutralClip);
			vrmBlendShapeAvatar.Clips.Add(neutralClip);

			var secondary = new GameObject {name = "VRM_secondary"};
			secondary.transform.SetParent(Context.Root.transform, false);

			return vrmMetaComponent;
		}

		public static Animator GetOrInitAnimator(NNAContext Context)
		{
			if(!Context.Root.TryGetComponent<Animator>(out var animator))
			{
				animator = Context.Root.AddComponent<Animator>();
			}
			animator.applyRootMotion = true;
			animator.updateMode = AnimatorUpdateMode.Normal;
			animator.cullingMode = AnimatorCullingMode.CullUpdateTransforms;

			return animator;
		}
	}

	[InitializeOnLoad]
	public class Register_AVA_Avatar_UniVRM0
	{
		static Register_AVA_Avatar_UniVRM0()
		{
			NNARegistry.RegisterGlobalProcessor(new AVA_Avatar_UNIVRM0Processor(), DetectorUNIVRM0.NNA_UNIVRM0_CONTEXT, true);
		}
	}
}

#endif
#endif