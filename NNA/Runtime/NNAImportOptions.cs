using System.Runtime.Serialization;
using UnityEngine;

namespace nna
{
	/// <summary>
	/// Import settings which get serialized to a JSON string into the userData of an `ModelImporter`.
	/// This way these settings can persist.
	/// </summary>
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
		private bool _RemoveNNAJson = true;
		public bool RemoveNNAJson {get => _RemoveNNAJson; set { if(value != _RemoveNNAJson) Modified = true; _RemoveNNAJson = value; }}

		[IgnoreDataMember]
		public bool Modified {get; private set;} = false;
	}
}
