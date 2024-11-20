using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace nna.UnityToNNAUtils
{
	[System.Serializable]
	public struct SerializerResult
	{
		public string NNAType;
		public bool IsJsonComplete;
		public string DeviatingJsonType;
		public string JsonTargetNode;
		public string JsonResult;
		public bool IsNameComplete;
		public string DeviatingNameType;
		public string NameTargetNode;
		public string NameResult;
		public UnityEngine.Object Origin;
	}

	/// <summary>
	/// NNA Json Serializers manually convert Unity Objects into NNA compatible Json.
	/// </summary>
	/// Once C# 11 becomes available in Unity, convert to using static virtual interface members (https://learn.microsoft.com/en-us/dotnet/csharp/whats-new/tutorials/static-virtual-interface-members)
	public interface INNASerializer
	{
		System.Type Target {get;}
		List<SerializerResult> Serialize(UnityEngine.Object UnityObject);
	}

	public static class RunNNASerializer
	{
		public static List<SerializerResult> Run(UnityEngine.Object Target)
		{
			var ret = new List<SerializerResult>();
			if(Target != null)
			{
				if(Target.GetType() == typeof(GameObject)) ret.AddRange(Run((GameObject)Target));
				else if(Target.GetType() == typeof(Component)) ret.AddRange(Run((Component)Target));
				// TODO Resources Maybe?
			}
			return ret;
		}
		public static List<SerializerResult> Run(Component Target)
		{
			var ret = new List<SerializerResult>();
			if(Target != null)
			{
				foreach(var serializer in NNAExportRegistry.Serializers.FindAll(s => Target.GetType() == s.Target))
				{
					ret.AddRange(serializer.Serialize(Target));
				}
			}
			return ret;
		}
		public static List<SerializerResult> Run(GameObject Target)
		{
			var ret = new List<SerializerResult>();
			if(Target != null)
			{
				foreach(var t in Target.transform.GetComponentsInChildren<Component>())
				{
					if(t.GetType() == typeof(Transform)) continue;
					ret.AddRange(Run(t));
				}
			}
			return ret;
		}

		public static bool ValidateResult(SerializerResult Result)
		{
			return false;
		}

		public static string CreateSetupString(List<SerializerResult> Results)
		{
			var ret = new JArray();

			foreach(var result in Results)
			{
				var jsonInstruction = new JObject {
					{"type", string.IsNullOrWhiteSpace(result.DeviatingJsonType) ? result.NNAType : result.DeviatingJsonType},
					{"target", result.JsonTargetNode},
					{"data", JObject.Parse(result.JsonResult)}
				};
				ret.Add(jsonInstruction);
			}

			return ret.ToString(Newtonsoft.Json.Formatting.None);
		}
	}
}
