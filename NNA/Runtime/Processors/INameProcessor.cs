
using UnityEngine;

namespace nna.processors
{
	/// <summary>
	/// Name Processors are responsible for processing information contained in node names directly.
	/// </summary>
	public interface INameProcessor
	{
		string Type {get;}
		bool CanProcessName(NNAContext Context, string Name);
		void Process(NNAContext Context, Transform Node, string Name);
	}
}
