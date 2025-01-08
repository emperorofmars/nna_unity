#if UNITY_EDITOR

using UnityEngine;

namespace nna.ava.common
{
	public static class PhysicsLocationUtil
	{
		public static Transform GetPhysicsNode(NNAContext Context, Transform Node, string NodeNamePrefix = "", string TargetNodeNameOverride = null, string ParentTargetName = "_physics", bool IgnoreMeta = false)
		{
			var noSeparatePhysics = Context.GetMetaCustomValue("ava.no_separate_physics");
			if(noSeparatePhysics != "true" || IgnoreMeta)
			{
				var separatePhysicsNode = !IgnoreMeta ? Context.GetMetaCustomValue("ava.separate_physics_node") : ParentTargetName;
				separatePhysicsNode ??= ParentTargetName;

				Transform parent = GetOrCreatePhysicsParent(Context, separatePhysicsNode);

				var targetNodeName = !string.IsNullOrWhiteSpace(TargetNodeNameOverride) ? TargetNodeNameOverride : NodeNamePrefix + ParseUtil.GetNameComponentId(Node.name);

				if(parent.Find(targetNodeName) is var existingTargetNode && existingTargetNode != null) return existingTargetNode;

				var targetNodeGo = new GameObject();
				var targetNode = targetNodeGo.transform;
				targetNode.parent = parent;
				targetNode.name = targetNodeName;
				return targetNode;
			}
			return Node;
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
