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
		public static ImportResult Convert(
			GameObject Root,
			NNAImportOptions ImportOptions,
			Dictionary<string, IJsonProcessor> JsonProcessors = null,
			Dictionary<string, INameProcessor> NameProcessors = null,
			Dictionary<string, IGlobalProcessor> GlobalProcessors = null,
			HashSet<string> IgnoreList = null
		){
			var State = new NNAImportState(Root, ImportOptions, JsonProcessors, NameProcessors, GlobalProcessors, IgnoreList);

			var Context = new NNAContext(Root, ImportOptions, State);


			// Setup of processor execution tasks.

			// This builds a dictionary of Node -> List<Component> relationships and figures out which node is being overridden by another.
			foreach(var node in State.Root.GetComponentsInChildren<Transform>())
			{
				if(node == State.Root.transform) continue;
				if(node.name == "$meta")
				{
					State.SetNNAMeta(ParseUtil.ParseMetaNode(node, State.Trash));
					//State.AddTrash(node);
					continue;
				}
				if(node.name == "$nna")
				{
					State.AddTrash(node.GetComponentsInChildren<Transform>());
					continue;
				}

				// Figure out the actually targeted node
				var target = node;
				if(node.name == "$root")
				{
					target = State.Root.transform;
				}
				else if(node.name.StartsWith("$target:"))
				{
					var targetNameFull = node.name[8 ..];
					target = ParseUtil.FindNode(State.Root.transform, targetNameFull);
				}

				if(target != null)
				{
					// Build the list of components for the target node
					var componentList = new List<JObject>();
					foreach(JObject component in ParseUtil.ParseNode(node, State.Trash).Cast<JObject>())
					{
						componentList.Add(component);
						if(State.ContainsJsonProcessor(component) && component.ContainsKey("overrides")) foreach(var overrideId in component["overrides"])
						{
							State.AddOverride((string)overrideId, component, target, State.GetJsonProcessor(component));
						}
					}
					State.AddComponentMap(target, componentList);
				}
				else {
					State.Report(new NNAReport($"Invalid Target ID: {node.name}", NNAErrorSeverity.ERROR, null, node));
				}
			}

			// Build execution map for global processors.
			foreach(var processor in State.GlobalProcessors)
			{
				State.AddProcessorTask(processor.Value.Order, new Task(() => {
					processor.Value.Process(Context);
				}));
			}

			// Build execution map for json processors.
			foreach(var node in State.Root.GetComponentsInChildren<Transform>())
			{
				if(State.Trash.Contains(node)) continue;

				ProcessNodeJson(State, Context, node);
			}

			// Build execution map for name processors on every nodename outside the `$nna` subtree.
			foreach(var node in State.Root.GetComponentsInChildren<Transform>())
			{
				if(State.Trash.Contains(node) || !node.name.Contains("$")) continue;

				int shortestStartIndex = -1;
				INameProcessor selectedProcessor = null;
				foreach(var processor in State.NameProcessors)
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
					State.RegisterNameComponent(node, selectedProcessor.Type);
					var nameDefinition = node.name;

					if(State.ImportOptions.RemoveNNADefinitions && node.name.StartsWith("$")) Context.AddTrash(node);
					if(State.ImportOptions.RemoveNNADefinitions) node.name = ParseUtil.GetNodeNameCleaned(node.name);

					State.AddProcessorTask(selectedProcessor.Order, new Task(() => {
						selectedProcessor.Process(Context, node, nameDefinition);
					}));
				}
				else
				{
					State.Report(new NNAReport($"Invalid Name Component: {node.name}", NNAErrorSeverity.ERROR, null, node));
				}
			}

			// Processor execution.

			// Execute processor tasks in their defined order
			foreach(var (order, taskList) in State.ProcessOrderMap.OrderBy(e => e.Key))
			{
				foreach(var task in taskList)
				{
					task.RunSynchronously();
					if(task.Exception != null)
					{
						HandleTaskException(State, task.Exception);
					}
				}
			}

			// Run any Tasks added to the State during the processor execution
			var maxDepth = 100;
			while(State.Tasks.Count > 0)
			{
				var taskset = State.Tasks;
				State.Tasks = new List<Task>();
				foreach(var task in taskset)
				{
					task.RunSynchronously();
					if(task.Exception != null)
					{
						HandleTaskException(State, task.Exception);
					}
				}

				maxDepth--;
				if(maxDepth <= 0)
				{
					Debug.LogWarning("Maximum recursion depth reached!");
					break;
				}
			}

			// Handly errors.

			if(State.Errors.Count > 0)
			{
				var errorList = ScriptableObject.CreateInstance<NNAErrorList>();
				errorList.name = "NNA Import Errors";

				foreach(var aggregateError in State.Errors)
				{
					foreach(var e in aggregateError.InnerExceptions)
					{
						if(e is NNAException nnaError)
						{
							errorList.Errors.Add(new(){Target=nnaError.Report.Node, ProcessorType = nnaError.Report.ProcessorType, Error=nnaError.Message});
						}
						else
						{
							errorList.Errors.Add(new(){Error=e.Message});
						}
					}
				}
				State.AddObjectToAsset(errorList.name, errorList);
				Debug.LogWarning($"Errors occured during NNA processing! View the \"NNA Import Errors\" in the imported asset for details!");
			}

			// Cleanup

			if(ImportOptions.RemoveNNADefinitions) foreach(var t in State.Trash)
			{
				if(t) Object.DestroyImmediate(t.gameObject);
			}

			return new ImportResult { NewObjects = State.NewObjects, Remaps = State.Remaps };
		}

		private static void ProcessNodeJson(NNAImportState State, NNAContext Context, Transform TargetNode)
		{
			foreach(JObject component in State.GetJsonComponentsByNode(TargetNode))
			{
				if(State.ContainsJsonProcessor(component))
				{
					if(!component.ContainsKey("id") || !State.IsOverridden((string)component["id"]))
					{
						var competingProcessorWins = false;
						// Check if competing components exist, disable execution if one the other overrides has a higher priority.
						if(component.ContainsKey("overrides"))
						{
							foreach(var overrideId in component["overrides"]) // for each override
							{
								if(State.OverriddeMappings.ContainsKey((string)overrideId))
								{
									foreach(var competingProcessor in State.OverriddeMappings[(string)overrideId]) // for each other component also overriding
									{
										if(competingProcessor.Type != (string)component["type"])
										{
											if(competingProcessor.Priority > State.GetJsonProcessor(component).Priority)
											{
												competingProcessorWins = true;
												break;
											}
										}
									}
								}
							}
						}

						if(!competingProcessorWins)
						{
							State.AddProcessorTask(State.GetJsonProcessor(component).Order, new Task(() => {
								State.GetJsonProcessor(component).Process(Context, TargetNode, component);
							}));
						}
					}
				}
				else if(!State.IsIgnored((string)component["t"]))
				{
					Debug.Log($"Processor not found for NNA type: {State.GetType(component)}");
					continue;
				}
			}
		}

		private static void HandleTaskException(NNAImportState State, System.AggregateException Exception)
		{
			foreach(var e in Exception.InnerExceptions)
			{
				Debug.LogError(e);
				if(e is not NNAException && State.ImportOptions.AbortOnException)
				{
					throw Exception;
				}
			}
			State.Errors.Add(Exception);
		}
	}
}
