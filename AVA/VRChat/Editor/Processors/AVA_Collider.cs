#if UNITY_EDITOR
#if NNA_AVA_VRCSDK3_FOUND

using nna.ava.common;
using UnityEditor;
using UnityEngine;
using VRC.SDK3.Dynamics.PhysBone.Components;

namespace nna.ava.vrchat
{
	public class AVA_Collider_VRCNameProcessor : Base_AVA_Collider_NameProcessor
	{
		private static VRCPhysBoneCollider InitCollider(NNAContext Context, Transform Node)
		{
			var targetNode = PhysicsLocationUtil.GetPhysicsNode(Context, Node, "COL_");
			var collider = targetNode.gameObject.AddComponent<VRCPhysBoneCollider>();
			if(targetNode != Node)
			{
				collider.rootTransform = Node.parent;
				collider.position = Node.localPosition;
				collider.rotation = Node.localRotation;
				if(Context.ImportOptions.RemoveNNADefinitions) Context.AddTrash(Node);
			}
			return collider;
		}

		override protected object BuildSphereCollider(NNAContext Context, Transform Node, bool InsideBounds, float Radius)
		{
			var collider = InitCollider(Context, Node);

			collider.shapeType = VRC.Dynamics.VRCPhysBoneColliderBase.ShapeType.Sphere;
			collider.insideBounds = InsideBounds;
			collider.radius = Radius;

			return collider;
		}

		override protected object BuildCapsuleCollider(NNAContext Context, Transform Node, bool InsideBounds, float Radius, float Height)
		{
			var collider = InitCollider(Context, Node);

			collider.shapeType = VRC.Dynamics.VRCPhysBoneColliderBase.ShapeType.Capsule;
			collider.insideBounds = InsideBounds;
			collider.radius = Radius;
			collider.height = Height;

			return collider;
		}

		override protected object BuildPlaneCollider(NNAContext Context, Transform Node)
		{
			var collider = InitCollider(Context, Node);

			collider.shapeType = VRC.Dynamics.VRCPhysBoneColliderBase.ShapeType.Plane;

			return collider;
		}
	}

	[InitializeOnLoad]
	public class Register_AVA_Collider_VRC
	{
		static Register_AVA_Collider_VRC()
		{
			NNARegistry.RegisterNameProcessor(new AVA_Collider_VRCNameProcessor(), DetectorVRC.NNA_VRC_AVATAR_CONTEXT);
			//NNAExportRegistry.RegisterSerializer(new AVA_Collider_VRCSerializer());
		}
	}
}

#endif
#endif
