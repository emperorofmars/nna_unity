
using System.Collections.Generic;

namespace nna.UnityToNNAUtils
{
	/// <summary>
	/// NNA Json Serializers manually convert Unity Objects into NNA compatible Json.
	/// </summary>
	/// Once C# 11 becomes available in Unity, convert to using static virtual interface members (https://learn.microsoft.com/en-us/dotnet/csharp/whats-new/tutorials/static-virtual-interface-members)
	public interface INNAJsonSerializer
	{
		System.Type Target {get;}
		List<(string ComponentType, string Json)> Serialize(UnityEngine.Object UnityObject);
	}
}
