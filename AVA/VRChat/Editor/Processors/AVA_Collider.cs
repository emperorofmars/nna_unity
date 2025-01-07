#if UNITY_EDITOR
#if NNA_AVA_VRCSDK3_FOUND

using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using nna.ava.common;
using nna.UnityToNNAUtils;
using UnityEditor;
using UnityEngine;
using VRC.SDK3.Dynamics.PhysBone.Components;

namespace nna.ava.vrchat
{
	public class AVA_Collider_VRC_NameProcessor : Base_AVA_Collider_NameProcessor
	{
		private static VRCPhysBoneCollider InitCollider(NNAContext Context, Transform Node, bool Disabled)
		{
			var targetNode = PhysicsLocationUtil.GetPhysicsNode(Context, Node);
			var collider = targetNode.gameObject.AddComponent<VRCPhysBoneCollider>();
			if(targetNode != Node)
			{
				collider.rootTransform = Node.parent;
				collider.position = Node.localPosition;
				collider.rotation = Node.localRotation;
				if(Context.ImportOptions.RemoveNNADefinitions) Context.AddTrash(Node);
			}
			if(Disabled)
			{
				collider.enabled = false;
				if(targetNode != Node) targetNode.gameObject.SetActive(false);
			}
			return collider;
		}

		override protected object BuildSphereCollider(NNAContext Context, Transform Node, bool InsideBounds, float Radius, bool Disabled)
		{
			var collider = InitCollider(Context, Node, Disabled);

			collider.shapeType = VRC.Dynamics.VRCPhysBoneColliderBase.ShapeType.Sphere;
			collider.insideBounds = InsideBounds;
			collider.radius = Radius;

			return collider;
		}

		override protected object BuildCapsuleCollider(NNAContext Context, Transform Node, bool InsideBounds, float Radius, float Height, bool Disabled)
		{
			var collider = InitCollider(Context, Node, Disabled);

			collider.shapeType = VRC.Dynamics.VRCPhysBoneColliderBase.ShapeType.Capsule;
			collider.insideBounds = InsideBounds;
			collider.radius = Radius;
			collider.height = Height;

			return collider;
		}

		override protected object BuildPlaneCollider(NNAContext Context, Transform Node, bool Disabled)
		{
			var collider = InitCollider(Context, Node, Disabled);

			collider.shapeType = VRC.Dynamics.VRCPhysBoneColliderBase.ShapeType.Plane;

			return collider;
		}
	}

	public class AVA_Collider_VRC_Serializer : INNASerializer
	{
		public static readonly System.Type _Target = typeof(VRCPhysBoneCollider);
		public System.Type Target => _Target;

		public List<SerializerResult> Serialize(NNASerializerContext Context, UnityEngine.Object UnityObject)
		{
			var collider = (VRCPhysBoneCollider)UnityObject;

			var retName = "$Col";
			if(collider.shapeType == VRC.Dynamics.VRCPhysBoneColliderBase.ShapeType.Sphere)
			{
				retName += "Sphere";
				if(collider.insideBounds) retName += "In";
				retName += "R" + Math.Round(collider.radius, 3);
			}
			else if(collider.shapeType == VRC.Dynamics.VRCPhysBoneColliderBase.ShapeType.Capsule)
			{
				retName += "Capsule";
				if(collider.insideBounds) retName += "In";
				retName += "R" + Math.Round(collider.radius, 3);
				retName += "H" + Math.Round(collider.height, 3);
			}
			else if(collider.shapeType == VRC.Dynamics.VRCPhysBoneColliderBase.ShapeType.Plane)
			{
				retName += "Plane";
			}

			return new List<SerializerResult>{new() {
				NNAType = Base_AVA_Collider_NameProcessor._Type,
				Origin = UnityObject,
				NameResult = retName,
				NameTargetNode = collider.transform.name,
				IsNameComplete = true,
				Confidence = SerializerResultConfidenceLevel.MANUAL,
			}};
		}
	}

	[InitializeOnLoad]
	public class Register_AVA_Collider_VRC
	{
		static Register_AVA_Collider_VRC()
		{
			NNARegistry.RegisterNameProcessor(new AVA_Collider_VRC_NameProcessor(), DetectorVRC.NNA_VRC_AVATAR_CONTEXT);
			NNAExportRegistry.RegisterSerializer(new AVA_Collider_VRC_Serializer());
		}
	}
}

#endif
#endif
