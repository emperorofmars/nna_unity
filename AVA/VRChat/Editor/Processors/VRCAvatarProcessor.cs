#if AVA_VRCSDK3_FOUND
#if UNITY_EDITOR

using Newtonsoft.Json.Linq;
using nna.processors;
using UnityEngine;
using VRC.SDK3.Avatars.Components;
using System.Threading.Tasks;
using System.Linq;
using UnityEditor;

namespace nna.ava.vrchat
{
	public class VRCAvatarAutodetector : IGlobalProcessor
	{
		public const string _Type = "ava.avatar";
		public string Type => _Type;

		public void Process(NNAContext Context)
		{
			Context.AddTask(new Task(() => {
				if(Context.Root.GetComponent<VRCAvatarDescriptor>() == null)
				{
					var avatar = AVAVRCUtils.InitAvatarDescriptor(Context);
					var animator = AVAVRCUtils.GetOrInitAnimator(Context);
					
					// Autodetect avatar features
					foreach(var feature in AVAVRChatFeatures.Features)
					{
						//feature.Value.AutoDetect(Context, avatar, Json);
					}
				}
			}));
		}
	}

	public class VRCAvatarProcessor : IJsonProcessor
	{
		public const string _Type = "ava.avatar";
		public string Type => _Type;

		public void ProcessJson(NNAContext Context, Transform Node, JObject Json)
		{
			var avatar = AVAVRCUtils.InitAvatarDescriptor(Context);

			Context.AddTask(new Task(() => {
				var animator = AVAVRCUtils.GetOrInitAnimator(Context);

				if(Json.ContainsKey("features"))
				{
					// Create avatar as configured
				}
				else
				{
					// Autodetect avatar features
					foreach(var feature in AVAVRChatFeatures.Features)
					{
						feature.Value.AutoDetect(Context, avatar, Json);
					}
				}
			}));
		}
	}

	public static class AVAVRCUtils
	{
		public static VRCAvatarDescriptor InitAvatarDescriptor(NNAContext Context)
		{
			var avatar = Context.Root.AddComponent<VRCAvatarDescriptor>();
			
			// set viewport
			if(Context.Root.transform.GetComponentsInChildren<Transform>().FirstOrDefault(t => t.name == "ViewportFirstPerson") is var viewportNode && viewportNode != null)
			{
				avatar.ViewPosition = viewportNode.transform.position - Context.Root.transform.position;
			}

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

	[InitializeOnLoad]
	public class Register_AVAVRChatProcessor
	{
		static Register_AVAVRChatProcessor()
		{
			NNARegistry.RegisterJsonProcessor(new VRCAvatarProcessor(), VRCAvatarProcessor._Type, DetectorVRC.NNA_VRC_AVATAR_CONTEXT);
			NNARegistry.RegisterGlobalProcessor(new VRCAvatarAutodetector(), VRCAvatarAutodetector._Type, DetectorVRC.NNA_VRC_AVATAR_CONTEXT);
		}
	}
}

#endif
#endif