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
			return ValidateJsonResult(Result) || ValidateNameResult(Result);
		}

		public static bool ValidateJsonResult(SerializerResult Result)
		{
			return
				!string.IsNullOrWhiteSpace(Result.JsonResult)
				&& !string.IsNullOrWhiteSpace(Result.NNAType)
				&& !string.IsNullOrWhiteSpace(Result.JsonTargetNode);
		}

		public static bool ValidateNameResult(SerializerResult Result)
		{
			return
				!string.IsNullOrWhiteSpace(Result.NameResult)
				&& !string.IsNullOrWhiteSpace(Result.NNAType)
				&& !string.IsNullOrWhiteSpace(Result.NameTargetNode);
		}

		public static bool CheckIfPrefersJson(SerializerResult Result, bool DefaultJsonPreference = false)
		{
			var jsonComplete = ValidateJsonResult(Result) && Result.IsJsonComplete;
			var nameComplete = ValidateNameResult(Result) && Result.IsNameComplete;
			return jsonComplete && (DefaultJsonPreference || !nameComplete);
		}

		public static string CreateSetupString(List<SerializerResult> Results, bool DefaultJsonPreference = false)
		{
			var ret = new JArray();

			foreach(var result in Results)
			{
				if(CheckIfPrefersJson(result, DefaultJsonPreference))
				{
					var jsonInstruction = new JObject {
						{"instruction_type", "json"},
						{"nna_type", string.IsNullOrWhiteSpace(result.DeviatingJsonType) ? result.NNAType : result.DeviatingJsonType},
						{"target", result.JsonTargetNode},
						{"data", JObject.Parse(result.JsonResult)}
					};
					ret.Add(jsonInstruction);
				}
				else if(ValidateNameResult(result))
				{
					var nameInstruction = new JObject {
						{"instruction_type", "name"},
						{"nna_type", string.IsNullOrWhiteSpace(result.DeviatingNameType) ? result.NNAType : result.DeviatingNameType},
						{"target", result.NameTargetNode},
						{"data", result.NameResult}
					};
					ret.Add(nameInstruction);
				}
			}

			return ret.ToString(Newtonsoft.Json.Formatting.None);
		}
	}
}
