
#if UNITY_EDITOR

using UnityEngine;
using UnityEditor;

namespace nna
{
	class UnityNNAPostprocessor : AssetPostprocessor
	{
		void OnPostprocessModel(GameObject Root)
		{
			var nnaContext = new NNAContext(Root);
			NNAConverter.Convert(nnaContext);
			foreach(var newObj in nnaContext.GetNewObjects())
			{
				context.AddObjectToAsset(newObj.Name, newObj.NewObject);
			}
		}
	}
}

#endif
