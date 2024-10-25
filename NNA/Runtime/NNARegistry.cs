
using System.Collections.Generic;
using nna.processors;

namespace nna
{
	public static class NNARegistry
	{
		public const string DefaultContext = "default";

		// Type -> Context -> IJsonProcessor
		// A `null` Context is the default context applicable to all context's, unless a IJsonProcessor for a specific context is registered.
		public static readonly Dictionary<string, Dictionary<string, IJsonProcessor>> DefaultJsonProcessors = new() {
			{TwistBoneJsonProcessor._Type, new Dictionary<string, IJsonProcessor> {{DefaultContext, new TwistBoneJsonProcessor()}}},
			{HumanoidMappingJsonProcessor._Type, new Dictionary<string, IJsonProcessor> {{DefaultContext, new HumanoidMappingJsonProcessor()}}},
		};

		// Type -> Context -> INameProcessor
		// A `null` Context is the default context applicable to all context's, unless a INameProcessor for a specific context is registered.
		public static readonly Dictionary<string, Dictionary<string, INameProcessor>> DefaultNameProcessors = new() {
			{TwistBoneNameProcessor._Type, new Dictionary<string, INameProcessor> {{DefaultContext, new TwistBoneNameProcessor()}}},
			{HumanoidMappingNameProcessor._Type, new Dictionary<string, INameProcessor> {{DefaultContext, new HumanoidMappingNameProcessor()}}},
		};
		
		// Type -> Context -> IGlobalProcessor
		// A `null` Context is the default context applicable to all context's, unless a INameProcessor for a specific context is registered.
		public static readonly Dictionary<string, Dictionary<string, IGlobalProcessor>> RegisteredGlobalProcessors = new();
		
		private static readonly Dictionary<string, Dictionary<string, IJsonProcessor>> RegisteredJsonProcessors = new();
		private static readonly Dictionary<string, Dictionary<string, INameProcessor>> RegisteredNameProcessors = new();

		private static readonly Dictionary<string, HashSet<string>> IgnoreList = new();

		public static void RegisterIgnoredJsonType(string TypeName, string Context = DefaultContext)
		{
			if(IgnoreList.ContainsKey(Context))
			{
				if(!IgnoreList[Context].Contains(TypeName)) IgnoreList[Context].Add(TypeName);
			}
			else
			{
				IgnoreList.Add(Context, new HashSet<string>{TypeName});
			}
		}

		public static HashSet<string> GetIgnoreList(string Context)
		{
			return new HashSet<string>(IgnoreList.ContainsKey(Context) ? IgnoreList[Context] : new HashSet<string>());
		}
		
		public static Dictionary<string, Dictionary<string, IJsonProcessor>> JsonProcessors { get {
			var ret = new Dictionary<string, Dictionary<string, IJsonProcessor>>();
			foreach(var entry in DefaultJsonProcessors)
			{
				ret.Add(entry.Key, new Dictionary<string, IJsonProcessor>(entry.Value));
			}
			foreach(var entry in RegisteredJsonProcessors)
			{
				foreach(var contextEntry in entry.Value)
				{
					MergeEntryIntoProcessorDict(ret, entry.Key, contextEntry.Key, contextEntry.Value);
				}
			}
			return ret;
		}}
		
		public static Dictionary<string, Dictionary<string, INameProcessor>> NameProcessors { get {
			var ret = new Dictionary<string, Dictionary<string, INameProcessor>>();
			foreach(var entry in DefaultNameProcessors)
			{
				ret.Add(entry.Key, new Dictionary<string, INameProcessor>(entry.Value));
			}
			foreach(var entry in RegisteredNameProcessors)
			{
				foreach(var contextEntry in entry.Value)
				{
					MergeEntryIntoProcessorDict(ret, entry.Key, contextEntry.Key, contextEntry.Value);
				}
			}
			return ret;
		}}

