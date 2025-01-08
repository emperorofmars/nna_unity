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
		public string DocumentationLink;

		public List<Entry> CustomProperties = new();
	}
}
