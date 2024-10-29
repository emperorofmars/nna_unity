#if UNITY_EDITOR

using System.Linq;
using System.Threading.Tasks;
using nna.processors;
using UnityEditor;
using UnityEngine;
using VRM;

namespace nna.ava.univrm0
{
	public class UNIVRM0EyeTrackingBones : IGlobalProcessor
	{
		public const string _Type = "ava.eyetracking";
		public string Type => _Type;

		public void Process(NNAContext Context)
		{
			var Json = Context.GetJsonComponent(Context.Root.transform, _Type);

			Context.AddTask(new Task(() => {
				var avatar = Context.Root.GetComponent<VRMMeta>();
				var animator = Context.Root.GetComponent<Animator>();
				
				// set eyebones if human
				if(animator.isHuman)
				{
					var humanEyeL = animator.avatar.humanDescription.human.FirstOrDefault(hb => hb.humanName == HumanBodyBones.LeftEye.ToString());
					var humanEyeR = animator.avatar.humanDescription.human.FirstOrDefault(hb => hb.humanName == HumanBodyBones.RightEye.ToString());
					
					if(humanEyeL.boneName != null && humanEyeR.boneName != null)
					{
						var eyeL = Context.Root.GetComponentsInChildren<Transform>().FirstOrDefault(t => t.name == humanEyeL.boneName);
						var eyeR = Context.Root.GetComponentsInChildren<Transform>().FirstOrDefault(t => t.name == humanEyeR.boneName);

						var vrmLookat = Context.Root.AddComponent<VRMLookAtBoneApplyer>();
						vrmLookat.LeftEye.Transform = eyeL;
						vrmLookat.RightEye.Transform = eyeR;
				
						// vrmLookat.VerticalDown = // TODO
						// etc

						return;
					}
				}
			}));
		}
	}

	[InitializeOnLoad]
	public class Register_UNIVRM0EyeTrackingBones
	{
		static Register_UNIVRM0EyeTrackingBones()
		{
			NNARegistry.RegisterGlobalProcessor(new UNIVRM0EyeTrackingBones(), DetectorUNIVRM0.NNA_UNIVRM0_CONTEXT, true);
		}
	}
}

#endif