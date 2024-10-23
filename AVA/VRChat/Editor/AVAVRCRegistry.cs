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

		private static readonly Dictionary<string, IAVAFeatureVRC> RegisteredFeatures = new();

		public static void RegisterFeature(string Type, IAVAFeatureVRC Feature) { RegisteredFeatures.Add(Type, Feature); }

		public static Dictionary<string, IAVAFeatureVRC> Features {get {
			var ret = new Dictionary<string, IAVAFeatureVRC>(DefaultFeatures);
			foreach(var entry in RegisteredFeatures)
			{
				if(ret.ContainsKey(entry.Key)) ret[entry.Key] = entry.Value;
				else ret.Add(entry.Key, entry.Value);
			}
			return ret;
		}}
	}
}

#endif
#endif
