/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/.
 */

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