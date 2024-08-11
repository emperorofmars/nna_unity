using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using nna.processors;
using UnityEngine;

namespace nna
{
	public class NNAContext
	{
		public Dictionary<string, IProcessor> Processors { get; private set; }
		public GameObject Root { get; private set; }
		List<(string, Object)> NewObjects = new List<(string, Object)>();


		public NNAContext(GameObject Root, Dictionary<string, IProcessor> Processors) { this.Root = Root; this.Processors = Processors; }
		public NNAContext(GameObject Root) { this.Root = Root; this.Processors = NNARegistry.Processors; }


		public bool ContainsProcessor(string Type) { return Processors.ContainsKey(Type); }
		public bool ContainsProcessor(JObject Component) { return Processors.ContainsKey((string)ParseUtil.GetMulkikey(Component, "t", "type")); }

		public string GetType(JObject Component) { return (string)ParseUtil.GetMulkikey(Component, "t", "type"); }
		public IProcessor Get(string Type) { return Processors[Type]; }
		public IProcessor Get(JObject Component) { return Processors[(string)ParseUtil.GetMulkikey(Component, "t", "type")]; }
		
		public void Process(NNAContext Context, GameObject NNANode, JObject Json) { Get(Json).Process(Context, NNANode, Json); }

		public void AddObjectToAsset(string name, Object NewObject) { NewObjects.Add((name, NewObject)); }

		public List<(string Name, Object NewObject)> GetNewObjects() { return NewObjects; }
	}
}
