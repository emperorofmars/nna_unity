/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/.
 */

using System.Collections.Generic;

namespace nna.UnityToNNAUtils
{
	/// <summary>
	/// Register your own NNA Json Serializer here.
	/// </summary>
	public static class NNAExportRegistry
	{
		public static readonly List<INNASerializer> DefaultSerializers = new() {
			new NNA_Twist_Serializer(),
		};

		private static readonly List<INNASerializer> RegisteredSerializers = new();

		public static void RegisterSerializer(INNASerializer Serializer) { RegisteredSerializers.Add(Serializer); }

		public static List<INNASerializer> Serializers { get {
			var ret = new List<INNASerializer>(DefaultSerializers);
			ret.AddRange(RegisteredSerializers);
			return ret;
		}}
	}
}
