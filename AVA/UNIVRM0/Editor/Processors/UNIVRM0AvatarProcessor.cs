#if NNA_AVA_UNIVRM0_FOUND
#if UNITY_EDITOR

using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using nna.processors;
using UnityEditor;
using UnityEngine;
using VRM;

namespace nna.ava.univrm0
{
	public class UNIVRM0AvatarAutodetector : IGlobalProcessor
	{
		public const string _Type = "ava.avatar";
		public string Type => _Type;

		public void Process(NNAContext Context)
		{
			Context.AddTask(new Task(() => {
				if(Context.Root.GetComponent<VRMMeta>() == null)
				{
					var avatar = AVAUNICRM0Utils.InitAvatarDescriptor(Context);
					var animator = AVAUNICRM0Utils.GetOrInitAnimator(Context);
					
					// Autodetect avatar features
					foreach(var feature in AVAUNIVRM0Registry.Features)
					{
						feature.Value.AutoDetect(Context, avatar, new JObject());
					}
				}
			}));
		}
	}
	public class UNIVRM0AvatarProcessor : IJsonProcessor
	{
		public const string _Type = "ava.avatar";
		public string Type => _Type;

		public void ProcessJson(NNAContext Context, Transform Node, JObject Json)
		{
			var avatar = AVAUNICRM0Utils.InitAvatarDescriptor(Context);

			Context.AddTask(new Task(() => {
				var animator = AVAUNICRM0Utils.GetOrInitAnimator(Context);

				if(Json.ContainsKey("features"))
				{
					// Create avatar as configured
				}
				else
				{
					// Autodetect avatar features
					foreach(var feature in AVAUNIVRM0Registry.Features)
					{
						feature.Value.AutoDetect(Context, avatar, Json);
					}
				}
			}));
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
			vrmMeta.Title = Context.Root.name;

			// TODO: Add optional meta information to ava.avatar
			vrmMeta.Version = "0.0.1";
			vrmMeta.Author = "tmp";
			/*vrmMeta.ExporterVersion = asset.Version;
			vrmMeta.OtherLicenseUrl = asset.LicenseLink;
			vrmMeta.Thumbnail = asset.Preview;*/

			Context.AddObjectToAsset(vrmMeta.name, vrmMeta);
			
			// set viewport
			if(Context.Root.transform.GetComponentsInChildren<Transform>().FirstOrDefault(t => t.name == "ViewportFirstPerson") is var viewportNode && viewportNode != null)
			{
				var vrmFirstPerson = Context.Root.AddComponent<VRMFirstPerson>();
				vrmFirstPerson.FirstPersonBone = viewportNode.parent;
				vrmFirstPerson.FirstPersonOffset = viewportNode.transform.position;
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
	public class Register_UNIVRM0AvatarProcessor
	{
		static Register_UNIVRM0AvatarProcessor()
		{
			NNARegistry.RegisterJsonProcessor(new UNIVRM0AvatarProcessor(), UNIVRM0AvatarProcessor._Type, DetectorUNIVRM0.NNA_UNIVRM0_CONTEXT);
			NNARegistry.RegisterGlobalProcessor(new UNIVRM0AvatarAutodetector(), UNIVRM0AvatarAutodetector._Type, DetectorUNIVRM0.NNA_UNIVRM0_CONTEXT);
		}
	}
}

#endif
#endif