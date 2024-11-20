#if UNITY_EDITOR
#if NNA_AVA_VRCSDK3_FOUND

using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using nna.processors;
using nna.UnityToNNAUtils;
using nna.util;
using UnityEditor;
using UnityEngine;
using VRC.SDK3.Avatars.Components;

namespace nna.ava.vrchat
{
	public class VRCAvatarCollidersJsonProcessor : IJsonProcessor
	{
		public const string _Type = "vrc.avatar_colliders";
		public string Type => _Type;
		public uint Order => VRCAvatar._Order + 1;

		public void Process(NNAContext Context, Transform Node, JObject Json)
		{
			var avatar = Context.Root.GetComponent<VRCAvatarDescriptor>();

			avatar.collider_head = ParseVRCCollider(Json["head"]);
			avatar.collider_torso = ParseVRCCollider(Json["torso"]);
			avatar.collider_footL = ParseVRCCollider(Json["footL"]);
			avatar.collider_footR = ParseVRCCollider(Json["footR"]);
			avatar.collider_handL = ParseVRCCollider(Json["handL"]);
			avatar.collider_handR = ParseVRCCollider(Json["handR"]);
			avatar.collider_fingerIndexL = ParseVRCCollider(Json["fingerIndexL"]);
			avatar.collider_fingerIndexR = ParseVRCCollider(Json["fingerIndexR"]);
			avatar.collider_fingerMiddleL = ParseVRCCollider(Json["fingerMiddleL"]);
			avatar.collider_fingerMiddleR = ParseVRCCollider(Json["fingerMiddleR"]);
			avatar.collider_fingerRingL = ParseVRCCollider(Json["fingerRingL"]);
			avatar.collider_fingerRingR = ParseVRCCollider(Json["fingerRingR"]);
			avatar.collider_fingerLittleL = ParseVRCCollider(Json["fingerLittleL"]);
			avatar.collider_fingerLittleR = ParseVRCCollider(Json["fingerLittleR"]);
		}

		private static VRCAvatarDescriptor.ColliderConfig ParseVRCCollider(JToken ColliderDef)
		{
			var ColliderConfig = new VRCAvatarDescriptor.ColliderConfig();
			if(ColliderDef == null) return ColliderConfig;
			if(ColliderDef.Type == JTokenType.Boolean)
			{
				ColliderConfig.state = (bool)ColliderDef ? VRCAvatarDescriptor.ColliderConfig.State.Automatic : VRCAvatarDescriptor.ColliderConfig.State.Disabled;
			}
			else if(ColliderDef.Type == JTokenType.Object)
			{
				ColliderConfig.state = VRCAvatarDescriptor.ColliderConfig.State.Custom;
				if(((JObject)ColliderDef).ContainsKey("m")) ColliderConfig.isMirrored = (bool)ColliderDef["m"];
				if(((JObject)ColliderDef).ContainsKey("r")) ColliderConfig.radius = (float)ColliderDef["r"];
				if(((JObject)ColliderDef).ContainsKey("h")) ColliderConfig.height = (float)ColliderDef["h"];
				if(((JObject)ColliderDef).ContainsKey("pos"))ColliderConfig.position = TRSUtil.ParseVector3((JArray)ColliderDef["pos"]);
				if(((JObject)ColliderDef).ContainsKey("rot")) ColliderConfig.rotation = TRSUtil.ParseQuat((JArray)ColliderDef["rot"]);
			}
			return ColliderConfig;
		}
	}

	public class VRCAvatarColliderJsonSerializer : INNAJsonSerializer
	{
		public Type Target => typeof(VRCAvatarDescriptor);

		public List<JsonSerializerResult> Serialize(UnityEngine.Object UnityObject)
		{
			var ret = new JObject {{"t", VRCAvatarCollidersJsonProcessor._Type}};
			var avatar = (VRCAvatarDescriptor)UnityObject;

			ret.Add("head", SerialilzeVRCCollider(avatar.collider_head));
			ret.Add("torso", SerialilzeVRCCollider(avatar.collider_torso));
			ret.Add("footL", SerialilzeVRCCollider(avatar.collider_footL));
			ret.Add("footR", SerialilzeVRCCollider(avatar.collider_footR));
			ret.Add("handL", SerialilzeVRCCollider(avatar.collider_handL));
			ret.Add("handR", SerialilzeVRCCollider(avatar.collider_handR));
			ret.Add("fingerIndexL", SerialilzeVRCCollider(avatar.collider_fingerIndexL));
			ret.Add("fingerIndexR", SerialilzeVRCCollider(avatar.collider_fingerIndexR));
			ret.Add("fingerMiddleL", SerialilzeVRCCollider(avatar.collider_fingerMiddleL));
			ret.Add("fingerMiddleR", SerialilzeVRCCollider(avatar.collider_fingerMiddleR));
			ret.Add("fingerRingL", SerialilzeVRCCollider(avatar.collider_fingerRingL));
			ret.Add("fingerRingR", SerialilzeVRCCollider(avatar.collider_fingerRingR));
			ret.Add("fingerLittleL", SerialilzeVRCCollider(avatar.collider_fingerLittleL));
			ret.Add("fingerLittleR", SerialilzeVRCCollider(avatar.collider_fingerLittleR));

			return new List<JsonSerializerResult>{new() {JsonType=VRCAvatarCollidersJsonProcessor._Type, JsonTargetNode="$root", JsonResult=ret.ToString(Newtonsoft.Json.Formatting.None)}};
		}

		private static JToken SerialilzeVRCCollider(VRCAvatarDescriptor.ColliderConfig ColliderConfig)
		{
			if(ColliderConfig.state == VRCAvatarDescriptor.ColliderConfig.State.Disabled) return false;
			if(ColliderConfig.state == VRCAvatarDescriptor.ColliderConfig.State.Automatic) return true;
			return new JObject
			{
				{ "m", ColliderConfig.isMirrored },
				{ "r", ColliderConfig.radius },
				{ "h", ColliderConfig.height },
				{ "pos", TRSUtil.SerializeVector3(ColliderConfig.position) },
				{ "rot", TRSUtil.SerializeQuat(ColliderConfig.rotation) }
			};
		}
	}

	[InitializeOnLoad]
	public class Register_VRCAvatarColliders
	{
		static Register_VRCAvatarColliders()
		{
			NNARegistry.RegisterJsonProcessor(new VRCAvatarCollidersJsonProcessor(), DetectorVRC.NNA_VRC_AVATAR_CONTEXT);
			NNAJsonExportRegistry.RegisterSerializer(new VRCAvatarColliderJsonSerializer());
		}
	}
}

#endif
#endif