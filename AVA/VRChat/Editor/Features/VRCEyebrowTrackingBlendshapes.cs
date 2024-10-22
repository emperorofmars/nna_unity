#if AVA_VRCSDK3_FOUND
#if UNITY_EDITOR

using System.Linq;
using Newtonsoft.Json.Linq;
using UnityEngine;
using VRC.SDK3.Avatars.Components;

namespace nna.ava.vrchat
{
	public class VRCEyebrowTrackingBlendshapes : IAVAFeature
	{
		public const string _Type = "ava.eyebrowtracking";
		public string Type => _Type;

		public bool AutoDetect(NNAContext Context, Component UnityComponent, JObject Json)
		{
			var avatar = Context.Root.GetComponent<VRCAvatarDescriptor>();
			//var animator = Context.Root.GetComponent<Animator>();

			SkinnedMeshRenderer smr = null;
			if(Json.ContainsKey("meshinstance"))
			{
				smr = Context.Root.transform.GetComponentsInChildren<SkinnedMeshRenderer>().FirstOrDefault(t => t.name == (string)Json["meshinstance"]);
			}
			else
			{
				smr = Context.Root.transform.GetComponentsInChildren<SkinnedMeshRenderer>().FirstOrDefault(t => t.name == "Body");
			}
			
			avatar.customEyeLookSettings.eyelidType = VRCAvatarDescriptor.EyelidType.Blendshapes;
			//var smr = meshTransform.gameObject.GetComponent<SkinnedMeshRenderer>();
			
			avatar.customEyeLookSettings.eyelidType = VRCAvatarDescriptor.EyelidType.Blendshapes;
			avatar.customEyeLookSettings.eyelidsSkinnedMesh = smr;
			avatar.customEyeLookSettings.eyelidsBlendshapes = new int[3];
			
			// Also allow for these to be explicitely mapped in the nna component
			avatar.customEyeLookSettings.eyelidsBlendshapes[0] = GetBlendshapeIndex(smr.sharedMesh, MapEyeLidBlendshapes(smr.sharedMesh, "eye_closed"));
			avatar.customEyeLookSettings.eyelidsBlendshapes[1] = GetBlendshapeIndex(smr.sharedMesh, MapEyeLidBlendshapes(smr.sharedMesh, "look_up"));
			avatar.customEyeLookSettings.eyelidsBlendshapes[2] = GetBlendshapeIndex(smr.sharedMesh, MapEyeLidBlendshapes(smr.sharedMesh, "look_down"));

			return false;
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