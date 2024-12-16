#if UNITY_EDITOR

using System.Linq;
using UnityEngine;

namespace nna.ava.common
{
	public static class Utils
	{
		public static SkinnedMeshRenderer FindMainMesh(Transform Root, string MeshName)
		{
			if(!string.IsNullOrWhiteSpace(MeshName))
			{
				return Root.GetComponentsInChildren<SkinnedMeshRenderer>().FirstOrDefault(t => t.name == MeshName);
			}
			else
			{
				return Root.GetComponentsInChildren<SkinnedMeshRenderer>().FirstOrDefault(t => t.name.ToLower() == "body");
			}
		}
	}
}

#endif
