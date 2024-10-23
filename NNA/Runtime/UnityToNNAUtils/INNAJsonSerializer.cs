
using System.Collections.Generic;

namespace nna.UnityToNNAUtils
{
	public interface INNAJsonSerializer
	{
		System.Type Target {get;}
		List<string> Serialize(UnityEngine.Object UnityObject);
	}
}
