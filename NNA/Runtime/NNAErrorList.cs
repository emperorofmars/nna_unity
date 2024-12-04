/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/.
 */

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