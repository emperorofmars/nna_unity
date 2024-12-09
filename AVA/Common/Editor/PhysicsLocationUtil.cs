#if UNITY_EDITOR

using UnityEngine;

namespace nna.ava.common
{
	public static class PhysicsLocationUtil
	{
		public static Transform GetPhysicsNode(NNAContext Context, Transform Node, string Prefix = "Phys_", string ParentTargetName = "physics")
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
				targetNode.name = Prefix + Node.name;
			}
			return targetNode;
		}

		public static Transform GetOrCreatePhysicsParent(NNAContext Context, string Name = "physics")
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
