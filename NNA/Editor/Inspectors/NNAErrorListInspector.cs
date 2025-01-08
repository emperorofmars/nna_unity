#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;

namespace nna
{
	[CustomEditor(typeof(NNAErrorList), true)]
	public class NNAErrorListInspector : Editor
	{
		public override void OnInspectorGUI()
		{
			var errorList = (NNAErrorList)target;

			int index = 0;
			foreach(var report in errorList.Reports)
			{
				EditorGUILayout.LabelField($"Report {index} : {report.Severity}");
				EditorGUI.indentLevel++;

				EditorGUILayout.BeginHorizontal();
				EditorGUILayout.PrefixLabel("Processor Type");
				EditorGUILayout.LabelField(report.ProcessorType ?? "-");
				EditorGUILayout.EndHorizontal();

				if(report.Node)
				{
					EditorGUILayout.BeginHorizontal();
					EditorGUILayout.PrefixLabel("Affected GameObject");
					EditorGUILayout.ObjectField(report.Node, typeof(NNAErrorList), false);
					EditorGUILayout.EndHorizontal();
				}

				EditorGUILayout.TextArea(report.Message ?? "No Message Added!");

				EditorGUI.indentLevel--;
				if(index < errorList.Reports.Count - 1) GUILayout.Space(10);
				index++;
			}
		}
	}
}

#endif
