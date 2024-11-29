
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
		public const string MatchNNANode = @"^\$([0-9]+)(.[0-9]+)?\$";
		//public const string MatchNNANode = @"^\$[0-9]+\$";

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
				var JsonString = ParseNodeRaw(Node, Trash);
				return JArray.Parse(JsonString);
			}
			return new JArray();
		}

		public static NNAMeta ParseMetaNode(Transform Node, List<Transform> Trash)
		{
			if(IsNNANode(Node))
			{
				var JsonString = ParseNodeRaw(Node, Trash);
				var metaJson = JObject.Parse(JsonString);
				var metaNNA = ScriptableObject.CreateInstance<NNAMeta>();
				metaNNA.name = "NNA Meta";
				foreach(var (key, value) in metaJson)
				{
					switch(key)
					{
						case "name": metaNNA.AssetName = (string)metaJson["name"]; break;
						case "author": metaNNA.Author = (string)metaJson["author"]; break;
						case "version": metaNNA.Version = (string)metaJson["version"]; break;
						case "url": metaNNA.URL = (string)metaJson["url"]; break;
						case "license": metaNNA.License = (string)metaJson["license"]; break;
						case "license_url": metaNNA.LicenseLink = (string)metaJson["license_url"]; break;
						case "documentation": metaNNA.Documentation = (string)metaJson["documentation"]; break;
						case "documentation_url": metaNNA.DocumentationLink = (string)metaJson["documentation_url"]; break;
						default: metaNNA.AdditionalProperties.Add(new NNAMeta.Entry{Key=key, Value=(string)value}); break;
					}
				}
				return metaNNA;
			}
			return null;
		}

		public static string ParseNodeRaw(Transform Node, List<Transform> Trash)
		{
			var NNAStrings = new List<(int, string)>();
			for(int childIdx = 0; childIdx < Node.childCount; childIdx++)
			{
				var child = Node.GetChild(childIdx);
				var match = Regex.Match(child.name, MatchNNANode);

				if(Regex.IsMatch(child.name, MatchNNANode))
				{
					var matchLen = match.Length;
					NNAStrings.Add((int.Parse(match.Groups[1].Value), child.name[matchLen..]));
					Trash.Add(child);
				}
			}
			return NNAStrings
				.OrderBy(s => s.Item1)
				.Select(s => s.Item2)
				.Aggregate((a, b) => a + b);
		}

		public const string MatchSideSignifier = @"(?i)(?<side>([._\-|:][lr])|[._\-|:\s]?(right|left))?$";
		public static string GetNameComponentId(string NodeName, int DefinitionStartIndex)
		{
			if(DefinitionStartIndex <= 0) return null;
			
			var match = Regex.Match(NodeName, MatchSideSignifier);
			var sideSignifier = match.Groups["side"].Success ? match.Groups["side"].Value : "";

			return NodeName[..DefinitionStartIndex] + sideSignifier;
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

		public static string GetPath(Transform root, Transform transform, bool relative = false)
		{
			string path = transform.name;
			while (transform.parent != root && transform.parent != null)
			{
				transform = transform.parent;
				path = "/" + transform.name + path;
			}
			if(relative) path = path.Substring(1);
			return path;
		}


		// targets get specified as elements of a path separated py splitChar.
		// For example `Armature$Hand.L` means the node is called `Hand.L` and it must have `Armature` as one of its ancestors.
		public static Transform FindNode(Transform Root, string TargetName, char splitChar = '$')
		{
			var targetPathRequirements = TargetName.Split(splitChar);
			var targetName = targetPathRequirements.Last();
			var satisfiedPathRequirements = 0;
			var targets = Root.transform.GetComponentsInChildren<Transform>().Where(t => t.name == targetName);
			Transform target = null;
			foreach(var t in targets)
			{
				var parent = t.parent;
				while(parent != null && satisfiedPathRequirements < targetPathRequirements.Length - 1)
				{
					if(targetPathRequirements.Contains(parent.name)) satisfiedPathRequirements++;
					parent = parent.parent;
				}
				if(satisfiedPathRequirements == targetPathRequirements.Length - 1)
				{
					target = t;
					break;
				}
				if(target != null) break;
			}
			return target;
		}

		public static Transform FindNodeNearby(Transform Node, string TargetName)
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

		
		public const string MatchLeft = @"(?i)(([._\-|:]l)|[._\-|:\s]?left)$";
		public const string MatchRight = @"(?i)(([._\-|:]r)|[._\-|:\s]?right)$";

		// TODO use enum
		public static int MatchSymmetrySide(string s)
		{
			if(Regex.IsMatch(s, MatchLeft)) return -1;
			else if(Regex.IsMatch(s, MatchRight)) return 1;
			else return 0;
		}
	}
}

