
using System.Collections.Generic;
using System.Collections.Immutable;
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
			// This builds a dictionary of Node -> List<Component> relationships and figures out which node is being overridden by another.
			foreach(var node in Context.Root.GetComponentsInChildren<Transform>())
			{
				var componentList = new List<JObject>();
				foreach(JObject component in ParseUtil.ParseNode(node, Context.Trash).Cast<JObject>())
				{
					if(Context.IgnoreList.FirstOrDefault(t => t == (string)component["t"]) == null) componentList.Add(component);
					if(Context.ContainsJsonProcessor(component) && component.ContainsKey("overrides")) foreach(var overrideId in component["overrides"])
					{
						Context.AddOverride((string)overrideId, component, node);
					}
				}
				Context.AddComponentMap(node, componentList.ToImmutableList());
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
				}
				// Every other node must specify a target node outside the `$nna` subtree.
				foreach(var node in nnaTree.GetComponentsInChildren<Transform>())
				{
					if(Context.Trash.Contains(node)) continue;
					
					if(node.name.StartsWith("$target:"))
					{
						var targetNameFull = node.name[8 ..];
						var target = ParseUtil.FindNode(Context.Root.transform, targetNameFull);
						if(target) ProcessNodeJson(Context, target, node);
						else Debug.LogWarning($"Invalid targeting object: {targetNameFull}");
					}
				}
				Context.AddTrash(nnaTree);
				Context.AddTrash(nnaTree.GetComponentsInChildren<Transform>());
			}

			// Run json processors on every node outside the `$nna` subtree.
			foreach(var node in Context.Root.GetComponentsInChildren<Transform>())
			{
				if(Context.Trash.Contains(node)) continue;

				ProcessNodeJson(Context, node, node);
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
			foreach(JObject component in Context.GetComponents(NNANode))
			{
				if(Context.ContainsJsonProcessor(component))
				{
					if(!component.ContainsKey("id") || !Context.IsOverridden((string)component["id"]))
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
