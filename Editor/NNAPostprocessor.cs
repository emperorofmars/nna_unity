
#if UNITY_EDITOR

using UnityEngine;
using UnityEditor;

namespace nna
{
	class NNAPostprocessor : AssetPostprocessor
	{
		void OnPostprocessModel(GameObject Root)
		{
			NNAConvert.Convert(Root);
		}
	}
}

#endif
