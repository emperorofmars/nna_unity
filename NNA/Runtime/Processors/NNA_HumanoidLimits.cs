using System.Text.RegularExpressions;
using Newtonsoft.Json.Linq;
using nna.util;
using UnityEngine;

namespace nna.processors
{
	public class NNA_HumanoidLimits_JsonProcessor : IJsonProcessor
	{
		public const string _Type = "nna.humanoid.limits";
		public string Type => _Type;
		public const uint _Order = 0;
		public uint Order => _Order;
		public int Priority => 0;

		public void Process(NNAContext Context, Transform Node, JObject Json)
		{
			//var locomotionType = (string)ParseUtil.GetMultikeyOrDefault(Json, "planti", "lt", "locomotion_type");
			//var noJaw = (bool)ParseUtil.GetMultikeyOrDefault(Json, false, "nj", "no_jaw");

			//var converted = CreateHumanoidMapping.Create(Context, Node, locomotionType, noJaw);
			//if(Json.ContainsKey("id")) Context.AddResultById((string)Json["id"], converted);
		}
	}

	public class NNA_HumanoidLimits_NameProcessor : INameProcessor
	{
		public const string _Type = "nna.humanoid.limits";
		public string Type => _Type;
		public const uint _Order = 0;
		public uint Order => _Order;

		public const string Match = @"(?i)\$HuLim(?<limits>[SVTF][-]?[0-9]+([.][0-9]*)?,[-]?[0-9]+([.][0-9]*)?){1,3}(?<bone_length>BL[0-9]*([.][0-9]+)?)?(?<side>([._\-|:][lr])|[._\-|:\s]?(right|left))?$";
		public const string MatchLimit = @"(?i)(?<axis>[SVTF]+)(?<min>[-]?[0-9]*([.][0-9]+)?),(?<max>[-]?[0-9]*([.][0-9]+)?)";

		public int CanProcessName(NNAContext Context, string Name)
		{
			var match = Regex.Match(Name, Match);
			return match.Success ? match.Index : -1;
		}

		public void Process(NNAContext Context, Transform Node, string Name)
		{
			var match = Regex.Match(Name, Match);

			var ret = new HumanLimit {useDefaultValues = true};

			var min = new Vector3();
			var max = new Vector3();
			foreach(var capture in match.Groups["limits"].Captures)
			{
				var matchLimit = Regex.Match(capture.ToString(), MatchLimit);
				var axis = matchLimit.Groups["axis"].Value;

				switch(axis.ToUpper())
				{
					case "P":
						min.z = float.Parse(matchLimit.Groups["min"].Value);
						max.z = float.Parse(matchLimit.Groups["max"].Value);
						break;
					case "S":
						min.y = float.Parse(matchLimit.Groups["min"].Value);
						max.y = float.Parse(matchLimit.Groups["max"].Value);
						break;
					case "T":
						min.x = float.Parse(matchLimit.Groups["min"].Value);
						max.x = float.Parse(matchLimit.Groups["max"].Value);
						break;
				}

			}
			ret.min = min;
			ret.max = max;

			if(match.Groups["bone_length"].Success)
			{
				ret.axisLength = float.Parse(match.Groups["bone_length"].Value[2..]);
			}

			//Context.AddMessage(Node.name, ret);
			if(ParseUtil.GetNameComponentId(Name) is var componentId && componentId != null) Context.AddResultById(componentId, ret);
		}
	}
}
