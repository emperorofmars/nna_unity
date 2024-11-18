#if UNITY_EDITOR
#if NNA_AVA_UNIVRM0_FOUND

using System.Collections.Generic;
using UnityEngine;
using VRM;

namespace nna.ava.univrm0
{
	public static class BlendshapeClipUtil
	{
		public static BlendShapeBinding CreateBinding(NNAContext Context, SkinnedMeshRenderer Renderer, string BlendshapeName, float Weight)
		{
			return new BlendShapeBinding()
			{
				Index = Renderer.sharedMesh.GetBlendShapeIndex(BlendshapeName),
				RelativePath = ParseUtil.GetPath(Context.Root.transform, Renderer.transform),
				Weight = Weight
			};
		}

		public static BlendShapeClip CreateEmpty(BlendShapePreset BlendshapePreset)
		{
			var clip = ScriptableObject.CreateInstance<BlendShapeClip>();
			clip.name = "VRM_Clip_" + BlendshapePreset.ToString();
			clip.BlendShapeName = BlendshapePreset.ToString();
			clip.Preset = BlendshapePreset;
			clip.Values = new BlendShapeBinding[] {};
			return clip;
		}

		public static BlendShapeClip CreateSimple(NNAContext Context, BlendShapePreset BlendshapePreset, SkinnedMeshRenderer Renderer, string BlendshapeName)
		{
			var clip = ScriptableObject.CreateInstance<BlendShapeClip>();
			clip.name = "VRM_Clip_" + BlendshapePreset.ToString();
			clip.BlendShapeName = BlendshapePreset.ToString();
			clip.Preset = BlendshapePreset;

			var bindingsList = new BlendShapeBinding[] {CreateBinding(Context, Renderer, BlendshapeName, 100)};
			clip.Values = bindingsList;

			return clip;
		}

		public static BlendShapeClip Create(NNAContext Context, BlendShapePreset BlendshapePreset, string ClipName, List<(SkinnedMeshRenderer Renderer, List<(string Name, float Weight)> Blendshapes)> Blendshapes)
		{
			var clip = ScriptableObject.CreateInstance<BlendShapeClip>();
			clip.name = "VRM_Clip_" + ClipName;
			clip.BlendShapeName = ClipName;
			clip.Preset = BlendshapePreset;

			var bindingList = new List<BlendShapeBinding>();
			foreach(var renderer in Blendshapes) foreach(var blendshape in renderer.Blendshapes)
			{
				bindingList.Add(CreateBinding(Context, renderer.Renderer, blendshape.Name, blendshape.Weight));
			}
			clip.Values = bindingList.ToArray();
			return clip;
		}
	}
}

#endif
#endif