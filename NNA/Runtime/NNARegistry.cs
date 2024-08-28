
using System.Collections.Generic;
using nna.processors;

namespace nna
{
	public static class NNARegistry
	{

		// Type -> IProcessor
		public static readonly Dictionary<string, IProcessor> DefaultProcessors = new() {
			{TwistBone._Type,  new TwistBone()},
			{HumanoidMapping._Type, new HumanoidMapping()},
		};
		private static readonly Dictionary<string, IProcessor> RegisteredProcessors = new();
		
		public static Dictionary<string, IProcessor> Processors { get {
			var ret = new Dictionary<string, IProcessor>(DefaultProcessors);
			foreach(var entry in RegisteredProcessors)
			{
				if(ret.ContainsKey(entry.Key)) ret[entry.Key] = entry.Value;
				else ret.Add(entry.Key, entry.Value);
			}
			return ret;
		}}

		public static void RegisterProcessor(IProcessor Processor, string Type)
		{
			if(RegisteredProcessors.ContainsKey(Type)) RegisteredProcessors[Type] = Processor;
			else RegisteredProcessors.Add(Type, Processor);
		}
	}
}
