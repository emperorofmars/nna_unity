
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using nna.processors;
using UnityEngine;

namespace nna
{
	public static class NNARegistry
	{
		public static readonly Dictionary<string, IProcessor> DefaultProcessors = new Dictionary<string, IProcessor>() {
			{TwistBone._Type, new TwistBone()}
		};
		private static Dictionary<string, IProcessor> RegisteredProcessors = new Dictionary<string, IProcessor>();
		
		public static Dictionary<string, IProcessor> Processors { get => CollectionUtil.Combine(DefaultProcessors, RegisteredProcessors); }

		public static void RegisterProcessors(string type, IProcessor Processor) { RegisteredProcessors.Add(type, Processor); }

		public static bool ContainsProcessor(string Type) { return Processors.ContainsKey(Type); }
		public static bool ContainsProcessor(JObject Component) { return Processors.ContainsKey((string)ParseUtil.GetMulkikey(Component, "t", "type")); }

		public static string GetType(JObject Component) { return (string)ParseUtil.GetMulkikey(Component, "t", "type"); }
		public static IProcessor Get(string Type) { return Processors[Type]; }
		public static IProcessor Get(JObject Component) { return Processors[(string)ParseUtil.GetMulkikey(Component, "t", "type")]; }
		
		public static void Process(GameObject Root, GameObject NNANode, JObject Json) { Get(Json).Process(Root, NNANode, Json); }
	}
}
