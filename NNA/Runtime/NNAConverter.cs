
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
			// Build the node->List<component> dict and figure out which components are being overridden
			foreach(var node in Context.Root.GetComponentsInChildren<Transform>())
			{
				var componentList = new List<JObject>();
				Context.ComponentMap.Add(node, componentList);
				foreach(JObject component in ParseUtil.ParseNode(node, Context.Trash).Cast<JObject>())
				{
					if(Context.IgnoreList.FirstOrDefault(t => t == (string)component["t"]) == null) componentList.Add(component);
					if(Context.ContainsJsonProcessor(component) && component.ContainsKey("overrides")) foreach(var overrideId in component["overrides"])
					{
						Context.Overrides.Add((string)overrideId, (component, node));
					}
				}
			}

			foreach(var processor in Context.GlobalProcessors)
			{
				processor.Value.Process(Context);
			}

			if(Context.Root.transform.Find("$nna") is var nnaTree && nnaTree != null)
			{
				if(nnaTree.Find("$root") is var nnaRoot && nnaRoot != null)
				{
					ProcessNodeJson(Context, Context.Root.transform, nnaRoot);
					Context.AddTrash(nnaRoot);
					Context.AddTrash(nnaRoot.GetComponentsInChildren<Transform>());
				}
				foreach(var node in nnaTree.GetComponentsInChildren<Transform>())
				{
					if(Context.Trash.Contains(node)) continue;
					
					if(node.name.StartsWith("$target:"))
					{
						var targetName = node.name[8 ..];
						var target = Context.Root.transform.GetComponentsInChildren<Transform>().FirstOrDefault(t => t.name == targetName);
						if(target) ProcessNodeJson(Context, target, node);
					}
				}
				Context.AddTrash(nnaTree);
				Context.AddTrash(nnaTree.GetComponentsInChildren<Transform>());
			}

			foreach(var node in Context.Root.GetComponentsInChildren<Transform>())
			{
				if(Context.Trash.Contains(node)) continue;

				// Simple naming logic
				foreach(var processor in Context.NameProcessors)
				{
					if(processor.Value.CanProcessName(Context, node.name))
					{
						processor.Value.Process(Context, node, node.name);
						break;
					}
				}

				// Json components
				ProcessNodeJson(Context, node, node);
			}
			Context.RunTasks();
		}

		private static void ProcessNodeJson(NNAContext Context, Transform TargetNode, Transform NNANode)
		{
			if(Context.ComponentMap.ContainsKey(NNANode)) foreach(JObject component in Context.ComponentMap[NNANode])
			{
				if(Context.ContainsJsonProcessor(component) && (!component.ContainsKey("id") || !Context.Overrides.ContainsKey((string)component["id"])))
				{
					Context.Get(component).Process(Context, TargetNode, component);
				}
				else
				{
					Debug.LogWarning($"Processor not found for NNA type: {Context.GetType(component)}");
					continue;
				}
			}
		}
	}
}
