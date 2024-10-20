using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using nna.processors;
using UnityEngine;

namespace nna
{
	public class NNAContext
	{
		public Dictionary<string, IJsonProcessor> JsonProcessors { get; private set; }
		public Dictionary<string, INameProcessor> NameProcessors { get; private set; }
		public GameObject Root { get; private set; }
		private readonly List<(string, Object)> NewObjects = new();

		private List<Task> Tasks = new();

		public NNAContext(GameObject Root, Dictionary<string, IJsonProcessor> JsonProcessors, Dictionary<string, INameProcessor> NameProcessors) { this.Root = Root; this.JsonProcessors = JsonProcessors; this.NameProcessors = NameProcessors; }
		public NNAContext(GameObject Root, string Context = NNARegistry.DefaultContext) { this.Root = Root; this.JsonProcessors = NNARegistry.GetJsonProcessors(Context); this.NameProcessors = NNARegistry.GetNameProcessors(Context); }

		public bool ContainsJsonProcessor(string Type) { return JsonProcessors.ContainsKey(Type); }
		public bool ContainsJsonProcessor(JObject Component) { return JsonProcessors.ContainsKey((string)ParseUtil.GetMulkikey(Component, "t", "type")); }

		public string GetType(JObject Component) { return (string)ParseUtil.GetMulkikey(Component, "t", "type"); }
		public IJsonProcessor Get(string Type) { return JsonProcessors[Type]; }
		public IJsonProcessor Get(JObject Component) { return JsonProcessors[(string)ParseUtil.GetMulkikey(Component, "t", "type")]; }

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
