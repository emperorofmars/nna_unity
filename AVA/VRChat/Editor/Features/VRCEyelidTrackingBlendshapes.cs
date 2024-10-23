#if AVA_VRCSDK3_FOUND
#if UNITY_EDITOR

using System.Linq;
using Newtonsoft.Json.Linq;
using nna.ava.common;
using UnityEngine;
using VRC.SDK3.Avatars.Components;

namespace nna.ava.vrchat
{
	public class VRCEyelidTrackingBlendshapes : IAVAFeatureVRC
	{
		public const string _Type = "ava.eyelidtracking";
		public string Type => _Type;

		public bool AutoDetect(NNAContext Context, Component UnityComponent, JObject Json)
		{
			var avatar = Context.Root.GetComponent<VRCAvatarDescriptor>();
			
			SkinnedMeshRenderer smr = Utils.FindMainMesh(Context.Root.transform, (string)Json["meshinstance"]);
			if(!smr) return false;
			
			avatar.customEyeLookSettings.eyelidType = VRCAvatarDescriptor.EyelidType.Blendshapes;
			avatar.customEyeLookSettings.eyelidsSkinnedMesh = smr;
			avatar.customEyeLookSettings.eyelidsBlendshapes = new int[3];

			// TODO: Also allow for these to be explicitely mapped in the nna component.
			// TODO: This is quite clunky, do this more legitimately at some point.
			if(MapEyeLidBlendshapes(smr.sharedMesh, "eye_closed") is var mappingEyeClosed && mappingEyeClosed != null)
				avatar.customEyeLookSettings.eyelidsBlendshapes[0] = GetBlendshapeIndex(smr.sharedMesh, mappingEyeClosed);
			else if(MapEyeLidBlendshapes(smr.sharedMesh, "blink") is var mappingBlink && mappingBlink != null)
				avatar.customEyeLookSettings.eyelidsBlendshapes[0] = GetBlendshapeIndex(smr.sharedMesh, mappingBlink);

			if(MapEyeLidBlendshapes(smr.sharedMesh, "look_up") is var mappingLookUp && mappingLookUp != null && !mappingLookUp.ToLower().Contains("left") && !mappingLookUp.ToLower().Contains("right"))
				avatar.customEyeLookSettings.eyelidsBlendshapes[1] = GetBlendshapeIndex(smr.sharedMesh, mappingLookUp);
				
			if(MapEyeLidBlendshapes(smr.sharedMesh, "look_down") is var mappingLookDown && mappingLookDown != null && !mappingLookDown.ToLower().Contains("left") && !mappingLookDown.ToLower().Contains("right"))
				avatar.customEyeLookSettings.eyelidsBlendshapes[2] = GetBlendshapeIndex(smr.sharedMesh, mappingLookDown);

			return true;
		}

		private static string MapEyeLidBlendshapes(Mesh Mesh, string Name)
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

		private static int GetBlendshapeIndex(Mesh mesh, string name)
		{
			for(int i = 0; i < mesh.blendShapeCount; i++)
			{
				var bName = mesh.GetBlendShapeName(i);
				if(bName == name) return i;
			}
			return -1;
		}
	}
}

#endif
#endif