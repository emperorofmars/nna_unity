
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
				if(!Trash.Contains(t) && ParseUtil.IsNNANode(t.name))
				{
					var actualNodeName = ParseUtil.GetActualNodeName(t.name);
					var components = ParseUtil.ParseNode(Root, t.gameObject, Trash);
					foreach(var component in components)
					{
						if(NNARegistry.ContainsProcessor(component.Key))
						{
							NNARegistry.Get(component.Key).Process(Root, t.gameObject, component.Value);
							if(string.IsNullOrWhiteSpace(actualNodeName)) Trash.Add(t);
							else t.name = actualNodeName;
						}
						else
						{
							Debug.LogWarning($"Processor not found for NNA type: {component.Key}");
							continue;
						}
					}
				}
			}
			foreach(var t in Trash)
			{
				Object.DestroyImmediate(t.gameObject);
			}
		}
	}
}
