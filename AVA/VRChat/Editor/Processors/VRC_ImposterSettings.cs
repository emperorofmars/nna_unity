#if UNITY_EDITOR
#if NNA_AVA_VRCSDK3_FOUND

using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using nna.ava.common;
using nna.processors;
using nna.UnityToNNAUtils;
using UnityEditor;
using UnityEngine;
using VRC.SDK3.Avatars.Components;

namespace nna.ava.vrchat
{
	public class VRC_ImposterSettings_VRC_JsonProcessor : IJsonProcessor
	{
		public const string _Type = "vrc.imposter_settings";
		public string Type => _Type;
		public uint Order => AVA_Avatar_VRC_Processor._Order + 1;
		public int Priority => int.MaxValue;

		public void Process(NNAContext Context, Transform Node, JObject Json)
		{
			var imposterSettings = Context.Root.AddComponent<VRCImpostorSettings>();

			if(Json.ContainsKey("resolutionScale")) imposterSettings.resolutionScale = (float)Json["resolutionScale"];

			if(Json.TryGetValue("transformsToIgnore", out var transformsToIgnoreNames) && transformsToIgnoreNames.Type == JTokenType.Array)
			{
				var transformsToIgnore = new List<Transform>();
				foreach(string name in transformsToIgnoreNames)
				{
					var node = ParseUtil.FindNode(Context.Root.transform, name);
					transformsToIgnore.Add(node);
				}
				imposterSettings.transformsToIgnore = transformsToIgnore.ToArray();
			}
			if(Json.TryGetValue("extraChildTransforms", out var extraChildTransformsNames) && extraChildTransformsNames.Type == JTokenType.Array)
			{
				var extraChildTransforms = new List<Transform>();
				foreach(string name in extraChildTransformsNames)
				{
					var node = ParseUtil.FindNode(Context.Root.transform, name);
					extraChildTransforms.Add(node);
				}
				imposterSettings.extraChildTransforms = extraChildTransforms.ToArray();
			}
			if(Json.TryGetValue("reparentHere", out var reparentHereNames) && reparentHereNames.Type == JTokenType.Array)
			{
				var reparentHere = new List<Transform>();
				foreach(string name in reparentHereNames)
				{
					var node = ParseUtil.FindNode(Context.Root.transform, name);
					reparentHere.Add(node);
				}
				imposterSettings.reparentHere = reparentHere.ToArray();
			}

			if(Json.ContainsKey("id")) Context.AddResultById((string)Json["id"], imposterSettings);
		}
	}

	public class VRC_ImposterSettings_VRC_Serializer : INNASerializer
	{
		public static readonly System.Type _Target = typeof(VRCImpostorSettings);
		public System.Type Target => _Target;

		public List<SerializerResult> Serialize(NNASerializerContext Context, UnityEngine.Object UnityObject)
		{
			var imposterSettings = (VRCImpostorSettings)UnityObject;
			var retJson = new JObject {{"t", VRC_ImposterSettings_VRC_JsonProcessor._Type}};
			if(UnityObject.name.StartsWith("$nna:")) retJson.Add("id", UnityObject.name[5..]);

			retJson.Add("resolutionScale", imposterSettings.resolutionScale);
			retJson.Add("transformsToIgnore", new JArray(imposterSettings.transformsToIgnore.Select(e => e.name).ToArray()));
			retJson.Add("extraChildTransforms", new JArray(imposterSettings.extraChildTransforms.Select(e => e.name).ToArray()));
			retJson.Add("reparentHere", new JArray(imposterSettings.reparentHere.Select(e => e.name).ToArray()));

			return new List<SerializerResult>{new() {
				NNAType = VRC_ImposterSettings_VRC_JsonProcessor._Type,
				Origin = UnityObject,
				JsonResult = retJson.ToString(Newtonsoft.Json.Formatting.None),
				JsonComponentId = Context.GetId(UnityObject),
				JsonTargetNode = imposterSettings.name,
				IsJsonComplete = true,
				Confidence = SerializerResultConfidenceLevel.MANUAL,
			}};
		}
	}

	[InitializeOnLoad]
	public class Register_VRC_ImposterSettings_VRC
	{
		static Register_VRC_ImposterSettings_VRC()
		{
			NNARegistry.RegisterJsonProcessor(new VRC_ImposterSettings_VRC_JsonProcessor(), DetectorVRC.NNA_VRC_AVATAR_CONTEXT);
			NNAExportRegistry.RegisterSerializer(new VRC_ImposterSettings_VRC_Serializer());
		}
	}
}

#endif
#endif
