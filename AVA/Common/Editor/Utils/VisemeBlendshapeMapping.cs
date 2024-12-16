#if UNITY_EDITOR

using System.Collections.Generic;
using UnityEngine;

namespace nna.ava.common
{
	public static class VisemeBlendshapeMapping
	{
		public static readonly List<string> VoiceVisemes15 = new() {
			"sil", "aa", "ch", "dd", "e", "ff", "ih", "kk", "nn", "oh", "ou", "pp", "rr", "ss", "th"
		};

		public static Dictionary<string, string> Map(SkinnedMeshRenderer MeshRenderer)
		{
			var Mappings = new Dictionary<string, string>();
			Mesh mesh = MeshRenderer.sharedMesh;
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
				Mappings.Add(v, match);
			}
			return Mappings;
		}
	}
}

#endif
