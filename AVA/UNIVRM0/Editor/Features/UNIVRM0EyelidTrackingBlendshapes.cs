#if NNA_AVA_UNIVRM0_FOUND
#if UNITY_EDITOR

using Newtonsoft.Json.Linq;
using nna.ava.common;
using UnityEngine;
using VRM;

namespace nna.ava.univrm0
{
	public class UNIVRM0EyelidTrackingBlendshapes : IAVAFeatureUNIVRM0
	{
		public const string _Type = "ava.eyelidtracking";
		public string Type => _Type;

		public bool AutoDetect(NNAContext Context, Component UnityComponent, JObject Json)
		{
			if(!Context.Root.TryGetComponent<VRMBlendShapeProxy>(out var vrmBlendshapeProxy)) return false;

			SkinnedMeshRenderer smr = Utils.FindMainMesh(Context.Root.transform, (string)Json["meshinstance"]);
			if(!smr) return false;

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
			
			return true;
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
}

#endif
#endif