
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine.Analytics;
using UnityEngine.Rendering;

namespace nna
{
	public enum NNAValueType
	{
		Null, String, Int, Float, Reference, Json
	}
	public class NNAValue
	{
		public NNAValue(NNAValueType ValueType, object Value) { this.ValueType = ValueType; this.Value = Value; }
		public NNAValueType ValueType { get; private set; }
		public object Value { get; private set; }
	}

	public static class ParseUtil
	{
		public static bool HasNNAType(string NodeName)
		{
			return NodeName.Contains("$nna:");
		}
		public static string GetActualNodeName(string NodeName)
		{
			return NodeName.Substring(0, NodeName.IndexOf("$nna:"));
		}
		public static string GetNNAString(string NodeName)
		{
			return NodeName.Substring(NodeName.IndexOf("$nna:") + 4);
		}
		public static string GetNNAType(string NodeName)
		{
			var NNAString = GetNNAString(NodeName);
			return NNAString.Substring(0, NNAString.IndexOf(':'));
		}
		public static Dictionary<string, NNAValue> GetNNADefinition(string NodeName)
		{
			var NNAString = GetNNAString(NodeName);
			return ParseNNADefinition(NNAString.Substring(NNAString.IndexOf(':') + 1));
		}
		public static (string ActualNodeName, string NNAType, Dictionary<string, NNAValue> NNADefinition) ParseNNAName(string NodeName)
		{
			return (GetActualNodeName(NodeName), GetNNAType(NodeName), GetNNADefinition(NodeName));
		}

		public static Dictionary<string, NNAValue> ParseNNADefinition(string NNADefinition)
		{
			var ret = new Dictionary<string, NNAValue>();
			var properties = NNADefinition.Split(';');
			foreach(var property in properties)
			{
				var propertyName = property.Substring(0, property.IndexOf('='));

				if(property.Length < propertyName.Length + 3)
				{
					ret.Add(propertyName, new NNAValue(NNAValueType.Null, null));
					continue;
				}

				var propertyValue = property.Substring(property.IndexOf('=') + 1);
				
				if(propertyValue == "null")
				{
					ret.Add(propertyName, new NNAValue(NNAValueType.Null, null));
				}
				else if(propertyValue.StartsWith("$ref:"))
				{
					// parse reference
				}
				else if(propertyValue.StartsWith("$json:"))
				{
					// parse json from child nodes
				}
				else if(propertyValue.StartsWith('"') && propertyValue.EndsWith('"'))
				{
					ret.Add(propertyName, new NNAValue(NNAValueType.String, propertyValue.Substring(1, propertyValue.Length - 2)));
				}
				else if(int.TryParse(propertyValue, out var resultInt))
				{
					ret.Add(propertyName, new NNAValue(NNAValueType.Int, resultInt));
				}
				else if(float.TryParse(propertyValue, out var resultFloat))
				{
					ret.Add(propertyName, new NNAValue(NNAValueType.Float, resultFloat));
				}
				else
				{
					throw new Exception($"Property \"{propertyName}\" has invalid value: {propertyValue}");
				}
			}
			return ret;
		}
	}
}
