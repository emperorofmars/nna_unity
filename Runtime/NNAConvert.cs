
using System.Collections.Generic;
using UnityEngine;

namespace nna
{
	public static class NNAConvert
	{
		public static void Convert(GameObject Root)
		{
			var Trash = new List<Transform>();
			foreach(var t in Root.GetComponentsInChildren<Transform>())
			{
				if(ParseUtil.HasNNAType(t.name))
				{
					var ActualNodeName = ParseUtil.GetActualNodeName(t.name);
					var NNAType = ParseUtil.GetNNAType(t.name);

					//Debug.Log($"node: {t.name} | type: {NNAType} | name: {ActualNodeName} | nna def: {ParseUtil.GetNNADefinition(t.name)}");
				
					if(NNARegistry.ContainsProcessor(NNAType))
					{
						NNARegistry.Get(NNAType).Process(Root, t.gameObject, out var delete);
						if(string.IsNullOrWhiteSpace(ActualNodeName))Trash.Add(t);
						else t.name = ActualNodeName;
					}
					else
					{
						Debug.LogWarning($"Processor not found for NNA type: {NNAType}");
						continue;
					}
				}
			}
			foreach(var t in Trash)
			{
				Object.DestroyImmediate(t);
			}
		}
	}
}
