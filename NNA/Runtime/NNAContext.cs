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

		private readonly Dictionary<string, System.Object> ResultsByID = new();

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

		public void AddResultById(string ID, System.Object Result) { ResultsByID.Add(ID, Result); }
		public System.Object GetResultById(string ID) { return ResultsByID.GetValueOrDefault(ID); }
		
		public (JObject Component, UnityEngine.Transform Node) GetJsonComponentById(string ID) { return ImportState.JsonComponentsByID.GetValueOrDefault(ID); }
		public UnityEngine.Object GetNameComponentById(string ID) { return ImportState.NameComponentsByID.GetValueOrDefault(ID); }
		public List<(JObject Component, UnityEngine.Transform Node)> GetJsonComponentByType(string Type) { return ImportState.JsonComponentsByType.GetValueOrDefault(Type); }
		public List<UnityEngine.Transform> GetNameComponentByType(string Type) { return ImportState.NameComponentsByType.GetValueOrDefault(Type); }
		public ImmutableList<JObject> GetJsonComponentsByNode(Transform Node) { return ImportState.JsonComponentByNode.ContainsKey(Node) ? ImportState.JsonComponentByNode[Node].ToImmutableList() : ImmutableList<JObject>.Empty; }

		public bool IsOverridden(string Id) { return ImportState.Overrides.ContainsKey(Id); }
		public bool IsIgnored(string Type) { return ImportState.IgnoreList.Contains(Type); }
		public (JObject Json, Transform Node) GetOverride(string Id) { return ImportState.Overrides.GetValueOrDefault(Id); }

		public void AddObjectToAsset(string name, Object NewObject) { ImportState.NewObjects.Add((name, NewObject)); }
		public void AddRemap(Object Original, Object New) { ImportState.Remaps.Add((Original, New)); }

		public NNAMeta GetMeta() {
			return ImportState.Meta;
		}
		public string GetMetaCustomValue(string Key) {
			return ImportState.Meta.AdditionalProperties.FirstOrDefault(e => e.Key == Key)?.Value;
		}

		public void AddTask(Task Task) { ImportState.Tasks.Add(Task); }
		public void AddTrash(Transform Trash) { ImportState.Trash.Add(Trash); }
		public void AddTrash(IEnumerable<Transform> Trash) { ImportState.Trash.AddRange(Trash); }
	}
}
