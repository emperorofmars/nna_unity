
using UnityEngine;

namespace nna.processors
{
	public interface INameProcessor
	{
		string Type {get;}
		bool CanProcessName(NNAContext Context, string Name);
		void ProcessName(NNAContext Context, Transform Node, string Name);
	}
}
