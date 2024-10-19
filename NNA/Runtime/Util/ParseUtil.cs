
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Newtonsoft.Json.Linq;
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
		public const string MatchNNANode = @"^\$[0-9]+\$";

		public static bool IsNNANode(Transform Node)
		{
			for(int childIdx = 0; childIdx < Node.childCount; childIdx++)
			{
				var child = Node.GetChild(childIdx);
				if(Regex.IsMatch(child.name, MatchNNANode)) return true;
			}
			return false;
		}

		public static JArray ParseNode(Transform Node, List<Transform> Trash)
		{
			if(IsNNANode(Node))
			{
				List<(int, string)> NNAStrings = new List<(int, string)>();
				for(int childIdx = 0; childIdx < Node.childCount; childIdx++)
				{
					var child = Node.GetChild(childIdx);
					if(Regex.IsMatch(child.name, MatchNNANode))
					{
						var matchLen = Regex.Match(child.name, MatchNNANode).Length;
						NNAStrings.Add((int.Parse(child.name.Substring(1, matchLen-2)), child.name.Substring(matchLen)));
						Trash.Add(child);
					}
				}
				var JsonString = NNAStrings
					.OrderBy(s => s.Item1)
					.Select(s => s.Item2)
					.Aggregate((a, b) => a + b);
					
				return JArray.Parse(JsonString);
			}
			return new JArray();
		}

		public static Transform ResolvePath(Transform Root, Transform NNANode, string Path)
		{
			Transform location = NNANode;
			foreach(var part in Path.Split('/'))
			{
				if(string.IsNullOrEmpty(part))
				{
					location = Root;
					continue;
				}
				if(part == "..")
				{
					location = location.parent;
					if(location == null) throw new Exception($"Invalid ref path in: {NNANode.name} (No parent node)");
				}
				else
				{
					location = location.Find(part);
					if(location == null) throw new Exception($"Invalid ref path in: {NNANode.name} (No child node named {part})");
				}
			}
			return location;
		}

		public static Transform FindNode(Transform Node, string TargetName)
		{
			var parent = Node.parent;
			while(parent != null)
			{
				if(parent.name == TargetName) return parent;
				parent = parent.parent;
			}
			foreach(var c in Node.parent.GetComponentsInChildren<Transform>())
			{
				if(c.name == TargetName) return c;
			}
			return null;
		}

		public static bool HasMulkikey(JObject Json, params string[] Keys)
		{
			foreach(var key in Keys)
			{
				if(Json.ContainsKey(key)) return true;
			}
			return false;
		}

		public static JToken GetMulkikey(JObject Json, params string[] Keys)
		{
			foreach(var key in Keys)
			{
				if(Json.ContainsKey(key)) return Json[key];
			}
			return null;
		}

		public static JToken GetMulkikeyOrDefault(JObject Json, JToken DefaultValue, params string[] Keys)
		{
			foreach(var key in Keys)
			{
				if(Json.ContainsKey(key)) return Json[key];
			}
			return DefaultValue;
		}
	}
}

