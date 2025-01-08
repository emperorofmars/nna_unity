using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace nna
{
	public static class ParseUtil
	{
		public const string MatchNNANode = @"^\$([0-9]+)(.[0-9]+)?\$";

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
						case "custom_properties":
							foreach(var customProperty in (JObject)value)
							{
								metaNNA.CustomProperties.Add(new NNAMeta.Entry{Key=(string)customProperty.Key, Value=(string)customProperty.Value});
							}
							break;
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
		public static string GetNameComponentId(string NodeName)
		{
			var DefinitionStartIndex = NodeName.IndexOf("$");
			if(DefinitionStartIndex == 0) return NodeName;
			if(DefinitionStartIndex < 0) return NodeName;

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
			string path = GetNodeNameCleaned(transform.name);
			while (transform.parent != root && transform.parent != null)
			{
				transform = transform.parent;
				path = "/" + GetNodeNameCleaned(transform.name) + path;
			}
			if(relative) path = path[1..];
			return path;
		}


		public const string MatchSide = @"(?i)([._\-|:][lr])|([._\-|:\s]?(right|left))$";
		public static (string Name, string SideSuffix) SplitSideSignifier(string Name)
		{
			var sideMatch = Regex.Match(Name, MatchSide);
			if(Name.Contains("$$") && Name.IndexOf("$") == Name.IndexOf("$$"))
			{
				return (Name[..Name.IndexOf("$$")], sideMatch.Success ? sideMatch.Value : "");
			}
			else
			{
				if(sideMatch.Success) Name = Name[..(Name.Length - sideMatch.Length)];
				return (Name, sideMatch.Success ? sideMatch.Value : "");
			}
		}

		public static string GetNodeNameCleaned(string Name)
		{
			(var name, var sideSuffix) = SplitSideSignifier(Name);
			return name + sideSuffix;
		}


		// targets get specified as elements of a path separated py splitChar.
		// For example `Armature;Hand.L` means the node is called `Hand.L` and it must have `Armature` as one of its ancestors.
		public static Transform FindNode(Transform Root, string TargetName, char splitChar = ';')
		{
			var targetPathRequirements = TargetName.Split(splitChar).Reverse().ToList();

			foreach(var t in Root.transform.GetComponentsInChildren<Transform>())
			{
				var satisfiedPathRequirements = 0;
				var parent = t;

				foreach(var req in targetPathRequirements)
				{
					while(parent != null && satisfiedPathRequirements < targetPathRequirements.Count())
					{
						if(GetNodeNameCleaned(req) == GetNodeNameCleaned(parent.name))
						{
							satisfiedPathRequirements++;
							break;
						}
						parent = parent.parent;
					}
				}
				if(satisfiedPathRequirements == targetPathRequirements.Count()) return t;
			}
			return null;
		}

		public static Transform FindNodeNearby(Transform Node, string TargetName)
		{
			var parent = Node.parent;
			while(parent != null)
			{
				if(GetNodeNameCleaned(parent.name) == GetNodeNameCleaned(TargetName)) return parent;
				parent = parent.parent;
			}
			foreach(var c in Node.parent.GetComponentsInChildren<Transform>())
			{
				if(GetNodeNameCleaned(c.name) == GetNodeNameCleaned(TargetName)) return c;
			}
			return null;
		}

		public static bool HasMultikey(JObject Json, params string[] Keys)
		{
			foreach(var key in Keys)
			{
				if(Json.ContainsKey(key)) return true;
			}
			return false;
		}

		public static JToken GetMultikey(JObject Json, params string[] Keys)
		{
			foreach(var key in Keys)
			{
				if(Json.ContainsKey(key)) return Json[key];
			}
			return null;
		}

		public static JToken GetMultikeyOrDefault(JObject Json, JToken DefaultValue, params string[] Keys)
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

