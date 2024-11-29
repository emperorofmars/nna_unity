using System.Collections.Generic;
using UnityEngine;

namespace nna
{
	public class NNAErrorList : ScriptableObject
	{
		[System.Serializable]
		public struct NNAErrorListEntry
		{
			public UnityEngine.Object Target;
			public string ProcessorType;
			public string Error;
		}
		public List<NNAErrorListEntry> Errors = new();
	}
}