using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using nna.processors;
using UnityEngine;

namespace nna
{
	public class NNAContext
	{
		public Dictionary<string, IProcessor> Processors { get; private set; }
		public GameObject Root { get; private set; }
		private readonly List<(string, Object)> NewObjects = new();

		private List<Task> Tasks = new();

		public NNAContext(GameObject Root, Dictionary<string, IProcessor> Processors) { this.Root = Root; this.Processors = Processors; }
		public NNAContext(GameObject Root, string Context = NNARegistry.DefaultContext) { this.Root = Root; this.Processors = NNARegistry.GetProcessors(Context); }

		public bool ContainsProcessor(string Type) { return Processors.ContainsKey(Type); }
		public bool ContainsProcessor(JObject Component) { return Processors.ContainsKey((string)ParseUtil.GetMulkikey(Component, "t", "type")); }

		public string GetType(JObject Component) { return (string)ParseUtil.GetMulkikey(Component, "t", "type"); }
		public IProcessor Get(string Type) { return Processors[Type]; }
		public IProcessor Get(JObject Component) { return Processors[(string)ParseUtil.GetMulkikey(Component, "t", "type")]; }

		public void AddObjectToAsset(string name, Object NewObject) { NewObjects.Add((name, NewObject)); }

		public List<(string Name, Object NewObject)> GetNewObjects() { return NewObjects; }

		public void AddTask(Task Task) { this.Tasks.Add(Task); }

		public void RunTasks()
		{
			while(Tasks.Count > 0)
			{
				var taskset = Tasks;
				Tasks = new List<Task>();
				foreach(var task in taskset)
				{
					task.RunSynchronously();
				}
			}
		}
	}
}
