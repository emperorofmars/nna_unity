#if AVA_VRCSDK3_FOUND
#if UNITY_EDITOR

using System.Collections.Generic;

namespace nna.ava.vrchat
{
	// Very placeholder-y implementation, final system is going to work more comprehensively, with layers of functionality fallback.
	public static class AVAVRCRegistry
	{
		public static readonly Dictionary<string, IAVAFeatureVRC> DefaultFeatures = new() {
			{VRCEyeTrackingBones._Type, new VRCEyeTrackingBones()},
			{VRCEyelidTrackingBlendshapes._Type, new VRCEyelidTrackingBlendshapes()}
		};

		public static Dictionary<string, IAVAFeatureVRC> Features {get => DefaultFeatures;}
	}
}

#endif
#endif
