
using System.Collections.Generic;
using nna.processors;

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

		public static IProcessor Get(string Type) { return Processors[Type]; }
	}
}
