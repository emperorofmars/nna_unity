
using System.Collections.Generic;

namespace nna.UnityToNNAUtils
{
	public static class NNAJsonExportRegistry
	{
		public static readonly List<INNAJsonSerializer> DefaultSerializers = new() {
			new TwistBoneExporter(),
		};

		private static readonly List<INNAJsonSerializer> RegisteredSerializers = new();

		public static void RegisterSerializer(INNAJsonSerializer Serializer) { RegisteredSerializers.Add(Serializer); }

		public static List<INNAJsonSerializer> Serializers { get {
			var ret = new List<INNAJsonSerializer>(DefaultSerializers);
			ret.AddRange(RegisteredSerializers);
			return ret;
		}}
	}
}
