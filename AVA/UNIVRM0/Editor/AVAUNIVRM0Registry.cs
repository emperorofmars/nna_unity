#if NNA_AVA_UNIVRM0_FOUND
#if UNITY_EDITOR

using System.Collections.Generic;

namespace nna.ava.univrm0
{
	// Very placeholder-y implementation, final system is going to work more comprehensively, with layers of functionality fallback.
	public static class AVAUNIVRM0Registry
	{
		public static readonly Dictionary<string, IAVAFeatureUNIVRM0> DefaultFeatures = new() {
			{UNIVRM0VisemesBlendshapes._Type, new UNIVRM0VisemesBlendshapes()},
		};

		private static readonly Dictionary<string, IAVAFeatureUNIVRM0> RegisteredFeatures = new();

		public static void RegisterFeature(string Type, IAVAFeatureUNIVRM0 Feature) { RegisteredFeatures.Add(Type, Feature); }

		public static Dictionary<string, IAVAFeatureUNIVRM0> Features {get {
			var ret = new Dictionary<string, IAVAFeatureUNIVRM0>(DefaultFeatures);
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
