#if UNITY_EDITOR

using System.Linq;
using Newtonsoft.Json.Linq;
using nna.util;
using UnityEditor;
using UnityEditor.UI;
using UnityEngine;

namespace nna.processors
{
	public class NNA_MaterialMapping : IJsonProcessor
	{
		public string Type => "nna.material_mapping";
		public uint Order => 0;
		public int Priority => 0;

		public void Process(NNAContext Context, Transform Node, JObject Json)
		{
			Renderer renderer = Node.GetComponent<SkinnedMeshRenderer>();
			if(!renderer) renderer = Node.GetComponent<MeshRenderer>();
			if(!renderer || !Json.ContainsKey("slots")) throw new NNAException("No MeshRenderer Component found!", NNAErrorSeverity.ERROR, Type, Node);

			for(int matIdx = 0; matIdx < Json["slots"].Count() && matIdx < renderer.sharedMaterials.Length; matIdx++)
			{
				var mat = AssetResourceUtil.FindAsset<Material>(Context, (string)Json["slots"][matIdx], true, "mat");
				if(mat)
				{
					renderer.sharedMaterials[matIdx] = mat; // Works if run outside of an asset import
					Context.AddRemap(renderer.sharedMaterials[matIdx], mat); // works if run within an asset postprocessor
				}
				else
				{
					Context.Report(new($"Could not find material: {(string)Json["slots"][matIdx]}", NNAErrorSeverity.WARNING, Type, Node));
				}
			}
		}
	}

	[InitializeOnLoad]
	public class Register_NNA_MaterialMapping
	{
		static Register_NNA_MaterialMapping()
		{
			NNARegistry.RegisterJsonProcessor(new NNA_MaterialMapping(), NNARegistry.DefaultContext);
		}
	}
}

#endif
