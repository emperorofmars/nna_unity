#if UNITY_EDITOR
#if NNA_AVA_VRCSDK3_FOUND

using nna.ava.common;
using nna.processors;
using UnityEditor;
using UnityEngine;
using VRC.SDK3.Avatars.Components;

namespace nna.ava.vrchat
{
	public class AVA_EyelidTrackingBlendshape_VRCProcessor : IGlobalProcessor
	{
		public const string _Type = "ava.eyelidtracking_blendshape";
		public string Type => _Type;
		public uint Order => AVA_Avatar_VRCProcessor._Order + 1;

		public void Process(NNAContext Context)
		{
			var avatarJson = Context.GetOnlyJsonComponentByType("ava.avatar").Component;
			if(avatarJson != null && avatarJson.ContainsKey("auto") && !(bool)avatarJson["auto"]) return;
			
			var Json = Context.GetJsonComponentOrDefault(Context.Root.transform, _Type);
			var avatar = Context.Root.GetComponent<VRCAvatarDescriptor>();
			if(!avatar) throw new NNAException("No Avatar Component created!", _Type);
			
			SkinnedMeshRenderer smr = Utils.FindMainMesh(Context.Root.transform, (string)Json["meshinstance"]);
			if(!smr) throw new NNAException("No SkinnedMeshRenderer found!", _Type);
			
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

	[InitializeOnLoad]
	public class Register_AVA_EyelidTrackingBlendshape_VRC
	{
		static Register_AVA_EyelidTrackingBlendshape_VRC()
		{
			NNARegistry.RegisterGlobalProcessor(new AVA_EyelidTrackingBlendshape_VRCProcessor(), DetectorVRC.NNA_VRC_AVATAR_CONTEXT, true);
		}
	}
}

#endif
#endif