		public static Dictionary<string, Dictionary<string, IGlobalProcessor>> GlobalProcessors { get {
			var ret = new Dictionary<string, Dictionary<string, IGlobalProcessor>>();
			foreach(var entry in RegisteredGlobalProcessors)
			{
				ret.Add(entry.Key, new Dictionary<string, IGlobalProcessor>(entry.Value));
			}
			return ret;
		}}

		public static Dictionary<string, IJsonProcessor> GetJsonProcessors(string Context = DefaultContext)
		{
			var ret = new Dictionary<string, IJsonProcessor>();
			foreach(var entry in JsonProcessors)
			{
				if(entry.Value.ContainsKey(Context))
				{
					ret.Add(entry.Key, entry.Value[Context]);
				}
				else if(entry.Value.ContainsKey(DefaultContext))
				{
					ret.Add(entry.Key, entry.Value[DefaultContext]);
				}
			}
			return ret;
		}

		public static Dictionary<string, INameProcessor> GetNameProcessors(string Context = DefaultContext)
		{
			var ret = new Dictionary<string, INameProcessor>();
			foreach(var entry in NameProcessors)
			{
				if(entry.Value.ContainsKey(Context))
				{
					ret.Add(entry.Key, entry.Value[Context]);
				}
				else if(entry.Value.ContainsKey(DefaultContext))
				{
					ret.Add(entry.Key, entry.Value[DefaultContext]);
				}
			}
			return ret;
		}
		
		public static Dictionary<string, IGlobalProcessor> GetGlobalProcessors(string Context = DefaultContext)
		{
			var ret = new Dictionary<string, IGlobalProcessor>();
			foreach(var entry in GlobalProcessors)
			{
				if(entry.Value.ContainsKey(Context))
				{
					ret.Add(entry.Key, entry.Value[Context]);
				}
				else if(entry.Value.ContainsKey(DefaultContext))
				{
					ret.Add(entry.Key, entry.Value[DefaultContext]);
				}
			}
			return ret;
		}

		public static List<string> GetAvaliableContexts()
		{
			var ret = new List<string>();
			foreach(var entry in JsonProcessors)
			{
				foreach(var contextEntry in entry.Value)
				{
					if(!ret.Contains(contextEntry.Key)) ret.Add(contextEntry.Key);
				}
			}
			foreach(var entry in NameProcessors)
			{
				foreach(var contextEntry in entry.Value)
				{
					if(!ret.Contains(contextEntry.Key)) ret.Add(contextEntry.Key);
				}
			}
			foreach(var entry in GlobalProcessors)
			{
				foreach(var contextEntry in entry.Value)
				{
					if(!ret.Contains(contextEntry.Key)) ret.Add(contextEntry.Key);
				}
			}
			return ret;
		}

		public static void RegisterJsonProcessor(IJsonProcessor Processor, string Type, string Context = DefaultContext)
		{
			MergeEntryIntoProcessorDict(RegisteredJsonProcessors, Type, Context, Processor);
		}

		public static void RegisterNameProcessor(INameProcessor Processor, string Type, string Context = DefaultContext)
		{
			MergeEntryIntoProcessorDict(RegisteredNameProcessors, Type, Context, Processor);
		}

		public static void RegisterGlobalProcessor(IGlobalProcessor Processor, string Type, string Context = DefaultContext)
		{
			MergeEntryIntoProcessorDict(RegisteredGlobalProcessors, Type, Context, Processor);
		}

		private static void MergeEntryIntoProcessorDict<T>(Dictionary<string, Dictionary<string, T>> Dict, string Type, string Context, T Processor)
		{
			if(Dict.ContainsKey(Type))
			{
				var existing = Dict[Type];
				if(existing.ContainsKey(Context))
				{
					existing[Context] = Processor;
				}
				else
				{
					existing.Add(Context, Processor);
				}
			}
			else
			{
				Dict.Add(Type, new Dictionary<string, T> {{Context, Processor}});
			}
		}
	}
}
