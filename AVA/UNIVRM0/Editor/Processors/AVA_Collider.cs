#if UNITY_EDITOR
#if NNA_AVA_UNIVRM0_FOUND

using System.Collections.Generic;
using nna.ava.common;
using nna.UnityToNNAUtils;
using UnityEditor;
using UnityEngine;
using VRM;

namespace nna.ava.univrm0
{
	public class AVA_Collider_VRM_NameProcessor : Base_AVA_Collider_NameProcessor
	{
		private static VRMSpringBoneColliderGroup InitCollider(NNAContext Context, Transform Node, bool Disabled)
		{
			Transform targetNode = Node;
			if((Node.name.Length == 0 || Node.name.StartsWith("$")) && Node.parent != null)
			{
				targetNode = Node.parent;
			}
			var colliderGroup = targetNode.gameObject.AddComponent<VRMSpringBoneColliderGroup>();
			var collider = new VRMSpringBoneColliderGroup.SphereCollider();
			colliderGroup.Colliders = new VRMSpringBoneColliderGroup.SphereCollider[1];
			colliderGroup.Colliders[0] = collider;

			if(Node.name.Length == 0 || Node.name.StartsWith("$"))
			{
				collider.Offset = Node.localPosition;
				if(Context.ImportOptions.RemoveNNADefinitions) Context.AddTrash(Node);
			}
			if(Disabled) colliderGroup.enabled = false;
			return colliderGroup;
		}

		override protected object BuildSphereCollider(NNAContext Context, Transform Node, bool InsideBounds, float Radius, bool Disabled)
		{
			var collider = InitCollider(Context, Node, Disabled);
			collider.Colliders[0].Radius = Radius;
			return collider;
		}

		override protected object BuildCapsuleCollider(NNAContext Context, Transform Node, bool InsideBounds, float Radius, float Height, bool Disabled)
		{
			var collider = InitCollider(Context, Node, Disabled);
			collider.Colliders[0].Radius = Radius;
			return collider;
		}

		override protected object BuildPlaneCollider(NNAContext Context, Transform Node, bool Disabled)
		{
			return null;
		}
	}

	public class AVA_Collider_VRM_Serializer : INNASerializer
	{
		public static readonly System.Type _Target = typeof(VRMSpringBoneColliderGroup);
		public System.Type Target => _Target;

		public List<SerializerResult> Serialize(NNASerializerContext Context, UnityEngine.Object UnityObject)
		{
			var collider = (VRMSpringBoneColliderGroup)UnityObject;

			var retName = "$Col";
			/*if(collider.shapeType == VRC.Dynamics.VRCPhysBoneColliderBase.ShapeType.Sphere)
			{
				retName += "Sphere";
				if(collider.insideBounds) retName += "In";
				retName += "R" + Math.Round(collider.radius, 2);
			}
			else if(collider.shapeType == VRC.Dynamics.VRCPhysBoneColliderBase.ShapeType.Capsule)
			{
				retName += "Capsule";
				if(collider.insideBounds) retName += "In";
				retName += "R" + Math.Round(collider.radius, 2);
				retName += "H" + Math.Round(collider.height, 2);
			}
			else if(collider.shapeType == VRC.Dynamics.VRCPhysBoneColliderBase.ShapeType.Plane)
			{
				retName += "Plane";
			}*/

			return new List<SerializerResult>{new() {
				NNAType = Base_AVA_Collider_NameProcessor._Type,
				Origin = UnityObject,
				NameResult = retName,
				NameTargetNode = UnityObject.name,
				IsNameComplete = true,
				Confidence = SerializerResultConfidenceLevel.MANUAL,
			}};
		}
	}

	[InitializeOnLoad]
	public class Register_AVA_Collider_VRM
	{
		static Register_AVA_Collider_VRM()
		{
			NNARegistry.RegisterNameProcessor(new AVA_Collider_VRM_NameProcessor(), DetectorUNIVRM0.NNA_UNIVRM0_CONTEXT);
			NNAExportRegistry.RegisterSerializer(new AVA_Collider_VRM_Serializer());
		}
	}
}

#endif
#endif
