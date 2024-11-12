
using System.Collections.Generic;
using UnityEngine;

namespace nna
{
	public class NNAMeta : ScriptableObject
	{
		public class Entry { public string Key; public string Value; }

		public string AssetName;
		public string Author;
		public string Version;
		public string URL;
		public string License;
		public string LicenseLink;
		public string Documentation;
		public string DocumentationLink;

		public readonly List<Entry> AdditionalProperties = new();
	}
}
