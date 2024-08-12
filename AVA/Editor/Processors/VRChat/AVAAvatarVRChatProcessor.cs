#if AVA_VRCSDK3_FOUND
#if UNITY_EDITOR

using Newtonsoft.Json.Linq;
using nna.processors;
using UnityEngine;
using UnityEditor;
using VRC.SDK3.Avatars.Components;
using System.Threading.Tasks;
using System.Linq;

namespace nna.ava.vrchat
{
	public class AVAAvatarVRChatProcessor : IProcessor
	{
		public const string _Type = "ava.avatar";
		public string Type => _Type;

		public void Process(NNAContext Context, GameObject Target, GameObject NNANode, JObject Json)
		{
			var vrcAvatar = Context.Root.AddComponent<VRCAvatarDescriptor>();

			Context.AddTask(new Task(() => {
				Animator animator = Context.Root.GetComponent<Animator>();
				if(animator == null)
				{
					animator = Context.Root.AddComponent<Animator>();
				}
				animator.applyRootMotion = true;
				animator.updateMode = AnimatorUpdateMode.Normal;
				animator.cullingMode = AnimatorCullingMode.CullUpdateTransforms;

				if(animator.isHuman)
				{
					var humanEyeL = animator.avatar.humanDescription.human.FirstOrDefault(hb => hb.humanName == HumanBodyBones.LeftEye.ToString());
					var humanEyeR = animator.avatar.humanDescription.human.FirstOrDefault(hb => hb.humanName == HumanBodyBones.RightEye.ToString());

					vrcAvatar.customEyeLookSettings.leftEye = FindBone(animator.avatarRoot, humanEyeL.boneName);
					vrcAvatar.customEyeLookSettings.rightEye  = FindBone(animator.avatarRoot, humanEyeR.boneName);
				}
			}));
		}

		private Transform FindBone(Transform Root, string Name)
		{
			foreach(var t in Root.GetComponentsInChildren<Transform>()) if(t.name == Name) return t;
			return null;
		}
	}
	public class AVAViewportVRChatProcessor : IProcessor
	{
		public const string _Type = "ava.viewport";
		public string Type => _Type;

		public void Process(NNAContext Context, GameObject Target, GameObject NNANode, JObject Json)
		{
			Context.AddTask(new Task(() => {
				var vrcAvatar = Context.Root.GetComponent<VRCAvatarDescriptor>();
				vrcAvatar.enableEyeLook = true;
				vrcAvatar.ViewPosition = NNANode.transform.position - Context.Root.transform.position;
			}));
		}
	}

	[InitializeOnLoad]
	public class Register_AVAVRChatProcessor
	{
		static Register_AVAVRChatProcessor()
		{
			NNARegistry.RegisterProcessor(new AVAAvatarVRChatProcessor(), AVAAvatarVRChatProcessor._Type, DetectorVRC.NNA_VRC_AVATAR_CONTEXT);
			NNARegistry.RegisterProcessor(new AVAViewportVRChatProcessor(), AVAViewportVRChatProcessor._Type, DetectorVRC.NNA_VRC_AVATAR_CONTEXT);
		}
	}
}

#endif
#endif
