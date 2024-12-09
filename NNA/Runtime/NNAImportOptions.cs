/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/.
 */

using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;

namespace nna
{
	/// <summary>
	/// Import settings which get serialized to a JSON string into the userData of an `ModelImporter`.
	/// This way these settings can persist.
	/// </summary>
	[System.Serializable]
	public class NNAImportOptions
	{
		public NNAImportOptions() {}

		public static NNAImportOptions Parse(string Json)
		{
			var nnaImportOptions = new NNAImportOptions();
			try
			{
				nnaImportOptions = JsonUtility.FromJson<NNAImportOptions>(Json);
			}
			catch
			{
			}
			return nnaImportOptions;
		}

		/// <summary>
		/// Enable the NNA Postprocessor.
		/// </summary>
		[SerializeField]
		private bool _NNAEnabled = true;
		public bool NNAEnabled {get => _NNAEnabled; set { if(value != _NNAEnabled) Modified = true; _NNAEnabled = value; }}

		/// <summary>
		/// The import context.
		/// </summary>
		[SerializeField]
		private string _SelectedContext = NNARegistry.DefaultContext;
		public string SelectedContext {get => _SelectedContext; set { if(value != _SelectedContext) Modified = true; _SelectedContext = value; }}

		/// <summary>
		/// Strip NNA Json definitions from the imported hierarchy.
		/// </summary>
		[SerializeField]
		private bool _RemoveNNADefinitions = true;
		public bool RemoveNNADefinitions {get => _RemoveNNADefinitions; set { if(value != _RemoveNNADefinitions) Modified = true; _RemoveNNADefinitions = value; }}

		/// <summary>
		/// Abort the PostProcessor, reverting all its changes made to the model, if an Exception is thrown, which isn't derived from NNAException.
		/// </summary>
		[SerializeField]
		private bool _AbortOnException = false;
		public bool AbortOnException {get => _AbortOnException; set { if(value != _AbortOnException) Modified = true; _AbortOnException = value; }}

		[System.Serializable]
		public class ContextImportOptions
		{
			[System.Serializable]
			public class ContextImportOption
			{
				public string Key;
				public string Value;
			}

			public string Context;
			public readonly List<ContextImportOption> Options = new();
		}

		/// <summary>
		/// Custom import options for contexts.
		/// </summary>
		[SerializeField]
		public readonly List<ContextImportOptions> ContextImportOptionsList = new();

		[IgnoreDataMember]
		public bool Modified {get; private set;} = false;
	}
}
