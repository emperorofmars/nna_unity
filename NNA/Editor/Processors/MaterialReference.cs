#if UNITY_EDITOR

using System.Linq;
using Newtonsoft.Json.Linq;
using nna.util;
using UnityEditor;
using UnityEngine;

namespace nna.processors
{
	public class MaterialReference : IJsonProcessor
	{
		public string Type => "nna.material_reference";

		public void Process(NNAContext Context, Transform Node, JObject Json)
		{
			Renderer renderer = Node.GetComponent<SkinnedMeshRenderer>();
			if(!renderer) renderer = Node.GetComponent<MeshRenderer>();
			if(!renderer || !Json.ContainsKey("slots")) return;

			for(int matIdx = 0; matIdx < Json["slots"].Count() && matIdx < renderer.sharedMaterials.Length; matIdx++)
			{
				var mat = AssetResourceUtil.FindAsset<Material>((string)Json["slots"][matIdx], true, "mat");
				if(mat) renderer.sharedMaterials[matIdx] = mat; // Works if run outside of an asset import
				Context.Remaps.Add((renderer.sharedMaterials[matIdx], mat)); // works if run within an asset postprocessor
			}
		}
	}

	[InitializeOnLoad]
	public class Register_MaterialReference
	{
		static Register_MaterialReference()
		{
			NNARegistry.RegisterJsonProcessor(new MaterialReference(), NNARegistry.DefaultContext);
		}
	}
}

#endif