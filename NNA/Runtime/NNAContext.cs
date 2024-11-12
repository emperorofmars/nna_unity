using System.Collections.Generic;
using System.Collections.Immutable;
using System.Runtime.Remoting.Contexts;
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
		public readonly NNAImportOptions ImportOptions;
		public readonly ImmutableDictionary<string, IJsonProcessor> JsonProcessors;
		public readonly ImmutableDictionary<string, INameProcessor> NameProcessors;
		public readonly ImmutableDictionary<string, IGlobalProcessor> GlobalProcessors;
		public readonly ImmutableHashSet<string> IgnoreList;
		public readonly GameObject Root;

		private readonly List<(string, Object)> NewObjects = new();
		public readonly List<(Object, Object)> Remaps = new();

		private readonly Dictionary<string, (JObject Json, Transform Node)> Overrides = new();
		private readonly Dictionary<Transform, List<JObject>> ComponentMap = new();

		private List<Task> Tasks = new();
		public readonly List<Transform> Trash = new();

		private NNAMeta _Meta;
		public NNAMeta Meta => _Meta;

		public NNAContext(GameObject Root, Dictionary<string, IJsonProcessor> JsonProcessors, Dictionary<string, INameProcessor> NameProcessors, Dictionary<string, IGlobalProcessor> GlobalProcessors, HashSet<string> IgnoreList, NNAImportOptions ImportOptions)
		{
			this.ImportOptions = ImportOptions;
			this.Root = Root;
			this.JsonProcessors = JsonProcessors.ToImmutableDictionary();
			this.NameProcessors = NameProcessors.ToImmutableDictionary();
			this.GlobalProcessors = GlobalProcessors.ToImmutableDictionary();
			this.IgnoreList = IgnoreList.ToImmutableHashSet();
		}

		public NNAContext(GameObject Root, NNAImportOptions ImportOptions)
		{
			this.ImportOptions = ImportOptions;
			this.Root = Root;
			this.JsonProcessors = NNARegistry.GetJsonProcessors(ImportOptions.SelectedContext).ToImmutableDictionary();
			this.NameProcessors = NNARegistry.GetNameProcessors(ImportOptions.SelectedContext).ToImmutableDictionary();
			this.GlobalProcessors = NNARegistry.GetGlobalProcessors(ImportOptions.SelectedContext).ToImmutableDictionary();
			this.IgnoreList = NNARegistry.GetIgnoreList(ImportOptions.SelectedContext).ToImmutableHashSet();
		}

		public bool ContainsJsonProcessor(string Type) { return JsonProcessors.ContainsKey(Type); }
		public bool ContainsJsonProcessor(JObject Component) { return JsonProcessors.ContainsKey((string)ParseUtil.GetMulkikey(Component, "t", "type")); }

		public JObject GetComponentOrDefault(Transform Node, string TypeName)
		{
			return ComponentMap[Node].Find(c => (string)c["t"] == TypeName) is var Json && Json != null ? Json : new JObject();
		}

		public JObject GetComponent(Transform Node, string TypeName)
		{
			return ComponentMap[Node].Find(c => (string)c["t"] == TypeName);
		}

		public string GetType(JObject Component) { return (string)ParseUtil.GetMulkikey(Component, "t", "type"); }
		public IJsonProcessor Get(string Type) { return JsonProcessors[Type]; }
		public IJsonProcessor Get(JObject Component) { return JsonProcessors[(string)ParseUtil.GetMulkikey(Component, "t", "type")]; }

		public void AddComponentMap(Transform Node, List<JObject> Components)
		{
			if(ComponentMap.ContainsKey(Node)) ComponentMap[Node].AddRange(Components);
			else ComponentMap.Add(Node, Components);
		}
		public ImmutableList<JObject> GetComponents(Transform Node) { return ComponentMap.ContainsKey(Node) ? ComponentMap[Node].ToImmutableList() : ImmutableList<JObject>.Empty; }

		public void AddOverride(string Id, JObject Json, Transform Node) { Overrides.Add(Id, (Json, Node)); }
		public bool IsOverridden(string Id) { return Overrides.ContainsKey(Id); }
		public bool IsIgnored(string Type) { return IgnoreList.Contains(Type); }
		public (JObject Json, Transform Node) GetOverride(string Id) { return Overrides[Id]; }

		public void AddObjectToAsset(string name, Object NewObject) { NewObjects.Add((name, NewObject)); }

		public void SetNNAMeta(NNAMeta Meta) {
			AddObjectToAsset(Meta.name, Meta);
			_Meta = Meta;
		}

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
