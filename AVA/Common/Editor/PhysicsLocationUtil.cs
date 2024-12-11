/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/.
 */

#if UNITY_EDITOR

using UnityEngine;

namespace nna.ava.common
{
	public static class PhysicsLocationUtil
	{
		public static Transform GetPhysicsNode(NNAContext Context, Transform Node, string NodeNamePrefix = "", string ParentTargetName = "_physics")
		{
			var targetNode = Node;
			var separatePhysics = Context.GetMetaCustomValue("nna.no_separate_physics");
			if(separatePhysics != "true")
			{
				var separatePhysicsNode = Context.GetMetaCustomValue("nna.separate_physics_node");
				separatePhysicsNode ??= ParentTargetName;
				separatePhysicsNode ??= "physics";

				Transform parent = GetOrCreatePhysicsParent(Context, separatePhysicsNode);

				var targetNodeGo = new GameObject();
				targetNode = targetNodeGo.transform;
				targetNode.parent = parent;
				targetNode.name = NodeNamePrefix + ParseUtil.GetNameComponentId(Node.name);
			}
			return targetNode;
		}

		public static Transform GetOrCreatePhysicsParent(NNAContext Context, string Name = "_physics")
		{
			for(int i = 0; i < Context.Root.transform.childCount; i++)
			{
				var child = Context.Root.transform.GetChild(i);
				if(child.name == Name)
				{
					return child;
				}
			}
			var parent = new GameObject();
			parent.name = Name;
			parent.transform.parent = Context.Root.transform;
			return parent.transform;
		}
	}
}

#endif
