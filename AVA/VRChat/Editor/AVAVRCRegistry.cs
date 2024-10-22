#if AVA_VRCSDK3_FOUND
#if UNITY_EDITOR

using System.Collections.Generic;

namespace nna.ava.vrchat
{
	public static class AVAVRChatFeatures
	{
		public static readonly Dictionary<string, IAVAFeature> Features = new() {
			{VRCEyeTrackingBones._Type, new VRCEyeTrackingBones()},
			{VRCEyelidTrackingBlendshapes._Type, new VRCEyelidTrackingBlendshapes()}
		};
	}
}

#endif
#endif
