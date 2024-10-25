#if AVA_VRCSDK3_FOUND
#if UNITY_EDITOR

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using nna.processors;
using nna.UnityToNNAUtils;
using nna.util;
using UnityEditor;
using UnityEngine;
using VRC.SDK3.Avatars.Components;

namespace nna.ava.vrchat
{
	public class VRCAvatarColliders : IJsonProcessor
	{
		public const string _Type = "vrc.avatar_colliders";
		public string Type => _Type;

		public void Process(NNAContext Context, Transform Node, JObject Json)
		{
			Context.AddTask(new Task(() => {
				var avatar = Context.Root.GetComponent<VRCAvatarDescriptor>();
				if(!avatar) return;

				ParseVRCCollider(avatar.collider_head, Json["head"]);
				ParseVRCCollider(avatar.collider_torso, Json["torso"]);
				ParseVRCCollider(avatar.collider_footL, Json["footL"]);
				ParseVRCCollider(avatar.collider_footR, Json["footR"]);
				ParseVRCCollider(avatar.collider_handL, Json["handL"]);
				ParseVRCCollider(avatar.collider_handR, Json["handR"]);
				ParseVRCCollider(avatar.collider_fingerIndexL, Json["fingerIndexL"]);
				ParseVRCCollider(avatar.collider_fingerIndexR, Json["fingerIndexR"]);
				ParseVRCCollider(avatar.collider_fingerMiddleL, Json["fingerMiddleL"]);
				ParseVRCCollider(avatar.collider_fingerMiddleR, Json["fingerMiddleR"]);
				ParseVRCCollider(avatar.collider_fingerRingL, Json["fingerRingL"]);
				ParseVRCCollider(avatar.collider_fingerRingR, Json["fingerRingR"]);
				ParseVRCCollider(avatar.collider_fingerLittleL, Json["fingerLittleL"]);
				ParseVRCCollider(avatar.collider_fingerLittleR, Json["fingerLittleR"]);
			}));
		}

		private static void ParseVRCCollider(VRCAvatarDescriptor.ColliderConfig ColliderConfig, JToken ColliderDef)
		{
			if(ColliderDef != null)
			{
				if(ColliderDef.Type == JTokenType.Boolean)
				{
					ColliderConfig.state = (bool)ColliderDef ? VRCAvatarDescriptor.ColliderConfig.State.Automatic : VRCAvatarDescriptor.ColliderConfig.State.Disabled;
				}
				else
				{
					ColliderConfig.state = VRCAvatarDescriptor.ColliderConfig.State.Custom;
					if(((JObject)ColliderDef).ContainsKey("m")) ColliderConfig.isMirrored = (bool)ColliderDef["m"];
					if(((JObject)ColliderDef).ContainsKey("r")) ColliderConfig.radius = (float)ColliderDef["r"];
					if(((JObject)ColliderDef).ContainsKey("h")) ColliderConfig.height = (float)ColliderDef["h"];
					if(((JObject)ColliderDef).ContainsKey("pos"))ColliderConfig.position = TRSUtil.ParseVector3((JArray)ColliderDef["pos"]);
					if(((JObject)ColliderDef).ContainsKey("rot")) ColliderConfig.rotation = TRSUtil.ParseQuat((JArray)ColliderDef["rot"]);
				}
			}
		}
	}

	public class VRCAvatarColliderJsonSerializer : INNAJsonSerializer
	{
		public Type Target => typeof(VRCAvatarDescriptor);

		public List<string> Serialize(UnityEngine.Object UnityObject)
		{
			var ret = new JObject {{"t", "vrc.avatar_colliders"}};
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

			return new List<string>{ret.ToString(Newtonsoft.Json.Formatting.Indented)};
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
			NNARegistry.RegisterJsonProcessor(new VRCAvatarColliders(), VRCAvatarColliders._Type, DetectorVRC.NNA_VRC_AVATAR_CONTEXT);
			NNAJsonExportRegistry.RegisterSerializer(new VRCAvatarColliderJsonSerializer());
		}
	}
}

#endif
#endif