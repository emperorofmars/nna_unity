#if UNITY_EDITOR

using UnityEngine;

namespace nna.ava.common
{
	public static class PhysicsLocationUtil
	{
		public static Transform GetPhysicsNode(NNAContext Context, Transform Node, string Prefix = "Phys_")
		{
			var separatePhysics = Context.GetMetaCustomValue("nna.separate_physics");
			var targetNode = Node;
			if(separatePhysics != null)
			{
				Transform parent = null;
				for(int i = 0; i < Context.Root.transform.childCount; i++)
				{
					var child = Context.Root.transform.GetChild(i);
					if(child.name == separatePhysics)
					{
						parent = child;
						break;
					}
				}
				if(!parent)
				{
					var parentGo = new GameObject();
					parent = parentGo.transform;
					parent.name = separatePhysics;
					parent.parent = Context.Root.transform;
				}
				var targetNodeGo = new GameObject();
				targetNode = targetNodeGo.transform;
				targetNode.parent = parent;
				targetNode.name = Prefix + Node.name;
			}
			return targetNode;
		}
	}
}

#endif
