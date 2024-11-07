#if UNITY_EDITOR

using System.Linq;
using System.Threading.Tasks;
using nna.processors;
using UnityEditor;
using UnityEngine;
using VRM;

namespace nna.ava.univrm0
{
	public class UNIVRM0Avatar : IGlobalProcessor
	{
		public const string _Type = "ava.avatar";
		public string Type => _Type;

		public void Process(NNAContext Context)
		{
			var avatar = AVAUNICRM0Utils.InitAvatarDescriptor(Context);
			Context.AddTask(new Task(() => {
				if(Context.Root.GetComponent<VRMMeta>() == null)
				{
					var animator = AVAUNICRM0Utils.GetOrInitAnimator(Context);
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
	public class Register_UNIVRM0Avatar
	{
		static Register_UNIVRM0Avatar()
		{
			NNARegistry.RegisterGlobalProcessor(new UNIVRM0Avatar(), DetectorUNIVRM0.NNA_UNIVRM0_CONTEXT, true);
		}
	}
}

#endif