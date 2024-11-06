
using System.Text.RegularExpressions;
using Newtonsoft.Json.Linq;
using nna.util;
using UnityEngine;

namespace nna.processors
{
	public class HumanoidMappingJsonProcessor : IJsonProcessor
	{
		public const string _Type = "nna.humanoid";
		public string Type => _Type;

		public void Process(NNAContext Context, Transform Node, JObject Json)
		{
			var locomotionType = (string)ParseUtil.GetMulkikeyOrDefault(Json, "planti", "lt", "locomotion_type");
			var noJaw = (bool)ParseUtil.GetMulkikeyOrDefault(Json, false, "nj", "no_jaw");

			CreateHumanoidMapping.Create(Context, Node, locomotionType, noJaw);
		}
	}

	public class HumanoidMappingNameProcessor : INameProcessor
	{
		public const string _Type = "nna.humanoid";
		public string Type => _Type;

		public const string Match = @"(?i)humanoid(?<digi>digi)?(?<no_jaw>nojaw)?(([._\-|:][lr])|[._\-|:\s]?(right|left))?$";

		public bool CanProcessName(NNAContext Context, string Name)
		{
			return Regex.IsMatch(Name, Match);
		}

		public void Process(NNAContext Context, Transform Node, string Name)
		{
			var match = Regex.Match(Name, Match);
			var locomotionType = match.Groups["digi"].Success ? "digi" : "planti";
			var noJaw = match.Groups["no_jaw"].Success;

			CreateHumanoidMapping.Create(Context, Node, locomotionType, noJaw);
		}
	}

	public static class CreateHumanoidMapping
	{
		public static void Create(NNAContext Context, Transform Node, string locomotionType, bool NoJaw)
		{
			var unityAvatar = UnityHumanoidMappingUtil.GenerateAvatar(Node.gameObject, Context.Root, locomotionType, NoJaw);
			unityAvatar.name = "Avatar";
			Context.AddObjectToAsset("avatar", unityAvatar);

			Animator animator = Context.Root.GetComponent<Animator>();
			if(!animator) animator = Context.Root.AddComponent<Animator>();

			animator.applyRootMotion = true;
			animator.updateMode = AnimatorUpdateMode.Normal;
			animator.cullingMode = AnimatorCullingMode.CullUpdateTransforms;
			animator.avatar = unityAvatar;
		}
	}
}
