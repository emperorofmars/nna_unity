
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
			// This builds a dictionary of Node -> List<Component> relationships and figures out which node is being overridden by another.
			foreach(var node in Context.Root.GetComponentsInChildren<Transform>())
			{
				if(node == Context.Root.transform) continue;
				if(node.name == "$meta")
				{
					Context.SetNNAMeta(ParseUtil.ParseMetaNode(node, Context.Trash));
					Context.AddTrash(node);
					continue;
				}
				if(node.name == "$nna")
				{
					Context.AddTrash(node);
					continue;
				}

				// Figure out the actually targeted node
				var target = node;
				if(node.name == "$root")
				{
					target = Context.Root.transform;
				}
				else if(node.name.StartsWith("$target:"))
				{
					var targetNameFull = node.name[8 ..];
					target = ParseUtil.FindNode(Context.Root.transform, targetNameFull);
				}

				// Build the list of components for the target node
				var componentList = new List<JObject>();
				foreach(JObject component in ParseUtil.ParseNode(node, Context.Trash).Cast<JObject>())
				{
					componentList.Add(component);
					if(Context.ContainsJsonProcessor(component) && component.ContainsKey("overrides")) foreach(var overrideId in component["overrides"])
					{
						Context.AddOverride((string)overrideId, component, target);
					}
				}
				Context.AddComponentMap(target, componentList);
			}

			// Execute global processors first.
			foreach(var processor in Context.GlobalProcessors)
			{
				processor.Value.Process(Context);
			}

			// Run json processors.
			foreach(var node in Context.Root.GetComponentsInChildren<Transform>())
			{
				if(Context.Trash.Contains(node)) continue;

				ProcessNodeJson(Context, node);
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

		private static void ProcessNodeJson(NNAContext Context, Transform TargetNode)
		{
			foreach(JObject component in Context.GetComponents(TargetNode))
			{
				if(Context.ContainsJsonProcessor(component))
				{
					if(!component.ContainsKey("id") || !Context.IsOverridden((string)component["id"]))
						Context.Get(component).Process(Context, TargetNode, component);
				}
				else if(Context.IsIgnored((string)component["t"]))
				{
					// Ignore
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
