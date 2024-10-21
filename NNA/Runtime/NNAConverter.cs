
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace nna
{
	public static class NNAConverter
	{
		public static void Convert(NNAContext Context)
		{
			var Trash = new List<Transform>();
			var Ignore = new List<Transform>();
			foreach(var nnaNode in Context.Root.GetComponentsInChildren<Transform>())
			{
				// Simple naming logic
				foreach(var processor in Context.NameProcessors)
				{
					if(processor.Value.CanProcessName(Context, nnaNode))
					{
						processor.Value.ProcessName(Context, nnaNode);
						break;
					}
				}

				// Json components
				foreach(JObject component in ParseUtil.ParseNode(nnaNode, Trash).Cast<JObject>())
				{
					if(Context.ContainsJsonProcessor(component))
					{
						Context.Get(component).ProcessJson(Context, nnaNode, component);
					}
					else
					{
						Debug.LogWarning($"Processor not found for NNA type: {Context.GetType(component)}");
						continue;
					}
				}
			}
			Context.RunTasks();
			if(Context.ImportOptions.CleanNodeNames) foreach(var t in Trash)
			{
				Object.DestroyImmediate(t.gameObject);
			}
		}
	}
}
