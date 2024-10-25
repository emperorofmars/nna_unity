
using Newtonsoft.Json.Linq;
using nna.util;
using UnityEngine;

namespace nna.processors
{
	public class HumanoidMappingJsonProcessor : IJsonProcessor
	{
		public const string _Type = "humanoid";
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
		public const string _Type = "humanoid";
		public string Type => _Type;

		public bool CanProcessName(NNAContext Context, string Name)
		{
			return Name.ToLower().Contains(_Type);
		}

		public void Process(NNAContext Context, Transform Node, string Name)
		{
			var definition = Name.ToLower()[Name.ToLower().IndexOf(_Type) ..];
			var locomotionType = definition.Contains("digi") ? "digi" : "planti";
			var noJaw = definition.Contains("nojaw");

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
