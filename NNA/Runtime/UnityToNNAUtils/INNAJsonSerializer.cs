
using System.Collections.Generic;

namespace nna.UnityToNNAUtils
{
	/// <summary>
	/// NNA Json Serializers manually convert Unity Objects into NNA compatible Json.
	/// </summary>
	public interface INNAJsonSerializer
	{
		System.Type Target {get;}
		List<string> Serialize(UnityEngine.Object UnityObject);
	}
}
