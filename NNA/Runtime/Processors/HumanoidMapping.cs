
using Newtonsoft.Json.Linq;
using nna.util;
using UnityEngine;

namespace nna.processors
{
	public class HumanoidMapping : IProcessor
	{
		public const string _Type = "humanoid";
		public string Type => _Type;

		public void Process(NNAContext Context, GameObject Target, GameObject NNANode, JObject Json)
		{
			var locomotionType = (string)ParseUtil.GetMulkikeyOrDefault(Json, "planti", "lt", "locomotion_type");

			Context.AddTask(new System.Threading.Tasks.Task(() => {
				var unityAvatar = UnityHumanoidMappingUtil.GenerateAvatar(NNANode, Context.Root, locomotionType);
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
