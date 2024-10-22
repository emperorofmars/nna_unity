using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using nna.processors;
using UnityEngine;

namespace nna
{
	public class NNAImportOptions
	{
		public NNAImportOptions() {}

		public static NNAImportOptions Parse(string JSON)
		{
			var nnaImportOptions = new NNAImportOptions();
			try
			{
				nnaImportOptions = JsonUtility.FromJson<NNAImportOptions>(JSON);
			}
			catch
			{
			}
			return nnaImportOptions;
		}

		[SerializeField]
		private string _SelectedContext = NNARegistry.DefaultContext;
		public string SelectedContext {get => _SelectedContext; set { if(value != _SelectedContext) Modified = true; _SelectedContext = value; }}

		[SerializeField]
		private bool _RemoveNNAJson = true;
		public bool RemoveNNAJson {get => _RemoveNNAJson; set { if(value != _RemoveNNAJson) Modified = true; _RemoveNNAJson = value; }}

		[IgnoreDataMember]
		public bool Modified {get; private set;} = false;
	}
	
	public class NNAContext
	{
		public NNAImportOptions ImportOptions { get; private set; }
		public Dictionary<string, IJsonProcessor> JsonProcessors { get; private set; }
		public Dictionary<string, INameProcessor> NameProcessors { get; private set; }
		public Dictionary<string, IGlobalProcessor> GlobalProcessors { get; private set; }
		public GameObject Root { get; private set; }
		private readonly List<(string, Object)> NewObjects = new();

		private List<Task> Tasks = new();
		public List<Transform> Trash = new();

		public NNAContext(GameObject Root, Dictionary<string, IJsonProcessor> JsonProcessors, Dictionary<string, INameProcessor> NameProcessors, Dictionary<string, IGlobalProcessor> GlobalProcessors, NNAImportOptions ImportOptions)
		{
			this.ImportOptions = ImportOptions;
			this.Root = Root;
			this.JsonProcessors = JsonProcessors;
			this.NameProcessors = NameProcessors;
			this.GlobalProcessors = GlobalProcessors;
		}

		public NNAContext(GameObject Root, NNAImportOptions ImportOptions)
		{
			this.ImportOptions = ImportOptions;
			this.Root = Root;
			this.JsonProcessors = NNARegistry.GetJsonProcessors(ImportOptions.SelectedContext);
			this.NameProcessors = NNARegistry.GetNameProcessors(ImportOptions.SelectedContext);
			this.GlobalProcessors = NNARegistry.GetGlobalProcessors(ImportOptions.SelectedContext);
		}

		public bool ContainsJsonProcessor(string Type) { return JsonProcessors.ContainsKey(Type); }
		public bool ContainsJsonProcessor(JObject Component) { return JsonProcessors.ContainsKey((string)ParseUtil.GetMulkikey(Component, "t", "type")); }

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
				}
			}
			
			if(ImportOptions.RemoveNNAJson) foreach(var t in Trash)
			{
				if(t) Object.DestroyImmediate(t.gameObject);
			}
		}
	}
}
