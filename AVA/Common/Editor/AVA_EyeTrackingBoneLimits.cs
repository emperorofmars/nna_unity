/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/.
 */

#if UNITY_EDITOR

using System.Text.RegularExpressions;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace nna.ava.common
{
	public static class EyeTrackingBoneLimits
	{
		private class EyeTrackingBoneLimitsResult
		{
			public Vector4 Result;
		}

		public const string _Type = "ava.eyetracking_bone_limits";
		public const string MatchExpression = @"(?i)\$EyeBoneLimits(?<up>[0-9]*[.][0-9]+),(?<down>[0-9]*[.][0-9]+),(?<in>[0-9]*[.][0-9]+),(?<out>[0-9]*[.][0-9]+)(?<side>([._\-|:][lr])|[._\-|:\s]?(right|left))?$";

		public static void ParseJsonToMessage(NNAContext Context, JObject Json)
		{
			(var limitsLeft, var limitsRight) = ParseJson(Json);
			Context.AddMessage(_Type + ".limits_left", limitsLeft);
			Context.AddMessage(_Type + ".limits_right", limitsRight);
		}

		public static void ParseNameDefinitionToMessage(NNAContext Context, string NameDefinition)
		{
			(Vector4 Limits, bool LeftMatch, bool RightMatch) = ParseNameDefinition(NameDefinition);
			if(LeftMatch) Context.AddMessage(_Type + ".limits_left", Limits);
			if(RightMatch) Context.AddMessage(_Type + ".limits_right", Limits);
		}

		public static (Vector4 LimitsLeft, Vector4 LimitsRight) ParseJson(JObject Json)
		{
			var limitsLeft = new Vector4(15.0f, 12.0f, 15.0f, 16.0f);
			var limitsRight = new Vector4(15.0f, 12.0f, 15.0f, 16.0f);

			limitsLeft.x = (float)ParseUtil.GetMultikeyOrDefault(Json, 15.0f, "left_up");
			limitsLeft.y = (float)ParseUtil.GetMultikeyOrDefault(Json, 12.0f, "left_down");
			limitsLeft.z = (float)ParseUtil.GetMultikeyOrDefault(Json, 15.0f, "left_in");
			limitsLeft.w = (float)ParseUtil.GetMultikeyOrDefault(Json, 16.0f, "left_out");

			if((bool)ParseUtil.GetMultikeyOrDefault(Json, true, "linked"))
			{
				limitsRight = limitsLeft;
			}
			else
			{
				limitsRight.x = (float)ParseUtil.GetMultikeyOrDefault(Json, 15.0f, "right_up");
				limitsRight.y = (float)ParseUtil.GetMultikeyOrDefault(Json, 12.0f, "right_down");
				limitsRight.z = (float)ParseUtil.GetMultikeyOrDefault(Json, 15.0f, "right_in");
				limitsRight.w = (float)ParseUtil.GetMultikeyOrDefault(Json, 16.0f, "right_out");
			}
			return (limitsLeft, limitsRight);
		}

		public static (Vector4 Limits, bool LeftMatch, bool RightMatch) ParseNameDefinition(string NameDefinition)
		{
			var limits = new Vector4(15.0f, 12.0f, 15.0f, 16.0f);
			bool leftMatch = false;
			bool rightMatch = false;

			var match = Regex.Match(NameDefinition, MatchExpression);

			if(ParseUtil.MatchSymmetrySide(NameDefinition) < 1) leftMatch = true;
			if(ParseUtil.MatchSymmetrySide(NameDefinition) > -1) rightMatch = true;

			limits.x = match.Groups["up"].Success ? float.Parse(match.Groups["up"].Value) : 15.0f;
			limits.y = match.Groups["down"].Success ? float.Parse(match.Groups["down"].Value) : 12.0f;
			limits.z = match.Groups["in"].Success ? float.Parse(match.Groups["in"].Value) : 15.0f;
			limits.w = match.Groups["out"].Success ? float.Parse(match.Groups["out"].Value) : 16.0f;

			return (limits, leftMatch, rightMatch);
		}

		public static bool LimitsExplicitelyDefined(NNAContext Context)
		{
			return Context.HasMessage(_Type + ".limits_left") && Context.HasMessage(_Type + ".limits_right");
		}

		public static (Vector4 LimitsLeft, Vector4 LimitsRight) GetLimitsOrDefault(NNAContext Context)
		{
			var limitsLeft = new Vector4(15.0f, 12.0f, 15.0f, 16.0f);
			var limitsRight = new Vector4(15.0f, 12.0f, 15.0f, 16.0f);

			if(LimitsExplicitelyDefined(Context))
			{
				limitsLeft = Context.GetMessage<Vector4>(_Type + ".limits_left");
				limitsRight = Context.GetMessage<Vector4>(_Type + ".limits_right");
			}
			return (limitsLeft, limitsRight);
		}
	}
}

#endif
