
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace nna.processors
{
	public interface IProcessor
	{
		string Type {get;}

		void Process(GameObject Root, GameObject NNANode, JObject Json);
	}
}
