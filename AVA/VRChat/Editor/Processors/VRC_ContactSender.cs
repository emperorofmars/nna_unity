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
	public class VRC_ContactSender_VRC_JsonProcessor : IJsonProcessor
	{
		public const string _Type = "vrc.contact_sender";
		public string Type => _Type;
		public uint Order => 1;
		public int Priority => int.MaxValue;

		public void Process(NNAContext Context, Transform Node, JObject Json)
		{
			var targetNode = PhysicsLocationUtil.GetPhysicsNode(Context, Node, "ContactSender");
			var contactSender = targetNode.gameObject.AddComponent<VRCContactSender>();
			if(targetNode != Node) contactSender.rootTransform = Node;

			JsonUtility.FromJsonOverwrite(Json["parsed"].ToString(), contactSender);

			if(Json.ContainsKey("id")) Context.AddResultById((string)Json["id"], contactSender);
		}
	}

	public class VRC_ContactSender_VRC_Serializer : INNASerializer
	{
		public static readonly System.Type _Target = typeof(VRCContactSender);
		public System.Type Target => _Target;

		public List<SerializerResult> Serialize(NNASerializerContext Context, UnityEngine.Object UnityObject)
		{
			var contactSender = (VRCContactSender)UnityObject;
			var retJson = new JObject {{"t", VRC_ContactSender_VRC_JsonProcessor._Type}};
			if(UnityObject.name.StartsWith("$nna:")) retJson.Add("id", UnityObject.name[5..]);

			var parsed = JObject.Parse(JsonUtility.ToJson(contactSender));
			parsed.Remove("rootTransform");

			retJson.Add("parsed", parsed);

			return new List<SerializerResult>{new() {
				NNAType = VRC_ContactSender_VRC_JsonProcessor._Type,
				Origin = UnityObject,
				JsonResult = retJson.ToString(Newtonsoft.Json.Formatting.None),
				JsonComponentId = Context.GetId(UnityObject),
				JsonTargetNode = contactSender.rootTransform ? contactSender.rootTransform.name : contactSender.transform.name,
				IsJsonComplete = true,
				Confidence = SerializerResultConfidenceLevel.MANUAL,
			}};
		}
	}

	[InitializeOnLoad]
	public class Register_VRC_ContactSender_VRC
	{
		static Register_VRC_ContactSender_VRC()
		{
			NNARegistry.RegisterJsonProcessor(new VRC_ContactSender_VRC_JsonProcessor(), DetectorVRC.NNA_VRC_AVATAR_CONTEXT);
			NNAExportRegistry.RegisterSerializer(new VRC_ContactSender_VRC_Serializer());
		}
	}
}

#endif
#endif
