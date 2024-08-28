#if UNITY_EDITOR

using UnityEditor;

namespace nna.ava.components
{
	[InitializeOnLoad]
	public class Register_AVA_Processors
	{
		static Register_AVA_Processors()
		{
			NNARegistry.RegisterProcessor(new AVA_AvatarProcessor(), AVA_AvatarProcessor._Type);
			NNARegistry.RegisterProcessor(new AVA_FacialTrackingProcessor(), AVA_FacialTrackingProcessor._Type);
			//NNARegistry.RegisterProcessor(new AVAEyetrackingVRChatProcessor(), AVAEyetrackingVRChatProcessor._Type);
		}
	}
}

#endif
