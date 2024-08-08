
using UnityEngine;

namespace nna
{
	public interface IProcessor
	{
		string Type {get;}

		void Process(GameObject Root, GameObject NNANode);
	}
}
