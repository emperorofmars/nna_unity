using System.Collections.Generic;
using UnityEngine;

namespace nna.UnityToNNAUtils
{
	[System.Serializable]
	public struct JsonSerializerResult
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
		List<JsonSerializerResult> Serialize(UnityEngine.Object UnityObject);
	}

	public static class RunJsonSerializer
	{
		public static List<JsonSerializerResult> Run(UnityEngine.Object Target)
		{
			var ret = new List<JsonSerializerResult>();
			if(Target != null)
			{
				if(Target.GetType() == typeof(GameObject)) ret.AddRange(Run((GameObject)Target));
				else if(Target.GetType() == typeof(Component)) ret.AddRange(Run((Component)Target));
				// TODO Resources Maybe?
			}
			return ret;
		}
		public static List<JsonSerializerResult> Run(Component Target)
		{
			var ret = new List<JsonSerializerResult>();
			if(Target != null)
			{
				foreach(var serializer in NNAExportRegistry.Serializers.FindAll(s => Target.GetType() == s.Target))
				{
					ret.AddRange(serializer.Serialize(Target));
				}
			}
			return ret;
		}
		public static List<JsonSerializerResult> Run(GameObject Target)
		{
			var ret = new List<JsonSerializerResult>();
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
	}
}
