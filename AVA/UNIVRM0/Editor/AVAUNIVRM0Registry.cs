#if NNA_AVA_UNIVRM0_FOUND
#if UNITY_EDITOR

using System.Collections.Generic;

namespace nna.ava.univrm0
{
	// Very placeholder-y implementation, final system is going to work more comprehensively, with layers of functionality fallback.
	public static class AVAUNIVRM0Registry
	{
		public static readonly Dictionary<string, IAVAFeatureUNIVRM0> DefaultFeatures = new() {
		};

		public static Dictionary<string, IAVAFeatureUNIVRM0> Features {get => DefaultFeatures;}
	}
}

#endif
#endif
