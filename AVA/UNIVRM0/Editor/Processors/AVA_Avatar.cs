#if UNITY_EDITOR
#if NNA_AVA_UNIVRM0_FOUND

using System.Linq;
using System.Text.RegularExpressions;
using Newtonsoft.Json.Linq;
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
		public const uint _Order = NNA_Humanoid_JsonProcessor._Order + 1;
		public uint Order => _Order;

		public void Process(NNAContext Context)
		{
			if(Context.GetJsonComponentByType("ava.avatar").Count > 1) throw new NNAException("Only one 'ava.avatar' component allowed!", _Type);
			var Json = Context.GetOnlyJsonComponentByType("ava.avatar", (new JObject(), null)).Component;
			var avatar = AVAUNICRM0Utils.InitAvatarDescriptor(Context);

			if(Json.ContainsKey("id")) Context.AddResultById((string)Json["id"], avatar);

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


	public class AVA_ViewportFirstPerson_VRC_Processor : INameProcessor
	{
		public const string MatchViewport = @"(?i)\$ViewportFirstPerson$";

		public const string _Type = "ava.viewport.first_person";
		public string Type => _Type;
		public uint Order => AVA_Avatar_UNIVRM0Processor._Order + 1;

		public int CanProcessName(NNAContext Context, string NameDefinition)
		{
			var match = Regex.Match(NameDefinition, MatchViewport);
			return match.Success ? match.Index : -1;
		}

		public void Process(NNAContext Context, Transform Node, string NameDefinition)
		{
			var vrmFirstPerson = Context.Root.AddComponent<VRMFirstPerson>();
			vrmFirstPerson.FirstPersonBone = Node.parent;
			vrmFirstPerson.FirstPersonOffset = Node.localPosition;
			Context.AddTrash(Node);
		}
	}


	[InitializeOnLoad]
	public class Register_AVA_Avatar_UniVRM0
	{
		static Register_AVA_Avatar_UniVRM0()
		{
			NNARegistry.RegisterGlobalProcessor(new AVA_Avatar_UNIVRM0Processor(), DetectorUNIVRM0.NNA_UNIVRM0_CONTEXT, true);
			NNARegistry.RegisterNameProcessor(new AVA_ViewportFirstPerson_VRC_Processor(), DetectorUNIVRM0.NNA_UNIVRM0_CONTEXT);
		}
	}
}

#endif
#endif
