using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace nna
{
	/// <summary>
	/// This class holds all the information processors need for execution.
	/// The processors can be either automatically pulled from the NNARegistry based on the selected import context, or passed manually to the constructor.
	/// </summary>
	public class NNAContext
	{
		public readonly NNAImportOptions ImportOptions;
		public readonly GameObject Root;
		private readonly NNAImportState ImportState;

		private readonly Dictionary<string, List<object>> ResultsById = new();

		private readonly Dictionary<string, object> Messages = new();

		public NNAContext(
			GameObject Root,
			NNAImportOptions ImportOptions,
			NNAImportState ImportState
		){
			this.Root = Root;
			this.ImportOptions = ImportOptions;
			this.ImportState = ImportState;
		}

		public JObject GetJsonComponentOrDefault(Transform Node, string TypeName)
		{
			return ImportState.JsonComponentByNode[Node].Find(c => (string)c["t"] == TypeName) is var Json && Json != null ? Json : new JObject();
		}

		public JObject GetJsonComponentByNode(Transform Node, string TypeName)
		{
			return ImportState.JsonComponentByNode[Node].Find(c => (string)c["t"] == TypeName);
		}

		public void AddResultById(string Id, object Result) {
			if(!string.IsNullOrWhiteSpace(Id))
			{
				if(ResultsById.ContainsKey(Id)) ResultsById[Id].Add(Result);
				else ResultsById.Add(Id, new List<object> {Result});
			}
		}
		public List<object> GetResultsById(string Id) {
			return ResultsById.GetValueOrDefault(Id);
		}

		public void AddMessage(string Id, object Message) {
			if(!string.IsNullOrWhiteSpace(Id))
			{
				if(Messages.ContainsKey(Id)) ResultsById[Id].Add(Message);
				else Messages.Add(Id, Message);
			}
		}
		public bool HasMessage(string Id) {
			return Messages.ContainsKey(Id);
		}
		public T GetMessage<T>(string Id) {
			return (T)Messages[Id];
		}
		public object GetMessage(string Id) {
			return Messages[Id];
		}

		public (JObject Component, Transform Node) GetJsonComponentById(string Id) { return ImportState.JsonComponentsById.GetValueOrDefault(Id); }
		public Object GetNameComponentById(string Id) { return ImportState.NameComponentsById.GetValueOrDefault(Id); }
		public List<(JObject Component, Transform Node)> GetJsonComponentByType(string Type) { return ImportState.JsonComponentsByType.GetValueOrDefault(Type, new List<(JObject Component, Transform Node)>()); }
		public (JObject Component, Transform Node) GetOnlyJsonComponentByType(string Type)
		{
			return GetOnlyJsonComponentByType(Type, (null, null));
		}
		public (JObject Component, Transform Node) GetOnlyJsonComponentByType(string Type, (JObject, Transform) Default)
		{
			var list = ImportState.JsonComponentsByType.GetValueOrDefault(Type);
			return list.Count == 1 ? list[0] : Default;
		}
		public List<Transform> GetNameComponentByType(string Type) { return ImportState.NameComponentsByType.GetValueOrDefault(Type); }
		public ImmutableList<JObject> GetJsonComponentsByNode(Transform Node) { return ImportState.JsonComponentByNode.ContainsKey(Node) ? ImportState.JsonComponentByNode[Node].ToImmutableList() : ImmutableList<JObject>.Empty; }

		public bool IsOverridden(string Id) { return ImportState.OverriddenComponents.ContainsKey(Id); }
		public bool IsIgnored(string Type) { return ImportState.IgnoreList.Contains(Type); }
		public (JObject Json, Transform Node) GetOverriddenComponent(string Id) { return ImportState.OverriddenComponents.GetValueOrDefault(Id); }

		public void AddObjectToAsset(string name, Object NewObject) { ImportState.NewObjects.Add((name, NewObject)); }
		public void AddRemap(Object Original, Object New) { ImportState.Remaps.Add((Original, New)); }

		public NNAMeta GetMeta() {
			return ImportState.Meta;
		}
		public string GetMetaCustomValue(string Key) {
			return ImportState.Meta.CustomProperties.FirstOrDefault(e => e.Key == Key)?.Value;
		}

		public void AddTask(Task Task) { ImportState.Tasks.Add(Task); }
		public void AddTrash(Transform Trash) { ImportState.Trash.Add(Trash); }
		public void AddTrash(IEnumerable<Transform> Trash) { ImportState.Trash.AddRange(Trash); }
	}
}
