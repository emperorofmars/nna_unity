
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using nna.processors;
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

			// Build execution map for global processors.
			foreach(var processor in Context.GlobalProcessors)
			{
				Context.AddProcessorTask(processor.Value.Order, new Task(() => {
					processor.Value.Process(Context);
				}));
			}

			// Build execution map for json processors.
			foreach(var node in Context.Root.GetComponentsInChildren<Transform>())
			{
				if(Context.Trash.Contains(node)) continue;

				ProcessNodeJson(Context, node);
			}

			// Build execution map for name processors on every nodename outside the `$nna` subtree.
			foreach(var node in Context.Root.GetComponentsInChildren<Transform>())
			{
				if(Context.Trash.Contains(node)) continue;

				int shortestStartIndex = -1;
				INameProcessor selectedProcessor = null;
				foreach(var processor in Context.NameProcessors)
				{
					var startIndex = processor.Value.CanProcessName(Context, node.name);
					if(startIndex >= 0 && (shortestStartIndex < 0 || startIndex < shortestStartIndex))
					{
						shortestStartIndex = startIndex;
						selectedProcessor = processor.Value;
					}
				}
				if(selectedProcessor != null)
				{
					Context.RegisterNameComponent(node, selectedProcessor.Type, (uint)shortestStartIndex);
					Context.AddProcessorTask(selectedProcessor.Order, new Task(() => {
						selectedProcessor.Process(Context, node, node.name);
					}));
				}
			}

			Context.Run();
		}

		private static void ProcessNodeJson(NNAContext Context, Transform TargetNode)
		{
			foreach(JObject component in Context.GetJsonComponentsByNode(TargetNode))
			{
				if(Context.ContainsJsonProcessor(component))
				{
					if(!component.ContainsKey("id") || !Context.IsOverridden((string)component["id"]))
					{
						Context.AddProcessorTask(Context.GetJsonProcessor(component).Order, new Task(() => {
							Context.GetJsonProcessor(component).Process(Context, TargetNode, component);
						}));
					}
				}
				else if(!Context.IsIgnored((string)component["t"]))
				{
					Debug.LogWarning($"Processor not found for NNA type: {Context.GetType(component)}");
					continue;
				}
			}
		}
	}
}
