using System.Collections.Generic;
using UnityEngine;

namespace nna.UnityToNNAUtils
{
	[System.Serializable]
	public struct JsonSerializerResult
	{
		public string Type;
		public string TargetNode;
		public string JsonResult;
		public string NameResult;
		public Component Origin;
	}

	/// <summary>
	/// NNA Json Serializers manually convert Unity Objects into NNA compatible Json.
	/// </summary>
	/// Once C# 11 becomes available in Unity, convert to using static virtual interface members (https://learn.microsoft.com/en-us/dotnet/csharp/whats-new/tutorials/static-virtual-interface-members)
	public interface INNAJsonSerializer
	{
		System.Type Target {get;}
		List<JsonSerializerResult> Serialize(UnityEngine.Object UnityObject);
	}
}
