
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using UnityEngine;
using VRC.Utility;

namespace nna
{
	public static class NNAConverter
	{
		public static void Convert(NNAContext Context)
		{
			var Trash = new List<Transform>();
			var Ignore = new List<Transform>();

			var nnaTree = Context.Root.transform.Find("$nna");
			if(nnaTree)
			{
				var nnaRoot = nnaTree.Find("$root");
				if(nnaRoot)
				{
					ProcessNodeJson(Context, Context.Root.transform, nnaRoot, Trash);
					Trash.Add(nnaRoot);
					Trash.AddRange(nnaRoot.GetComponentsInChildren<Transform>());
				}
				foreach(var node in nnaTree.GetComponentsInChildren<Transform>())
				{
					if(Trash.Contains(node) || Ignore.Contains(node)) continue;
					
					if(node.name.StartsWith("$target:"))
					{
						var targetName = node.name[8 ..];
						var target = Context.Root.transform.GetComponentsInChildren<Transform>().FirstOrDefault(t => t.name == targetName);
						if(target) ProcessNodeJson(Context, target, node, Trash);
					}
				}
				Trash.Add(nnaTree);
				Trash.AddRange(nnaTree.GetComponentsInChildren<Transform>());
			}

			foreach(var node in Context.Root.GetComponentsInChildren<Transform>())
			{
				if(Trash.Contains(node) || Ignore.Contains(node)) continue;

				// Simple naming logic
				foreach(var processor in Context.NameProcessors)
				{
					if(processor.Value.CanProcessName(Context, node.name))
					{
						processor.Value.ProcessName(Context, node, node.name);
						break;
					}
				}

				// Json components
				ProcessNodeJson(Context, node, node, Trash);
			}
			Context.RunTasks();
			if(Context.ImportOptions.RemoveNNAJson) foreach(var t in Trash)
			{
				if(t) Object.DestroyImmediate(t.gameObject);
			}
		}

		private static void ProcessNodeJson(NNAContext Context, Transform TargetNode, Transform NNANode, List<Transform> Trash)
		{
			foreach(JObject component in ParseUtil.ParseNode(NNANode, Trash).Cast<JObject>())
			{
				if(Context.ContainsJsonProcessor(component))
				{
					Context.Get(component).ProcessJson(Context, TargetNode, component);
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
