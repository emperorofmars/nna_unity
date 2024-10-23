#if AVA_VRCSDK3_FOUND
#if UNITY_EDITOR

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using UnityEngine;
using VRC.SDK3.Avatars.Components;

namespace nna.ava.vrchat
{
	public class VRCVisemesBlendshapes : IAVAFeatureVRC
	{
		public const string _Type = "ava.voice_visemes";
		public string Type => _Type;

		[System.Serializable]
		public class BlendshapeMapping
		{
			public string VisemeName;
			public string BlendshapeName;
		}
		public static readonly List<string> VoiceVisemes15 = new List<string> {
			"sil", "aa", "ch", "dd", "e", "ff", "ih", "kk", "nn", "oh", "ou", "pp", "rr", "ss", "th"
		};

		public static List<BlendshapeMapping> Map(SkinnedMeshRenderer MeshRenderer)
		{
			Mesh mesh = MeshRenderer.sharedMesh;

			var Mappings = new List<BlendshapeMapping>();
			foreach(var v in VoiceVisemes15)
			{
				string match = null;
				for(int i = 0; i < mesh.blendShapeCount; i++)
				{
					var bName = mesh.GetBlendShapeName(i);
					if(bName.ToLower().Contains("vrc." + v)) { match = bName; break; }
					else if(bName.ToLower().Contains("vrc.v_" + v)) { match = bName; break; }
					else if(bName.ToLower().Contains("vis." + v)) { match = bName; break; }
					else if(bName.ToLower().Contains("vis_" + v)) { match = bName; break; }
				}
				Mappings.Add(new BlendshapeMapping{VisemeName = v, BlendshapeName = match});
			}
			return Mappings;
		}

		public bool AutoDetect(NNAContext Context, Component UnityComponent, JObject Json)
		{
			var avatar = Context.Root.GetComponent<VRCAvatarDescriptor>();

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
			
			var Mappings = Map(smr);

			if(Mappings.Count == 15)
			{
				Context.AddTask(new Task(() => {
					avatar.VisemeSkinnedMesh = smr;
					avatar.lipSync = VRC.SDKBase.VRC_AvatarDescriptor.LipSyncStyle.VisemeBlendShape;
					avatar.VisemeBlendShapes = new string[15];
					avatar.VisemeBlendShapes[0] = Mappings.Find(m => m.VisemeName == "sil")?.BlendshapeName;
					avatar.VisemeBlendShapes[1] = Mappings.Find(m => m.VisemeName == "pp")?.BlendshapeName;
					avatar.VisemeBlendShapes[2] = Mappings.Find(m => m.VisemeName == "ff")?.BlendshapeName;
					avatar.VisemeBlendShapes[3] = Mappings.Find(m => m.VisemeName == "th")?.BlendshapeName;
					avatar.VisemeBlendShapes[4] = Mappings.Find(m => m.VisemeName == "dd")?.BlendshapeName;
					avatar.VisemeBlendShapes[5] = Mappings.Find(m => m.VisemeName == "kk")?.BlendshapeName;
					avatar.VisemeBlendShapes[6] = Mappings.Find(m => m.VisemeName == "ch")?.BlendshapeName;
					avatar.VisemeBlendShapes[7] = Mappings.Find(m => m.VisemeName == "ss")?.BlendshapeName;
					avatar.VisemeBlendShapes[8] = Mappings.Find(m => m.VisemeName == "nn")?.BlendshapeName;
					avatar.VisemeBlendShapes[9] = Mappings.Find(m => m.VisemeName == "rr")?.BlendshapeName;
					avatar.VisemeBlendShapes[10] = Mappings.Find(m => m.VisemeName == "aa")?.BlendshapeName;
					avatar.VisemeBlendShapes[11] = Mappings.Find(m => m.VisemeName == "e")?.BlendshapeName;
					avatar.VisemeBlendShapes[12] = Mappings.Find(m => m.VisemeName == "ih")?.BlendshapeName;
					avatar.VisemeBlendShapes[13] = Mappings.Find(m => m.VisemeName == "oh")?.BlendshapeName;
					avatar.VisemeBlendShapes[14] = Mappings.Find(m => m.VisemeName == "ou")?.BlendshapeName;
				}));
				return true;
			}
			else return false;
		}
	}
}

#endif
#endif