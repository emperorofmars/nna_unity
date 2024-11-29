#if UNITY_EDITOR
#if NNA_AVA_UNIVRM0_FOUND

using nna.ava.common;
using nna.processors;
using UnityEditor;
using UnityEngine;
using VRM;

namespace nna.ava.univrm0
{
	public class AVA_EyelidTrackingBlendshape_UNIVRM0Processor : IGlobalProcessor
	{
		public const string _Type = "ava.eyelidtracking_blendshape";
		public string Type => _Type;
		public uint Order => AVA_Avatar_UNIVRM0Processor._Order + 1;

		public void Process(NNAContext Context)
		{
			var avatarJson = Context.GetOnlyJsonComponentByType("ava.avatar").Component;
			if(avatarJson != null && avatarJson.ContainsKey("auto") && !(bool)avatarJson["auto"]) return;

			var Json = Context.GetJsonComponentOrDefault(Context.Root.transform, _Type);

			if(!Context.Root.TryGetComponent<VRMBlendShapeProxy>(out var vrmBlendshapeProxy)) return;

			SkinnedMeshRenderer smr = Utils.FindMainMesh(Context.Root.transform, (string)Json["meshinstance"]);
			if(!smr) throw new NNAException("No SkinnedMeshRenderer found!", _Type);

			// TODO: Also allow for these to be explicitely mapped in the nna component.
			// TODO: This is quite clunky, do this more legitimately at some point.
			if(MapEyeLidBlendshape(smr.sharedMesh, "eye_closed") is var mappingEyeClosed && mappingEyeClosed != null)
			{
				var clip = BlendshapeClipUtil.CreateSimple(Context, BlendShapePreset.Blink, smr, mappingEyeClosed);
				vrmBlendshapeProxy.BlendShapeAvatar.Clips.Add(clip);
				Context.AddObjectToAsset(clip.name, clip);
			}
			else if(MapEyeLidBlendshape(smr.sharedMesh, "blink") is var mappingBlink && mappingBlink != null)
			{
				var clip = BlendshapeClipUtil.CreateSimple(Context, BlendShapePreset.Blink, smr, mappingBlink);
				vrmBlendshapeProxy.BlendShapeAvatar.Clips.Add(clip);
				Context.AddObjectToAsset(clip.name, clip);
			}
			if(MapEyeLidBlendshape(smr.sharedMesh, "eye_closed_left") is var mappingEyeClosedLeft && mappingEyeClosedLeft != null)
			{
				var clip = BlendshapeClipUtil.CreateSimple(Context, BlendShapePreset.Blink_L, smr, mappingEyeClosedLeft);
				vrmBlendshapeProxy.BlendShapeAvatar.Clips.Add(clip);
				Context.AddObjectToAsset(clip.name, clip);
			}
			else if(MapEyeLidBlendshape(smr.sharedMesh, "blink_left") is var mappingBlinkLeft && mappingBlinkLeft != null)
			{
				var clip = BlendshapeClipUtil.CreateSimple(Context, BlendShapePreset.Blink_L, smr, mappingBlinkLeft);
				vrmBlendshapeProxy.BlendShapeAvatar.Clips.Add(clip);
				Context.AddObjectToAsset(clip.name, clip);
			}
			if(MapEyeLidBlendshape(smr.sharedMesh, "eye_closed_right") is var mappingEyeClosedRight && mappingEyeClosedRight != null)
			{
				var clip = BlendshapeClipUtil.CreateSimple(Context, BlendShapePreset.Blink_R, smr, mappingEyeClosedRight);
				vrmBlendshapeProxy.BlendShapeAvatar.Clips.Add(clip);
				Context.AddObjectToAsset(clip.name, clip);
			}
			else if(MapEyeLidBlendshape(smr.sharedMesh, "blink_right") is var mappingBlinkRight && mappingBlinkRight != null)
			{
				var clip = BlendshapeClipUtil.CreateSimple(Context, BlendShapePreset.Blink_R, smr, mappingBlinkRight);
				vrmBlendshapeProxy.BlendShapeAvatar.Clips.Add(clip);
				Context.AddObjectToAsset(clip.name, clip);
			}

			if(MapEyeLidBlendshape(smr.sharedMesh, "look_up") is var mappingLookUp && mappingLookUp != null && !mappingLookUp.ToLower().Contains("left") && !mappingLookUp.ToLower().Contains("right"))
			{
				var clip = BlendshapeClipUtil.CreateSimple(Context, BlendShapePreset.LookUp, smr, mappingLookUp);
				vrmBlendshapeProxy.BlendShapeAvatar.Clips.Add(clip);
				Context.AddObjectToAsset(clip.name, clip);
			}
			if(MapEyeLidBlendshape(smr.sharedMesh, "look_down") is var mappingLookDown && mappingLookDown != null && !mappingLookDown.ToLower().Contains("left") && !mappingLookDown.ToLower().Contains("right"))
			{
				var clip = BlendshapeClipUtil.CreateSimple(Context, BlendShapePreset.LookDown, smr, mappingLookDown);
				vrmBlendshapeProxy.BlendShapeAvatar.Clips.Add(clip);
				Context.AddObjectToAsset(clip.name, clip);
			}

			if(MapEyeLidBlendshape(smr.sharedMesh, "look_left") is var mappingLookLeft && mappingLookLeft != null && !mappingLookLeft.ToLower().Contains("up") && !mappingLookLeft.ToLower().Contains("down"))
			{
				var clip = BlendshapeClipUtil.CreateSimple(Context, BlendShapePreset.LookLeft, smr, mappingLookLeft);
				vrmBlendshapeProxy.BlendShapeAvatar.Clips.Add(clip);
				Context.AddObjectToAsset(clip.name, clip);
			}
			if(MapEyeLidBlendshape(smr.sharedMesh, "look_right") is var mappingLookRight && mappingLookRight != null && !mappingLookLeft.ToLower().Contains("up") && !mappingLookLeft.ToLower().Contains("down"))
			{
				var clip = BlendshapeClipUtil.CreateSimple(Context, BlendShapePreset.LookRight, smr, mappingLookRight);
				vrmBlendshapeProxy.BlendShapeAvatar.Clips.Add(clip);
				Context.AddObjectToAsset(clip.name, clip);
			}
		}

		private static string MapEyeLidBlendshape(Mesh Mesh, string Name)
		{
			var compare = Name.Split('_');
			string match = null;
			for(int i = 0; i < Mesh.blendShapeCount; i++)
			{
				var bName = Mesh.GetBlendShapeName(i);
				bool matchedAll = true;
				foreach(var c in compare)
				{
					if(!bName.ToLower().Contains(c)) { matchedAll = false; break; }
				}
				if(matchedAll && (match == null || bName.Length < match.Length)) match = bName;
			}
			return match;
		}
	}

	[InitializeOnLoad]
	public class Register_AVA_EyelidTrackingBlendshape_UniVRM0
	{
		static Register_AVA_EyelidTrackingBlendshape_UniVRM0()
		{
			NNARegistry.RegisterGlobalProcessor(new AVA_EyelidTrackingBlendshape_UNIVRM0Processor(), DetectorUNIVRM0.NNA_UNIVRM0_CONTEXT, true);
		}
	}
}

#endif
#endif