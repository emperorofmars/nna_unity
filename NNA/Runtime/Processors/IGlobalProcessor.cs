
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace nna.processors
{
	public interface IGlobalProcessor
	{
		string Type {get;}
		void Process(NNAContext Context);
	}
}
