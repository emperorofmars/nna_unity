
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace nna.processors
{
	/// <summary>
	/// Json Processors are responsible for processing information serialized as Json in node names.
	/// </summary>
	public interface IJsonProcessor
	{
		string Type {get;}
		void Process(NNAContext Context, Transform Node, JObject Json);
	}
}
