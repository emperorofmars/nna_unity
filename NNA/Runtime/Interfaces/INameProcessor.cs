using UnityEngine;

namespace nna.processors
{
	/// <summary>
	/// Name Processors are responsible for processing information contained in node names directly.
	/// </summary>
	/// Once C# 11 becomes available in Unity, convert to using static virtual interface members (https://learn.microsoft.com/en-us/dotnet/csharp/whats-new/tutorials/static-virtual-interface-members)
	public interface INameProcessor
	{
		string Type {get;}
		uint Order {get;}
		bool CanProcessName(NNAContext Context, string Name);
		void Process(NNAContext Context, Transform Node, string Name);
	}
}


