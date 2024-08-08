
using System.Collections.Generic;
using UnityEngine;

namespace nna.processors
{
	public interface IProcessor
	{
		string Type {get;}

		void Process(GameObject Root, GameObject NNANode, Dictionary<string, NNAValue> Properties);
	}
}
