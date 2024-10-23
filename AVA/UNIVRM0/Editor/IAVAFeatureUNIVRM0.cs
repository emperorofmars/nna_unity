#if UNITY_EDITOR

using Newtonsoft.Json.Linq;
using UnityEngine;

namespace nna.ava.univrm0
{
	public interface IAVAFeatureUNIVRM0
	{
		string Type {get;}
		bool AutoDetect(NNAContext Context, Component UnityComponent, JObject Json);
	}
}

#endif