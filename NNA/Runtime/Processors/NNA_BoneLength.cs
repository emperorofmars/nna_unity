using System.Text.RegularExpressions;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace nna.processors
{
	public class NNA_BoneLength_JsonProcessor : IJsonProcessor
	{
		public const string _Type = "nna.bone_length";
		public string Type => _Type;
		public const uint _Order = 0;
		public uint Order => _Order;
		public int Priority => 0;

		public void Process(NNAContext Context, Transform Node, JObject Json)
		{
			Context.AddMessage(Node.name + ".bone_length", (float)Json["length"]);
		}
	}

	public class NNA_BoneLength_NameProcessor : INameProcessor
	{
		public const string _Type = "nna.bone_length";
		public string Type => _Type;
		public const uint _Order = 0;
		public uint Order => _Order;

		public const string Match = @"(?i)\$BoneLen(?<bone_length>[0-9]*([.][0-9]+)?)(?<side>([._\-|:][lr])|[._\-|:\s]?(right|left))?$";

		public int CanProcessName(NNAContext Context, string Name)
		{
			var match = Regex.Match(Name, Match);
			return match.Success ? match.Index : -1;
		}

		public void Process(NNAContext Context, Transform Node, string Name)
		{
			var match = Regex.Match(Name, Match);
			Context.AddMessage(Node.name + ".bone_length", float.Parse(match.Groups["bone_length"].Value));
		}
	}
}
