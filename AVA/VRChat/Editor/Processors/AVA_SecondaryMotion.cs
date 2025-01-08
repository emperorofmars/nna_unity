#if UNITY_EDITOR
#if NNA_AVA_VRCSDK3_FOUND

using Newtonsoft.Json.Linq;
using nna.ava.common;
using nna.processors;
using UnityEditor;
using UnityEngine;
using VRC.Dynamics;
using VRC.SDK3.Dynamics.PhysBone.Components;

namespace nna.ava.vrchat
{
	public class AVA_SecondaryMotion_VRCJsonProcessor : IJsonProcessor
	{
		public const string _Type = "ava.secondary_motion";
		public string Type => _Type;
		public uint Order => Base_AVA_Collider_NameProcessor._Order + 1; // Colliders have to be parsed first
		public int Priority => int.MaxValue;

		public void Process(NNAContext Context, Transform Node, JObject Json)
		{
			var targetNode = PhysicsLocationUtil.GetPhysicsNode(Context, Node);
			var physbone = targetNode.gameObject.AddComponent<VRCPhysBone>();
			if(targetNode != Node) physbone.rootTransform = Node;

			if(Json.ContainsKey("ignoreTransforms") && Json["ignoreTransforms"].Type == JTokenType.Array)
			{
				var ignoreTransforms = Json["ignoreTransforms"];
				foreach(string name in ignoreTransforms)
				{
					var node = ParseUtil.FindNode(Context.Root.transform, name);
					physbone.ignoreTransforms.Add(node);
				}
			}
			if(Json.ContainsKey("colliders") && Json["colliders"].Type == JTokenType.Array)
			{
				var colliders = Json["colliders"];
				foreach(var id in colliders)
					foreach(var result in Context.GetResultsById((string)id))
						if(result is VRCPhysBoneColliderBase)
							physbone.colliders.Add(result as VRCPhysBoneColliderBase);
						else
							Context.Report(new("Didn't find Physbone Collider", NNAErrorSeverity.WARNING, _Type, Node));
			}


			// TODO: Proper best effort conversion from the nna secondary motion values to physbone values

			if(Json.ContainsKey("id")) Context.AddResultById((string)Json["id"], physbone);
		}
	}

	[InitializeOnLoad]
	public class Register_AVA_SecondaryMotion_VRC
	{
		static Register_AVA_SecondaryMotion_VRC()
		{
			NNARegistry.RegisterJsonProcessor(new AVA_SecondaryMotion_VRCJsonProcessor(), DetectorVRC.NNA_VRC_AVATAR_CONTEXT);
		}
	}
}

#endif
#endif
