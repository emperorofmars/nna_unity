
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace nna
{
	public enum NNAValueType
	{
		Null, Bool, String, Int, Float, Reference
	}
	public class NNAValue
	{
		public NNAValue(NNAValueType ValueType, object Value) { this.ValueType = ValueType; this.Value = Value; }
		public NNAValueType ValueType { get; private set; }
		public object Value { get; private set; }
	}

	public static class ParseUtil
	{
		public static bool IsNNANode(string NodeName)
		{
			return NodeName.Contains("$nna:") && !NodeName.StartsWith("$$");
		}
		public static string GetActualNodeName(string NodeName)
		{
			return NodeName.Substring(0, NodeName.IndexOf("$nna:")).Trim();;
		}
		public static string GetNNAString(string NodeName)
		{
			return NodeName.Substring(NodeName.IndexOf("$nna:") + 5).Trim();;
		}
		public static string GetNNAType(string NodeName)
		{
			var NNAString = GetNNAString(NodeName);
			if(NNAString.StartsWith("$multinode")) return "$multinode";
			else return NNAString.Substring(0, NNAString.IndexOf(':')).Trim();;
		}
		public static string GetNNADefinition(string NodeName)
		{
			var NNAString = GetNNAString(NodeName);
			return NNAString.Substring(NNAString.IndexOf(':') + 1).Trim();;
		}

		public static Dictionary<string, Dictionary<string, NNAValue>> ParseNode(GameObject Root, GameObject Node, List<Transform> Trash)
		{
			var ret = new Dictionary<string, Dictionary<string, NNAValue>>();
			if(IsNNANode(Node.name))
			{
				var actualNodeName = GetActualNodeName(Node.name);
				var NNAType = GetNNAType(Node.name);
				var NNAString = GetNNAString(Node.name);
				string fullDefinition;
				if(NNAType == "$multinode")
				{
					int numLen = 2;
					if(NNAString.StartsWith("$multinode:")) numLen = int.Parse(NNAString.Substring(11));
					fullDefinition = CombineMultinodeDefinition(Node, numLen, Trash);
				}
				else
				{
					fullDefinition = Node.name;
				}
				var components = fullDefinition.Contains(';') ? fullDefinition.Split(';') : new string[] {fullDefinition};
				foreach(var component in components)
				{
					ret.Add(GetNNAType(component), ParseSingle(Root, Node, GetNNADefinition(component)));
				}
				if(string.IsNullOrWhiteSpace(actualNodeName)) Trash.Add(Node.transform);
			}
			return ret;
		}

		private static string CombineMultinodeDefinition(GameObject NNANode, int NumLen, List<Transform> Trash)
		{
			List<string> NNAStrings = new List<string>();
			for(int childIdx = 0; childIdx < NNANode.transform.childCount; childIdx++)
			{
				var child = NNANode.transform.GetChild(childIdx);
				if(child.name.StartsWith("$$"))
				{
					NNAStrings.Add(child.name);
					Trash.Add(child);
				}
			}
			return NNAStrings
				.OrderByDescending(s => int.Parse(s.Substring(2, NumLen)))
				.Select(s => s.Substring(2 + NumLen))
				.Select(s => s.EndsWith(',') ? s : s + ',')
				.Aggregate((a, b) => a + b);
		}

		private static Dictionary<string, NNAValue> ParseSingle(GameObject Root, GameObject NNANode, string NNADefinition)
		{
			var ret = new Dictionary<string, NNAValue>();
			string[] properties = NNADefinition.Split(',');

			foreach(var property in properties)
			{
				if(property.Length == 0) continue;
				
				var propertyName = property.Substring(0, property.IndexOf(':')).Trim();

				if(property.Length < propertyName.Length + 3)
				{
					ret.Add(propertyName, new NNAValue(NNAValueType.Null, null));
					continue;
				}

				var propertyValue = property.Substring(property.IndexOf(':') + 1).Trim();
				
				if(propertyValue == "t" || propertyValue == "f" || propertyValue == "true" || propertyValue == "false")
				{
					ret.Add(propertyName, new NNAValue(NNAValueType.Bool, propertyValue == "t" || propertyValue == "true"));
				}
				else if(propertyValue == "null")
				{
					ret.Add(propertyName, new NNAValue(NNAValueType.Null, null));
				}
				else if(propertyValue.StartsWith("$ref:"))
				{
					var path = propertyValue.Substring(5).Split('/');
					Transform location = NNANode.transform;
					foreach(var part in path)
					{
						if(string.IsNullOrEmpty(part))
						{
							location = Root.transform;
							continue;
						}
						var partProcessed = IsNNANode(part) ? GetActualNodeName(part) : part;
						if(partProcessed == "..")
						{
							location = location.parent;
							if(location == null) throw new Exception($"Invalid ref path in: {NNANode.name} (No parent node)");
						}
						else
						{
							location = location.Find(partProcessed);
							if(location == null) throw new Exception($"Invalid ref path in: {NNANode.name} (No child node named {partProcessed})");
						}
					}
					ret.Add(propertyName, new NNAValue(NNAValueType.Reference, location.gameObject));
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
