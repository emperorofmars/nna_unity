
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace nna.processors
{
	public interface IProcessor
	{
		string Type {get;}

		void ProcessJson(NNAContext Context, Transform Node, JObject Json);

		bool CanProcessName(NNAContext Context, Transform Node);
		void ProcessName(NNAContext Context, Transform Node);
	}
}
