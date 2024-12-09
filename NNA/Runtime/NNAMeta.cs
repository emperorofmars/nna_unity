/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/.
 */

using System.Collections.Generic;
using UnityEngine;

namespace nna
{
	public class NNAMeta : ScriptableObject
	{
		[System.Serializable]
		public class Entry { public string Key; public string Value; }

		public string AssetName;
		public string Author;
		public string Version;
		public string URL;
		public string License;
		public string LicenseLink;
		public string Documentation;
		public string DocumentationLink;

		public List<Entry> AdditionalProperties = new();
	}
}
