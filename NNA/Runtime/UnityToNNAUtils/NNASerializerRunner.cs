using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace nna.UnityToNNAUtils
{
	public static class RunNNASerializer
	{
		public static List<SerializerResult> Run(Object Target, NNASerializerContext Context = null)
		{
			var ret = new List<SerializerResult>();
			if(Target != null)
			{
				if(Target is GameObject) ret.AddRange(Run((GameObject)Target));
				else if(Target is Component) ret.AddRange(Run((Component)Target));
				// TODO Resources Maybe?
			}
			return ret;
		}

		public static List<SerializerResult> Run(Component Target, NNASerializerContext Context = null)
		{
			var ret = new List<SerializerResult>();
			if(Target != null)
			{
				Context ??= new NNASerializerContext(Target);
				foreach(var serializer in NNAExportRegistry.Serializers.FindAll(s => Target.GetType() == s.Target))
				{
					var components = serializer.Serialize(Context, Target);
					if(components != null && components.Count > 0) ret.AddRange(serializer.Serialize(Context, Target));
				}
			}
			return ret;
		}

		public static List<SerializerResult> Run(GameObject Target, NNASerializerContext Context = null)
		{
			var ret = new List<SerializerResult>();
			if(Target != null)
			{
				var targets = Target.transform.GetComponentsInChildren<Component>();
				Context ??= new NNASerializerContext(targets);
				foreach(var t in Target.transform.GetComponentsInChildren<Component>())
				{
					if(t.GetType() == typeof(Transform)) continue;
					ret.AddRange(Run(t, Context));
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
						{"component_id", result.JsonComponentId},
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
