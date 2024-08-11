
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace nna
{
	public static class NNAConverter
	{
		public static void Convert(NNAContext Context)
		{
			var Trash = new List<Transform>();
			foreach(var t in Context.Root.GetComponentsInChildren<Transform>())
			{
				if(!Trash.Contains(t) && ParseUtil.IsNNANode(t.name))
				{
					foreach(JObject component in ParseUtil.ParseNode(Context.Root, t.gameObject, Trash))
					{

						if(Context.ContainsProcessor(component))
						{
							var actualNodeName = ParseUtil.GetActualNodeName(t.name);
							Context.Get(component).Process(Context, t.gameObject, component);
							if(string.IsNullOrWhiteSpace(actualNodeName)) Trash.Add(t);
							else t.name = actualNodeName;
						}
						else
						{
							Debug.LogWarning($"Processor not found for NNA type: {Context.GetType(component)}");
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
