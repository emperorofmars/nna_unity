using Newtonsoft.Json.Linq;
using UnityEngine;
using static nna.util.UnityHumanoidMappingUtil;

namespace nna.processors
{
	public class NNA_HumanoidSettings_JsonProcessor : IJsonProcessor
	{
		public const string _Type = "nna.humanoid.settings";
		public string Type => _Type;
		public const uint _Order = 0;
		public uint Order => _Order;
		public int Priority => 0;

		public void Process(NNAContext Context, Transform Node, JObject Json)
		{
			var ret = new HumanDescriptionSettings();
			if(Json.ContainsKey("upper_arm_twist")) ret.upperArmTwist = (float)Json["upper_arm_twist"];
			if(Json.ContainsKey("lower_arm_twist")) ret.lowerArmTwist = (float)Json["lower_arm_twist"];
			if(Json.ContainsKey("upper_leg_twist")) ret.upperLegTwist = (float)Json["upper_leg_twist"];
			if(Json.ContainsKey("lower_leg_twist")) ret.lowerLegTwist = (float)Json["lower_leg_twist"];
			if(Json.ContainsKey("arm_stretch")) ret.armStretch = (float)Json["arm_stretch"];
			if(Json.ContainsKey("leg_stretch")) ret.legStretch = (float)Json["leg_stretch"];
			if(Json.ContainsKey("feet_spacing")) ret.feetSpacing = (float)Json["feet_spacing"];
			if(Json.ContainsKey("translation_dof")) ret.hasTranslationDoF = (bool)Json["translation_dof"];

			Context.AddMessage(Node.name + ".settings", ret);
			if(Json.ContainsKey("id")) Context.AddResultById((string)Json["id"], ret);
		}
	}
}
