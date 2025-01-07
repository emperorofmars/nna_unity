#if UNITY_EDITOR
#if NNA_AVA_DYNAMICBONES_FOUND

using nna.ava.common;
using UnityEditor;
using UnityEngine;

namespace nna.ava.dynamicbones
{
	public class AVA_Collider_NameProcessor : Base_AVA_Collider_NameProcessor
	{
		override protected object BuildSphereCollider(NNAContext Context, Transform Node, bool InsideBounds, float Radius, bool Disabled)
		{
			var collider = Node.gameObject.AddComponent<DynamicBoneCollider>();
			collider.m_Radius = Radius;

			if(Disabled) collider.enabled = false;
			return collider;
		}

		override protected object BuildCapsuleCollider(NNAContext Context, Transform Node, bool InsideBounds, float Radius, float Height, bool Disabled)
		{
			var collider = Node.gameObject.AddComponent<DynamicBoneCollider>();
			collider.m_Radius = Radius;
			collider.m_Height = Height;

			if(Disabled) collider.enabled = false;
			return collider;
		}

		override protected object BuildPlaneCollider(NNAContext Context, Transform Node, bool Disabled)
		{
			var collider = Node.gameObject.AddComponent<DynamicBonePlaneCollider>();

			if(Disabled) collider.enabled = false;
			return collider;
		}
	}

	[InitializeOnLoad]
	public class Register_AVA_Collider
	{
		static Register_AVA_Collider()
		{
			NNARegistry.RegisterNameProcessor(new AVA_Collider_NameProcessor());
		}
	}
}

#endif
#endif
