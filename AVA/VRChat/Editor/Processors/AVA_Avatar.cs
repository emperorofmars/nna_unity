#if UNITY_EDITOR
#if NNA_AVA_VRCSDK3_FOUND

using nna.processors;
using UnityEngine;
using VRC.SDK3.Avatars.Components;
using System.Linq;
using UnityEditor;
using Newtonsoft.Json.Linq;
using System.Text.RegularExpressions;

namespace nna.ava.vrchat
{
	public class AVA_Avatar_VRC_Processor : IGlobalProcessor
	{
		public const string _Type = "ava.avatar";
		public string Type => _Type;
		public const uint _Order = NNA_Humanoid_JsonProcessor._Order + 1;
		public uint Order => _Order;

		public void Process(NNAContext Context)
		{
			if(Context.GetJsonComponentByType("ava.avatar").Count > 1) throw new NNAException("Only one 'ava.avatar' component allowed!", NNAErrorSeverity.ERROR, _Type);
			var Json = Context.GetOnlyJsonComponentByType("ava.avatar", (new JObject(), null)).Component;
			var avatar = AVAVRCUtils.InitAvatarDescriptor(Context);

			if(Json.ContainsKey("id")) Context.AddResultById((string)Json["id"], avatar);

			if(Context.Root.GetComponent<VRCAvatarDescriptor>() == null)
			{
				AVAVRCUtils.GetOrInitAnimator(Context);
			}
		}
	}

	public static class AVAVRCUtils
	{
		public static VRCAvatarDescriptor InitAvatarDescriptor(NNAContext Context)
		{
			var avatar = Context.Root.AddComponent<VRCAvatarDescriptor>();
			return avatar;
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
		public uint Order => AVA_Avatar_VRC_Processor._Order + 1;

		public int CanProcessName(NNAContext Context, string NameDefinition)
		{
			var match = Regex.Match(NameDefinition, MatchViewport);
			return match.Success ? match.Index : -1;
		}

		public void Process(NNAContext Context, Transform Node, string NameDefinition)
		{
			var avatar = Context.Root.GetComponent<VRCAvatarDescriptor>();
			if(!avatar) throw new NNAException("No Avatar Component created!", NNAErrorSeverity.ERROR, _Type);

			avatar.ViewPosition = Node.position - Context.Root.transform.position;
			Context.AddTrash(Node);
		}
	}


	[InitializeOnLoad]
	public class Register_AVA_Avatar_VRC
	{
		static Register_AVA_Avatar_VRC()
		{
			NNARegistry.RegisterGlobalProcessor(new AVA_Avatar_VRC_Processor(), DetectorVRC.NNA_VRC_AVATAR_CONTEXT, true);
			NNARegistry.RegisterNameProcessor(new AVA_ViewportFirstPerson_VRC_Processor(), DetectorVRC.NNA_VRC_AVATAR_CONTEXT);
		}
	}
}

#endif
#endif
