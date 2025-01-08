#if UNITY_EDITOR
#if NNA_AVA_UNIVRM0_FOUND

using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json.Linq;
using nna.ava.common;
using nna.processors;
using nna.UnityToNNAUtils;
using nna.util;
using UnityEditor;
using UnityEngine;
using VRM;

namespace nna.ava.univrm0
{
	public class VRM_ClipMapping_VRM_JsonProcessor : IJsonProcessor
	{
		public const string _Type = "vrm.vrm.clip_mapping";
		public string Type => _Type;
		public uint Order => AVA_Avatar_UNIVRM0Processor._Order + 1;
		public int Priority => 0;

		public void Process(NNAContext Context, Transform Node, JObject Json)
		{
			var avatarJson = Context.GetOnlyJsonComponentByType("ava.avatar").Component;
			if(avatarJson != null && avatarJson.ContainsKey("auto") && !(bool)avatarJson["auto"]) return;

			var vrmBlendshapeProxy = Context.Root.GetComponent<VRMBlendShapeProxy>();
			if(!vrmBlendshapeProxy) vrmBlendshapeProxy = Context.Root.AddComponent<VRMBlendShapeProxy>();

			if(!vrmBlendshapeProxy.BlendShapeAvatar)
			{
				var vrmBlendShapeAvatar = ScriptableObject.CreateInstance<BlendShapeAvatar>();
				vrmBlendShapeAvatar.name = "VRM_BlendshapeAvatar";

				vrmBlendshapeProxy.BlendShapeAvatar = vrmBlendShapeAvatar;
				Context.AddObjectToAsset(vrmBlendShapeAvatar.name, vrmBlendShapeAvatar);

				var neutralClip = BlendshapeClipUtil.CreateEmpty(BlendShapePreset.Neutral);
				Context.AddObjectToAsset(neutralClip.name, neutralClip);
				vrmBlendShapeAvatar.Clips.Add(neutralClip);
			}

			foreach(var mapping in Json["clips"])
			{
				if(string.IsNullOrWhiteSpace((string)mapping) && AssetResourceUtil.FindAsset<BlendShapeClip>(Context, (string)mapping, true, "asset") is var clip && clip)
				{
					vrmBlendshapeProxy.BlendShapeAvatar.Clips.Add(clip);
				}
				else
				{
					Context.Report(new("Didn't find valid VRM Clip: " + mapping, NNAErrorSeverity.WARNING, _Type, Node));
				}
			}

			if(Json.ContainsKey("id")) Context.AddResultById((string)Json["id"], vrmBlendshapeProxy);
		}
	}


	public class VRM_ClipMapping_VRMSerializer : INNASerializer
	{
		public static readonly System.Type _Target = typeof(VRMBlendShapeProxy);
		public System.Type Target => _Target;

		public List<SerializerResult> Serialize(NNASerializerContext Context, UnityEngine.Object UnityObject)
		{
			var vrmBlendshapeProxy = (VRMBlendShapeProxy)UnityObject;
			var retJson = new JObject {{"t", VRM_ClipMapping_VRM_JsonProcessor._Type}};
			if(UnityObject.name.StartsWith("$nna:")) retJson.Add("id", UnityObject.name[5..]);

			var clips = new JArray();
			foreach(var clip in vrmBlendshapeProxy?.BlendShapeAvatar?.Clips)
			{
				clips.Add(Path.GetFileName(AssetDatabase.GetAssetPath(clip)));
			}
			retJson.Add("clips", clips);

			return new List<SerializerResult>{new() {
				NNAType = VRM_ClipMapping_VRM_JsonProcessor._Type,
				Origin = UnityObject,
				JsonResult = retJson.ToString(Newtonsoft.Json.Formatting.None),
				JsonComponentId = Context.GetId(UnityObject),
				JsonTargetNode = "$root",
				IsJsonComplete = true,
				Confidence = SerializerResultConfidenceLevel.MANUAL,
			}};
		}
	}


	[InitializeOnLoad]
	public class Register_VRM_ClipMapping_VRM
	{
		static Register_VRM_ClipMapping_VRM()
		{
			NNARegistry.RegisterJsonProcessor(new VRM_ClipMapping_VRM_JsonProcessor(), DetectorUNIVRM0.NNA_UNIVRM0_CONTEXT);
			NNAExportRegistry.RegisterSerializer(new VRM_ClipMapping_VRMSerializer());
		}
	}
}

#endif
#endif
