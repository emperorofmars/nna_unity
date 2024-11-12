using Newtonsoft.Json.Linq;
using UnityEngine;

namespace nna.processors
{
	/// <summary>
	/// Json Processors are responsible for processing information serialized as Json in node names.
	/// </summary>
	/// Once C# 11 becomes available in Unity, convert to using static virtual interface members (https://learn.microsoft.com/en-us/dotnet/csharp/whats-new/tutorials/static-virtual-interface-members)
	public interface IJsonProcessor
	{
		string Type {get;}
		void Process(NNAContext Context, Transform Node, JObject Json);
	}
}
