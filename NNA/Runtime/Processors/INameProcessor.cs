
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace nna.processors
{
	public interface INameProcessor
	{
		string Type {get;}
		bool CanProcessName(NNAContext Context, Transform Node);
		void ProcessName(NNAContext Context, Transform Node);
	}
}
