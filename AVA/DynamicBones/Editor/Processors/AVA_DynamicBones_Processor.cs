#if UNITY_EDITOR
#if NNA_AVA_DYNAMICBONES_FOUND

using Newtonsoft.Json.Linq;
using nna.ava.common;
using nna.processors;
using UnityEditor;
using UnityEngine;

namespace nna.ava.dynamicbones
{
	public class AVA_DynamicBones_JsonProcessor : IJsonProcessor
	{
		public const string _Type = "willhongcom.dynamicbone";
		public string Type => _Type;
		public uint Order => Base_AVA_Collider_NameProcessor._Order + 1; // Colliders have to be parsed first

		public int Priority => int.MaxValue;

		public void Process(NNAContext Context, Transform Node, JObject Json)
		{
			var targetNode = PhysicsLocationUtil.GetPhysicsNode(Context, Node, "DB_");
			var dybone = targetNode.gameObject.AddComponent<DynamicBone>();
			if(targetNode != Node) dybone.m_Root = Node;

			JsonUtility.FromJsonOverwrite(Json["parsed"].ToString(), dybone);

			if(Json.TryGetValue("ignoreTransforms", out var ignoreTransforms) && ignoreTransforms.Type != JTokenType.Array)
			{
				foreach(string name in ignoreTransforms)
				{
					var node = ParseUtil.FindNode(Context.Root.transform, name);
					dybone.m_Exclusions.Add(node);
				}
			}

			if(Json.TryGetValue("colliders", out var colliders) && colliders.Type == JTokenType.Array)
				foreach(var id in colliders)
					foreach(var result in Context.GetResultsById((string)id))
						if(result is DynamicBoneColliderBase)
							dybone.m_Colliders.Add(result as DynamicBoneColliderBase);

			if(Json.ContainsKey("id")) Context.AddResultById((string)Json["id"], dybone);
		}

		[InitializeOnLoad]
		public class Register_AVA_DynamicBones
		{
			static Register_AVA_DynamicBones()
			{
				NNARegistry.RegisterJsonProcessor(new AVA_DynamicBones_JsonProcessor());
			}
		}
	}
}

#endif
#endif
