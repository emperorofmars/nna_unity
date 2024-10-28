
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace nna
{
	/// <summary>
	/// The main star of the show. This is where the processors are determined and executed.
	/// </summary>
	public static class NNAConverter
	{
		public static void Convert(NNAContext Context)
		{
			// This builds a dictionary of Node -> List<(Id Component)> relationships and figures out which node is being overridden by another.
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

			// Execute global processors first.
			foreach(var processor in Context.GlobalProcessors)
			{
				processor.Value.Process(Context);
			}

			// Run Json Processors on the `$nna` subtree in the imported hierarchy if it exists.
			if(Context.Root.transform.Find("$nna") is var nnaTree && nnaTree != null)
			{
				// The `$root` node targets the actual root node of the hirarchy.
				if(nnaTree.Find("$root") is var nnaRoot && nnaRoot != null)
				{
					ProcessNodeJson(Context, Context.Root.transform, nnaRoot);
					Context.AddTrash(nnaRoot);
					Context.AddTrash(nnaRoot.GetComponentsInChildren<Transform>());
				}
				// Every other node must specify a target node outside the `$nna` subtree.
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

			// Run name processors on every nodename outside the `$nna` subtree.
			foreach(var node in Context.Root.GetComponentsInChildren<Transform>())
			{
				if(Context.Trash.Contains(node)) continue;

				foreach(var processor in Context.NameProcessors)
				{
					if(processor.Value.CanProcessName(Context, node.name))
					{
						processor.Value.Process(Context, node, node.name);
						break;
					}
				}
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
