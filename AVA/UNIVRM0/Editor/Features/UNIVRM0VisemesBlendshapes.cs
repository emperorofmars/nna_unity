#if NNA_AVA_UNIVRM0_FOUND
#if UNITY_EDITOR

using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using nna.ava.common;
using UnityEngine;
using VRM;

namespace nna.ava.univrm0
{
	public class UNIVRM0VisemesBlendshapes : IAVAFeatureUNIVRM0
	{
		public const string _Type = "ava.voice_visemes_blendshape";
		public string Type => _Type;

		public bool AutoDetect(NNAContext Context, Component UnityComponent, JObject Json)
		{
			if(!Context.Root.TryGetComponent<VRMBlendShapeProxy>(out var vrmBlendshapeProxy)) return false;

			SkinnedMeshRenderer smr = Utils.FindMainMesh(Context.Root.transform, (string)Json["meshinstance"]);
			if(!smr) return false;

			var mappings = VisemeBlendshapeMapping.Map(smr);
			if(mappings.Count < 5) return false;

			if(mappings.ContainsKey("aa"))
			{
				var clip = BlendshapeClipUtil.CreateSimple(Context, BlendShapePreset.A, smr, mappings["aa"]);
				vrmBlendshapeProxy.BlendShapeAvatar.Clips.Add(clip);
				Context.AddObjectToAsset(clip.name, clip);
			}

			if(mappings.ContainsKey("e"))
			{
				var clip = BlendshapeClipUtil.CreateSimple(Context, BlendShapePreset.E, smr, mappings["e"]);
				vrmBlendshapeProxy.BlendShapeAvatar.Clips.Add(clip);
				Context.AddObjectToAsset(clip.name, clip);
			}

			if(mappings.ContainsKey("ih"))
			{
				var clip = BlendshapeClipUtil.CreateSimple(Context, BlendShapePreset.I, smr, mappings["ih"]);
				vrmBlendshapeProxy.BlendShapeAvatar.Clips.Add(clip);
				Context.AddObjectToAsset(clip.name, clip);
			}

			if(mappings.ContainsKey("oh"))
			{
				var clip = BlendshapeClipUtil.CreateSimple(Context, BlendShapePreset.O, smr, mappings["oh"]);
				vrmBlendshapeProxy.BlendShapeAvatar.Clips.Add(clip);
				Context.AddObjectToAsset(clip.name, clip);
			}

			if(mappings.ContainsKey("ou"))
			{
				var clip = BlendshapeClipUtil.CreateSimple(Context, BlendShapePreset.U, smr, mappings["ou"]);
				vrmBlendshapeProxy.BlendShapeAvatar.Clips.Add(clip);
				Context.AddObjectToAsset(clip.name, clip);
			}

			return true;
		}
	}
}

#endif
#endif
