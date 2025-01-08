using System.Collections.Generic;
using UnityEngine;

namespace nna
{
	public class NNAErrorList : ScriptableObject
	{
		public List<NNAReport> Reports = new();
	}
}
