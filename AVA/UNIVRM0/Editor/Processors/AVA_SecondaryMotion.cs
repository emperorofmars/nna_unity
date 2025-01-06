#if UNITY_EDITOR
#if NNA_AVA_UNIVRM0_FOUND

using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using nna.ava.common;
using nna.processors;
using UnityEditor;
using UnityEngine;
using VRM;

namespace nna.ava.univrm0
{
	public class AVA_SecondaryMotion_VRM_JsonProcessor : IJsonProcessor
	{
		public const string _Type = "ava.secondary_motion";
		public string Type => _Type;
		public uint Order => Base_AVA_Collider_NameProcessor._Order + 1; // Colliders have to be parsed first
		public int Priority => 0;

		public void Process(NNAContext Context, Transform Node, JObject Json)
		{
			var targetNode = PhysicsLocationUtil.GetPhysicsNode(Context, Node, "", "VRM_secondary");
			var springBone = targetNode.gameObject.AddComponent<VRMSpringBone>();
			if(targetNode != Node) springBone.RootBones.Add(Node);

			/*if(Json.TryGetValue("ignoreTransforms", out var ignoreTransforms) && ignoreTransforms.Type == JTokenType.Array)
			{
				foreach(string name in ignoreTransforms)
				{
					var node = ParseUtil.FindNode(Context.Root.transform, name);
					//why no springbone exclusions
				}
			}*/

			var colliderGroups = new List<VRMSpringBoneColliderGroup>();

			if(Json.TryGetValue("colliders", out var colliders) && colliders.Type == JTokenType.Array)
				foreach(var id in colliders)
					foreach(var result in Context.GetResultsById((string)id))
						if(result is VRMSpringBoneColliderGroup)
							colliderGroups.Add(result as VRMSpringBoneColliderGroup);

			springBone.ColliderGroups = colliderGroups.ToArray();


			// TODO: Proper best effort conversion from the nna secondary motion values to springbone values

			if(Json.ContainsKey("id")) Context.AddResultById((string)Json["id"], springBone);
		}
	}

	[InitializeOnLoad]
	public class Register_AVA_SecondaryMotion_VRM_JsonProcessor
	{
		static Register_AVA_SecondaryMotion_VRM_JsonProcessor()
		{
			NNARegistry.RegisterJsonProcessor(new AVA_SecondaryMotion_VRM_JsonProcessor(), DetectorUNIVRM0.NNA_UNIVRM0_CONTEXT);
		}
	}
}

#endif
#endif
