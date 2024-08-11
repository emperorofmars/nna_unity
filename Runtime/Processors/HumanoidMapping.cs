
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace nna.processors
{
	public class HumanoidMapping : IProcessor
	{
		public static readonly string _Type = "humanoid";
		public string Type => _Type;

		public void Process(NNAContext Context, GameObject NNANode, JObject Json)
		{
		}
	}
}
