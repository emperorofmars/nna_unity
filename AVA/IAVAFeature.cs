
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace nna.ava
{
	public interface IAVAFeature
	{
		bool AutoDetect(NNAContext Context, Component UnityComponent, JObject Json);
	}
}

