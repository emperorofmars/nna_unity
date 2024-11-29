using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using nna.processors;
using UnityEngine;

namespace nna
{
	public struct ImportResult
	{
		public List<(string Name, UnityEngine.Object Object)> NewObjects;
		public List<(UnityEngine.Object Original, UnityEngine.Object New)> Remaps;
	}

	/// <summary>
	/// Contains the state for the import process.
	/// </summary>
	public class NNAImportState
	{
		public readonly NNAImportOptions ImportOptions;
		public readonly ImmutableDictionary<string, IJsonProcessor> JsonProcessors;
		public readonly ImmutableDictionary<string, INameProcessor> NameProcessors;
		public readonly ImmutableDictionary<string, IGlobalProcessor> GlobalProcessors;
		public readonly ImmutableHashSet<string> IgnoreList;
		public readonly GameObject Root;

		public readonly Dictionary<string, (JObject Json, Transform Node)> Overrides = new();
		public readonly Dictionary<string, List<(JObject Component, Transform Node)>> JsonComponentsByType = new();
		public readonly Dictionary<string, List<Transform>> NameComponentsByType = new();
		public readonly Dictionary<string, (JObject Component, Transform Node)> JsonComponentsByID = new();
		public readonly Dictionary<string, Transform> NameComponentsByID = new();
		public readonly Dictionary<Transform, List<JObject>> JsonComponentByNode = new();

		public readonly List<(string, Object)> NewObjects = new();
		public readonly List<(Object, Object)> Remaps = new();

		public readonly Dictionary<uint, List<Task>> ProcessOrderMap = new();
		public List<Task> Tasks = new();
		public readonly List<Transform> Trash = new();

		public NNAMeta Meta;

		public readonly List<System.AggregateException> Errors = new();

		public NNAImportState(
			GameObject Root,
			NNAImportOptions ImportOptions,
			Dictionary<string, IJsonProcessor> JsonProcessors = null,
			Dictionary<string, INameProcessor> NameProcessors = null,
			Dictionary<string, IGlobalProcessor> GlobalProcessors = null,
			HashSet<string> IgnoreList = null
		){
			this.ImportOptions = ImportOptions;
			this.Root = Root;
			this.JsonProcessors = JsonProcessors != null ? JsonProcessors.ToImmutableDictionary() : NNARegistry.GetJsonProcessors(ImportOptions.SelectedContext).ToImmutableDictionary();
			this.NameProcessors = NameProcessors != null ? NameProcessors.ToImmutableDictionary() : NNARegistry.GetNameProcessors(ImportOptions.SelectedContext).ToImmutableDictionary();
			this.GlobalProcessors = GlobalProcessors != null ? GlobalProcessors.ToImmutableDictionary() : NNARegistry.GetGlobalProcessors(ImportOptions.SelectedContext).ToImmutableDictionary();
			this.IgnoreList = IgnoreList != null ? IgnoreList.ToImmutableHashSet() : NNARegistry.GetIgnoreList(ImportOptions.SelectedContext).ToImmutableHashSet();
		}

		public bool ContainsJsonProcessor(string Type) { return JsonProcessors.ContainsKey(Type); }
		public bool ContainsJsonProcessor(JObject Component) { return JsonProcessors.ContainsKey((string)ParseUtil.GetMulkikey(Component, "t", "type")); }

		public string GetType(JObject Component) { return (string)ParseUtil.GetMulkikey(Component, "t", "type"); }
		public string GetID(JObject Component) { return (string)ParseUtil.GetMulkikey(Component, "id"); }
		public IJsonProcessor GetJsonProcessor(JObject Component) { return JsonProcessors[(string)ParseUtil.GetMulkikey(Component, "t", "type")]; }

		public void AddComponentMap(Transform Node, List<JObject> Components)
		{
			if(JsonComponentByNode.ContainsKey(Node)) JsonComponentByNode[Node].AddRange(Components);
			else JsonComponentByNode.Add(Node, Components);
			foreach(var component in Components)
			{
				var id = GetID(component);
				if(id != null) JsonComponentsByID.Add(id, (component, Node));

				var type = GetType(component);
				if(JsonComponentsByType.ContainsKey(type)) JsonComponentsByType[type].Add((component, Node));
				else JsonComponentsByType.Add(type, new List<(JObject Component, Transform Node)> {(component, Node)});
			}
		}
		public void RegisterNameComponent(Transform Node, string Type, uint DefinitionStartIndex)
		{
			var id = ParseUtil.GetNameComponentId(Node.name, DefinitionStartIndex);
			if(id != null) NameComponentsByID.Add(id, Node);
			if(NameComponentsByType.ContainsKey(Type)) NameComponentsByType[Type].Add(Node);
			else NameComponentsByType.Add(Type, new List<Transform> {Node});
		}
		
		public ImmutableList<JObject> GetJsonComponentsByNode(Transform Node) { return JsonComponentByNode.ContainsKey(Node) ? JsonComponentByNode[Node].ToImmutableList() : ImmutableList<JObject>.Empty; }

		public void AddOverride(string Id, JObject Json, Transform Node) { Overrides.Add(Id, (Json, Node)); }
		public bool IsOverridden(string Id) { return Overrides.ContainsKey(Id); }
		public bool IsIgnored(string Type) { return IgnoreList.Contains(Type); }

		public void AddObjectToAsset(string name, Object NewObject) { NewObjects.Add((name, NewObject)); }

		public void SetNNAMeta(NNAMeta Meta) {
			AddObjectToAsset(Meta.name, Meta);
			this.Meta = Meta;
		}

		public void AddProcessorTask(uint Order, Task Task)
		{
			if(ProcessOrderMap.ContainsKey(Order)) ProcessOrderMap[Order].Add(Task);
			else ProcessOrderMap.Add(Order, new List<Task> {Task});
		}

		public void AddTrash(Transform Trash) { this.Trash.Add(Trash); }
		public void AddTrash(IEnumerable<Transform> Trash) { this.Trash.AddRange(Trash); }
	}
}
