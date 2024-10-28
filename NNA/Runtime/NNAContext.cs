using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using nna.processors;
using UnityEngine;

namespace nna
{
	/// <summary>
	/// This class holds all the processors to be used for an import.
	/// The processors can be either automatically pulled from the NNARegistry based on the selected import context, or passed manually to the constructor.
	/// </summary>
	public class NNAContext
	{
		public NNAImportOptions ImportOptions { get; private set; }
		public Dictionary<string, IJsonProcessor> JsonProcessors { get; private set; }
		public Dictionary<string, INameProcessor> NameProcessors { get; private set; }
		public Dictionary<string, IGlobalProcessor> GlobalProcessors { get; private set; }
		public HashSet<string> IgnoreList { get; private set; }
		public GameObject Root { get; private set; }

		private readonly List<(string, Object)> NewObjects = new();

		public readonly Dictionary<string, (JObject Json, Transform Node)> Overrides = new();
		public readonly Dictionary<Transform, List<JObject>> ComponentMap = new();

		private List<Task> Tasks = new();
		public List<Transform> Trash = new();

		public NNAContext(GameObject Root, Dictionary<string, IJsonProcessor> JsonProcessors, Dictionary<string, INameProcessor> NameProcessors, Dictionary<string, IGlobalProcessor> GlobalProcessors, HashSet<string> IgnoreList, NNAImportOptions ImportOptions)
		{
			this.ImportOptions = ImportOptions;
			this.Root = Root;
			this.JsonProcessors = JsonProcessors;
			this.NameProcessors = NameProcessors;
			this.GlobalProcessors = GlobalProcessors;
			this.IgnoreList = IgnoreList;
		}

		public NNAContext(GameObject Root, NNAImportOptions ImportOptions)
		{
			this.ImportOptions = ImportOptions;
			this.Root = Root;
			this.JsonProcessors = NNARegistry.GetJsonProcessors(ImportOptions.SelectedContext);
			this.NameProcessors = NNARegistry.GetNameProcessors(ImportOptions.SelectedContext);
			this.GlobalProcessors = NNARegistry.GetGlobalProcessors(ImportOptions.SelectedContext);
			this.IgnoreList = NNARegistry.GetIgnoreList(ImportOptions.SelectedContext);
		}

		public bool ContainsJsonProcessor(string Type) { return JsonProcessors.ContainsKey(Type); }
		public bool ContainsJsonProcessor(JObject Component) { return JsonProcessors.ContainsKey((string)ParseUtil.GetMulkikey(Component, "t", "type")); }

		public JObject GetJsonComponent(Transform Node, string TypeName)
		{
			return ComponentMap[Node].Find(c => (string)c["t"] == TypeName) is var Json && Json != null ? Json : new JObject();
		}

		public string GetType(JObject Component) { return (string)ParseUtil.GetMulkikey(Component, "t", "type"); }
		public IJsonProcessor Get(string Type) { return JsonProcessors[Type]; }
		public IJsonProcessor Get(JObject Component) { return JsonProcessors[(string)ParseUtil.GetMulkikey(Component, "t", "type")]; }

		public void AddObjectToAsset(string name, Object NewObject) { NewObjects.Add((name, NewObject)); }

		public List<(string Name, Object NewObject)> GetNewObjects() { return NewObjects; }

		public void AddTask(Task Task) { this.Tasks.Add(Task); }
		public void AddTrash(Transform Trash) { this.Trash.Add(Trash); }
		public void AddTrash(IEnumerable<Transform> Trash) { this.Trash.AddRange(Trash); }

		public void RunTasks()
		{
			while(Tasks.Count > 0)
			{
				var taskset = Tasks;
				Tasks = new List<Task>();
				foreach(var task in taskset)
				{
					task.RunSynchronously();
					if(task.Exception != null) throw task.Exception;
				}
			}
			
			if(ImportOptions.RemoveNNAJson) foreach(var t in Trash)
			{
				if(t) Object.DestroyImmediate(t.gameObject);
			}
		}
	}
}
