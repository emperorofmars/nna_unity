
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace nna.processors
{
	public interface IJsonProcessor
	{
		string Type {get;}
		void ProcessJson(NNAContext Context, Transform Node, JObject Json);
	}
}
