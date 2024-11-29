#if UNITY_EDITOR

using System.Text.RegularExpressions;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace nna.ava.common
{
	public static class EyeTrackingBoneLimits
	{
		public const string _Type = "ava.eyetracking_bone_limits";
		public const string MatchExpression = @"(?i)EyeBoneLimits(?<up>[0-9]*[.][0-9]+),(?<down>[0-9]*[.][0-9]+),(?<in>[0-9]*[.][0-9]+),(?<out>[0-9]*[.][0-9]+)(?<side>([._\-|:][lr])|[._\-|:\s]?(right|left))?$";

		public static (Vector4 LimitsLeft, Vector4 LimitsRight) ParseGlobal(NNAContext Context)
		{
			var Json = Context.GetJsonComponentByNode(Context.Root.transform, _Type);
			if(Json != null) return ParseJson(Json);
			else return ParseNameGlobal(Context);
		}

		public static (Vector4 LimitsLeft, Vector4 LimitsRight) ParseJson(JObject Json)
		{
			var limitsLeft = new Vector4(15.0f, 12.0f, 15.0f, 16.0f);
			var limitsRight = new Vector4(15.0f, 12.0f, 15.0f, 16.0f);

			limitsLeft.x = (float)ParseUtil.GetMulkikeyOrDefault(Json, 15.0f, "left_up");
			limitsLeft.y = (float)ParseUtil.GetMulkikeyOrDefault(Json, 12.0f, "left_down");
			limitsLeft.z = (float)ParseUtil.GetMulkikeyOrDefault(Json, 15.0f, "left_in");
			limitsLeft.w = (float)ParseUtil.GetMulkikeyOrDefault(Json, 16.0f, "left_out");

			if((bool)ParseUtil.GetMulkikeyOrDefault(Json, true, "linked"))
			{
				limitsRight = limitsLeft;
			}
			else
			{
				limitsRight.x = (float)ParseUtil.GetMulkikeyOrDefault(Json, 15.0f, "right_up");
				limitsRight.y = (float)ParseUtil.GetMulkikeyOrDefault(Json, 12.0f, "right_down");
				limitsRight.z = (float)ParseUtil.GetMulkikeyOrDefault(Json, 15.0f, "right_in");
				limitsRight.w = (float)ParseUtil.GetMulkikeyOrDefault(Json, 16.0f, "right_out");
			}
			return (limitsLeft, limitsRight);
		}
		
		public static (Vector4 LimitsLeft, Vector4 LimitsRight) ParseNameGlobal(NNAContext Context)
		{
			var limitsLeft = new Vector4(15.0f, 12.0f, 15.0f, 16.0f);
			var limitsRight = new Vector4(15.0f, 12.0f, 15.0f, 16.0f);
			
			// This is a bit stupid.
			// TODO create a system for processors to set information for another procesor.
			// That way the a name processor could do this when matched in NNAConverter, and store the information in the NNAContext for this global processor.
			foreach(var t in Context.Root.GetComponentsInChildren<Transform>())
			{
				if(Regex.IsMatch(t.name, MatchExpression))
				{
					var match = Regex.Match(t.name, MatchExpression);
					if(ParseUtil.MatchSymmetrySide(t.name) < 1)
					{
						limitsLeft.x = match.Groups["up"].Success ? float.Parse(match.Groups["up"].Value) : 15.0f;
						limitsLeft.y = match.Groups["down"].Success ? float.Parse(match.Groups["down"].Value) : 12.0f;
						limitsLeft.z = match.Groups["in"].Success ? float.Parse(match.Groups["in"].Value) : 15.0f;
						limitsLeft.w = match.Groups["out"].Success ? float.Parse(match.Groups["out"].Value) : 16.0f;
					}
					if(ParseUtil.MatchSymmetrySide(t.name) > -1)
					{
						limitsRight.x = match.Groups["up"].Success ? float.Parse(match.Groups["up"].Value) : 15.0f;
						limitsRight.y = match.Groups["down"].Success ? float.Parse(match.Groups["down"].Value) : 12.0f;
						limitsRight.z = match.Groups["in"].Success ? float.Parse(match.Groups["in"].Value) : 15.0f;
						limitsRight.w = match.Groups["out"].Success ? float.Parse(match.Groups["out"].Value) : 16.0f;
					}
					
					if(Context.ImportOptions.RemoveNNAJson && match.Length == t.name.Length) Context.AddTrash(t);
				}
			}
			return (limitsLeft, limitsRight);
		}
	}
}

#endif