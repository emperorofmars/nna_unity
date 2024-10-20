
using Newtonsoft.Json.Linq;
using nna.util;
using UnityEngine;

namespace nna.processors
{
	public class HumanoidMappingJsonProcessor : IJsonProcessor
	{
		public const string _Type = "humanoid";
		public string Type => _Type;

		public void ProcessJson(NNAContext Context, Transform Node, JObject Json)
		{
			var locomotionType = (string)ParseUtil.GetMulkikeyOrDefault(Json, "planti", "lt", "locomotion_type");
			var noJaw = (bool)ParseUtil.GetMulkikeyOrDefault(Json, false, "nj", "locomotion_type");

			CreateHumanoidMapping.Create(Context, Node, locomotionType, noJaw);
		}
	}

	public class HumanoidMappingNameProcessor : INameProcessor
	{
		public const string _Type = "humanoid";
		public string Type => _Type;
		
		public bool CanProcessName(NNAContext Context, Transform Node)
		{
			return Node.name.Contains("Humanoid");
		}

		public void ProcessName(NNAContext Context, Transform Node)
		{
			var locomotionType = Node.name.Contains("Digi") ? "digi" : "planti";
			var noJaw = Node.name.Contains("NoJaw");

			CreateHumanoidMapping.Create(Context, Node, locomotionType, noJaw);
		}

	}

	public static class CreateHumanoidMapping
	{
		public static void Create(NNAContext Context, Transform Node, string locomotionType, bool NoJaw)
		{
			Context.AddTask(new System.Threading.Tasks.Task(() => {
				var unityAvatar = UnityHumanoidMappingUtil.GenerateAvatar(Node.gameObject, Context.Root, locomotionType);
				unityAvatar.name = "Avatar";
				Context.AddObjectToAsset("avatar", unityAvatar);

				Animator animator = Context.Root.GetComponent<Animator>();
				if(!animator) animator = Context.Root.AddComponent<Animator>();

				animator.applyRootMotion = true;
				animator.updateMode = AnimatorUpdateMode.Normal;
				animator.cullingMode = AnimatorCullingMode.CullUpdateTransforms;
				animator.avatar = unityAvatar;
			}));
		}
	}
}
