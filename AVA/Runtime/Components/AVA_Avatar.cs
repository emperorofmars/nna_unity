
using Newtonsoft.Json.Linq;
using nna.processors;
using nna.util;
using UnityEngine;

namespace nna.ava.components
{
	public class AVA_Avatar : MonoBehaviour
	{
		public GameObject ViewportParent;
		public Vector3 ViewportOffset;
	}

	
	public class AVA_AvatarProcessor : IProcessor
	{
		public const string _Type = "ava.avatar";
		public string Type => _Type;

		public void Process(NNAContext Context, GameObject Target, GameObject NNANode, JObject Json)
		{
			var avatar = Context.Root.AddComponent<AVA_Avatar>();
			
			if(ParseUtil.HasMulkikey(Json, "viewport_parent", "vp")) avatar.ViewportParent = ParseUtil.ResolvePath(Context.Root.transform, Target.transform, (string)ParseUtil.GetMulkikey(Json, "viewport_parent", "vp"));
			if(ParseUtil.HasMulkikey(Json, "viewport_offset", "vo")) avatar.ViewportOffset = TRSUtil.ParseVector3((JArray)ParseUtil.GetMulkikey(Json, "viewport_offset", "vo"));
		}
	}
}