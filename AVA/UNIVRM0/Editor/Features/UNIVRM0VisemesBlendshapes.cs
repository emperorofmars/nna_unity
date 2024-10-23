#if NNA_AVA_UNIVRM0_FOUND
#if UNITY_EDITOR

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

		private void createBlendshapeClip(NNAContext Context, string BlendshapeName, BlendShapePreset BlendshapePreset, VRMBlendShapeProxy VRMBlendshapeProxy, SkinnedMeshRenderer Renderer)
		{
			var clip = ScriptableObject.CreateInstance<BlendShapeClip>();
			clip.name = "VRM_Clip_" + BlendshapePreset.ToString();
			clip.BlendShapeName = BlendshapeName;
			clip.Preset = BlendshapePreset;

			var bindingsList = new BlendShapeBinding[1];
			BlendShapeBinding binding = new();
			Mesh mesh = Renderer.sharedMesh;
			binding.Index = mesh.GetBlendShapeIndex(clip.BlendShapeName);
			binding.RelativePath = ParseUtil.GetPath(Context.Root.transform, Renderer.transform);
			binding.Weight = 100;
			bindingsList[0] = binding;
			clip.Values = bindingsList;

			VRMBlendshapeProxy.BlendShapeAvatar.Clips.Add(clip);
			Context.AddObjectToAsset(clip.name, clip);
		}

		public int GetBlendshapeIndex(Mesh mesh, string name)
		{
		
			for(int i = 0; i < mesh.blendShapeCount; i++)
			{
				var bName = mesh.GetBlendShapeName(i);
				if(bName == name) return i;
			}
			return -1;
		}

		public bool AutoDetect(NNAContext Context, Component UnityComponent, JObject Json)
		{
			//var c = (AVAFacialTrackingSimple)Component;
			if(!Context.Root.TryGetComponent<VRMBlendShapeProxy>(out var vrmBlendshapeProxy)) return false;

			SkinnedMeshRenderer smr = null;
			if(Json.ContainsKey("meshinstance"))
			{
				smr = Context.Root.transform.GetComponentsInChildren<SkinnedMeshRenderer>().FirstOrDefault(t => t.name == (string)Json["meshinstance"]);
			}
			else
			{
				smr = Context.Root.transform.GetComponentsInChildren<SkinnedMeshRenderer>().FirstOrDefault(t => t.name == "Body");
			}
			if(!smr) return false;

			var mappings = VisemeBlendshapeMapping.Map(smr);
			if(mappings.Count < 5) return false;

			if(mappings.ContainsKey("aa")) createBlendshapeClip(Context, mappings["aa"], BlendShapePreset.A, vrmBlendshapeProxy, smr);
			if(mappings.ContainsKey("e")) createBlendshapeClip(Context, mappings["e"], BlendShapePreset.E, vrmBlendshapeProxy, smr);
			if(mappings.ContainsKey("ih")) createBlendshapeClip(Context, mappings["ih"], BlendShapePreset.I, vrmBlendshapeProxy, smr);
			if(mappings.ContainsKey("oh")) createBlendshapeClip(Context, mappings["oh"], BlendShapePreset.O, vrmBlendshapeProxy, smr);
			if(mappings.ContainsKey("ou")) createBlendshapeClip(Context, mappings["ou"], BlendShapePreset.U, vrmBlendshapeProxy, smr);

			//createBlendshapeClip("blink", BlendShapePreset.Blink, c, vrmBlendshapeProxy, State);

			return true;
		}
	}
}

#endif
#endif
