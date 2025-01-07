#if UNITY_EDITOR
#if NNA_AVA_VRCSDK3_FOUND

using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using nna.ava.common;
using nna.processors;
using nna.UnityToNNAUtils;
using UnityEditor;
using UnityEngine;
using VRC.SDK3.Dynamics.Contact.Components;

namespace nna.ava.vrchat
{
	public class VRC_ContactReceiver_VRC_JsonProcessor : IJsonProcessor
	{
		public const string _Type = "vrc.contact_receiver";
		public string Type => _Type;
		public uint Order => 0;
		public int Priority => int.MaxValue;

		public void Process(NNAContext Context, Transform Node, JObject Json)
		{
			var targetNode = PhysicsLocationUtil.GetPhysicsNode(Context, Node, "ContactReceiver");
			var contactReceiver = targetNode.gameObject.AddComponent<VRCContactReceiver>();
			if(targetNode != Node) contactReceiver.rootTransform = Node;

			JsonUtility.FromJsonOverwrite(Json["parsed"].ToString(), contactReceiver);

			/*if(Json.ContainsKey("target"))
			{
				var target = ParseUtil.FindNode(Context.Root.transform, (string)Json["target"]);
				if(target) contactReceiver.rootTransform = target;
			}*/

			if(Json.ContainsKey("id")) Context.AddResultById((string)Json["id"], contactReceiver);
		}
	}

	public class VRC_ContactReceiver_VRC_Serializer : INNASerializer
	{
		public static readonly System.Type _Target = typeof(VRCContactReceiver);
		public System.Type Target => _Target;

		public List<SerializerResult> Serialize(NNASerializerContext Context, UnityEngine.Object UnityObject)
		{
			var contactReceiver = (VRCContactReceiver)UnityObject;
			var retJson = new JObject {{"t", VRC_ContactReceiver_VRC_JsonProcessor._Type}};
			if(UnityObject.name.StartsWith("$nna:")) retJson.Add("id", UnityObject.name[5..]);

			var parsed = JObject.Parse(JsonUtility.ToJson(contactReceiver));
			parsed.Remove("rootTransform");

			retJson.Add("parsed", parsed);

			//if(contactReceiver.rootTransform) retJson.Add("target", contactReceiver.rootTransform.name);

			return new List<SerializerResult>{new() {
				NNAType = VRC_ContactReceiver_VRC_JsonProcessor._Type,
				Origin = UnityObject,
				JsonResult = retJson.ToString(Newtonsoft.Json.Formatting.None),
				JsonComponentId = Context.GetId(UnityObject),
				JsonTargetNode = contactReceiver.rootTransform ? contactReceiver.rootTransform.name : contactReceiver.transform.name,
				IsJsonComplete = true,
				Confidence = SerializerResultConfidenceLevel.MANUAL,
			}};
		}
	}

	[InitializeOnLoad]
	public class Register_VRC_ContactReceiver_VRC
	{
		static Register_VRC_ContactReceiver_VRC()
		{
			NNARegistry.RegisterJsonProcessor(new VRC_ContactReceiver_VRC_JsonProcessor(), DetectorVRC.NNA_VRC_AVATAR_CONTEXT);
			NNAExportRegistry.RegisterSerializer(new VRC_ContactReceiver_VRC_Serializer());
		}
	}
}

#endif
#endif
