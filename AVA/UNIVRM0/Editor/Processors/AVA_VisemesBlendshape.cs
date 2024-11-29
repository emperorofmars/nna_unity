#if UNITY_EDITOR
#if NNA_AVA_UNIVRM0_FOUND

using nna.ava.common;
using nna.processors;
using UnityEditor;
using UnityEngine;
using VRM;

namespace nna.ava.univrm0
{
	public class AVA_VoiceVisemesBlendshape_UNIVRM0Processor : IGlobalProcessor
	{
		public const string _Type = "ava.voice_visemes_blendshape";
		public string Type => _Type;
		public uint Order => AVA_Avatar_UNIVRM0Processor._Order + 1;

		public void Process(NNAContext Context)
		{
			var avatarJson = Context.GetOnlyJsonComponentByType("ava.avatar").Component;
			if(avatarJson != null && avatarJson.ContainsKey("auto") && !(bool)avatarJson["auto"]) return;
			
			var Json = Context.GetJsonComponentOrDefault(Context.Root.transform, _Type);
			
			if(!Context.Root.TryGetComponent<VRMBlendShapeProxy>(out var vrmBlendshapeProxy)) throw new NNAException("No VRMBlendShapeProxy found!", _Type);

			SkinnedMeshRenderer smr = Utils.FindMainMesh(Context.Root.transform, (string)Json["meshinstance"]);
			if(!smr) throw new NNAException("No SkinnedMeshRenderer found!", _Type);

			var mappings = VisemeBlendshapeMapping.Map(smr);
			if(mappings.Count < 5) return;

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
		}
	}

	[InitializeOnLoad]
	public class Register_AVA_VoiceVisemesBlendshape_UNIVRM0
	{
		static Register_AVA_VoiceVisemesBlendshape_UNIVRM0()
		{
			NNARegistry.RegisterGlobalProcessor(new AVA_VoiceVisemesBlendshape_UNIVRM0Processor(), DetectorUNIVRM0.NNA_UNIVRM0_CONTEXT, true);
		}
	}
}

#endif
#endif