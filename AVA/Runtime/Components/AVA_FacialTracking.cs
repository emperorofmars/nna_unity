
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using nna.processors;
using UnityEngine;

namespace nna.ava.components
{
	public class AVA_FacialTracking : MonoBehaviour
	{
		public List<string> VoiceVisemes = new();
		public List<string> EyeRotations = new();
		public List<string> EyeLids = new();
		public List<string> FacialTracking = new();
	}
	
	public class AVA_FacialTrackingProcessor : IProcessor
	{
		public const string _Type = "ava.facial_tracking";
		public string Type => _Type;

		public void Process(NNAContext Context, GameObject Target, GameObject NNANode, JObject Json)
		{
			var component = Context.Root.AddComponent<AVA_FacialTracking>();
			
			/*if(ParseUtil.HasMulkikey(Json, "viewport_parent", "vp")) avatar.ViewportParent = ParseUtil.ResolvePath(Context.Root.transform, Target.transform, (string)ParseUtil.GetMulkikey(Json, "viewport_parent", "vp"));
			if(ParseUtil.HasMulkikey(Json, "viewport_offset", "vo")) avatar.ViewportOffset = TRSUtil.ParseVector3((JArray)ParseUtil.GetMulkikey(Json, "viewport_offset", "vo"));*/
		}
	}
}