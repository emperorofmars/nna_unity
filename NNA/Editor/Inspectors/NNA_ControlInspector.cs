
#if UNITY_EDITOR

using System.Collections.Generic;
using nna.applicationconversion;
using UnityEditor;

namespace nna.tools
{
	[CustomEditor(typeof(NNA_Control))]
	public class NNA_ControlInspector : Editor
	{
		private readonly List<IApplicationConverter> Converters = new();

		public void OnEnable()
		{
			var c = (NNA_Control)target;
			foreach(var ac in NNARegistry.ApplicationConverters)
			{
				if(ac.CanConvert(c.gameObject)) Converters.Add(ac);
			}
		}

		public override void OnInspectorGUI()
		{
			var c = (NNA_Control)target;

			if(Converters.Count > 0)
			{
				EditorGUILayout.LabelField("Convert to", EditorStyles.whiteLargeLabel);
				foreach(var converter in Converters)
				{
					EditorGUILayout.LabelField(converter.Name);
				}
			}
			else
			{
				EditorGUILayout.LabelField("No suitable NNA converters present!", EditorStyles.whiteLargeLabel);
			}
		}
	}
}

#endif