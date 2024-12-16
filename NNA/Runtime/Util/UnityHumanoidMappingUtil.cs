using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace nna.util
{
	public static class UnityHumanoidMappingUtil
	{
		private static readonly List<string> _MappingsLeftList = new() { "left", "_l", ".l", "-l"};
		private static readonly List<string> _MappingsRightList = new() { "right", "_r", ".r", "-r"};
		private static readonly Dictionary<string, List<List<string>>> NameMappings = new()
		{
			{HumanBodyBones.Hips.ToString(), new List<List<string>>{new() { "hip", "hips"}}},
			{HumanBodyBones.Spine.ToString(), new List<List<string>>{new() { "spine"}}},
			{HumanBodyBones.Chest.ToString(), new List<List<string>>{new() { "chest"}}},
			{HumanBodyBones.UpperChest.ToString(), new List<List<string>>{new() { "upper"}, new() { "chest"}}},
			{HumanBodyBones.Neck.ToString(), new List<List<string>>{new() { "neck"}}},
			{HumanBodyBones.Head.ToString(), new List<List<string>>{new() { "head"}}},
			{HumanBodyBones.Jaw.ToString(), new List<List<string>>{new() { "jaw"}}},
			{HumanBodyBones.LeftEye.ToString(), new List<List<string>>{new() { "eye"}, _MappingsLeftList}},
			{HumanBodyBones.RightEye.ToString(), new List<List<string>>{new() { "eye"}, _MappingsRightList}},
			{HumanBodyBones.LeftShoulder.ToString(), new List<List<string>>{new() { "shoulder"}, _MappingsLeftList}},
			{HumanBodyBones.LeftUpperArm.ToString(), new List<List<string>>{new() { "upper"}, new() { "arm"}, _MappingsLeftList}},
			{HumanBodyBones.LeftLowerArm.ToString(), new List<List<string>>{new() { "lower"}, new() { "arm"}, _MappingsLeftList}},
			{HumanBodyBones.LeftHand.ToString(), new List<List<string>>{new() { "hand", "wrist"}, _MappingsLeftList}},
			{"Left Thumb Proximal", new List<List<string>>{new() { "thumb"}, new() { "1", "proximal"}, _MappingsLeftList}},
			{"Left Thumb Intermediate", new List<List<string>>{new() { "thumb"}, new() { "2", "intermediate"}, _MappingsLeftList}},
			{"Left Thumb Distal", new List<List<string>>{new() { "thumb"}, new() { "3", "distal"}, _MappingsLeftList}},
			{"Left Index Proximal", new List<List<string>>{new() { "index"}, new() { "1", "proximal"}, _MappingsLeftList}},
			{"Left Index Intermediate", new List<List<string>>{new() { "index"}, new() { "2", "intermediate"}, _MappingsLeftList}},
			{"Left Index Distal", new List<List<string>>{new() { "index"}, new() { "3", "distal"}, _MappingsLeftList}},
			{"Left Middle Proximal", new List<List<string>>{new() { "middle"}, new() { "1", "proximal"}, _MappingsLeftList}},
			{"Left Middle Intermediate", new List<List<string>>{new() { "middle"}, new() { "2", "intermediate"}, _MappingsLeftList}},
			{"Left Middle Distal", new List<List<string>>{new() { "middle"}, new() { "3", "distal"}, _MappingsLeftList}},
			{"Left Ring Proximal", new List<List<string>>{new() { "ring"}, new() { "1", "proximal"}, _MappingsLeftList}},
			{"Left Ring Intermediate", new List<List<string>>{new() { "ring"}, new() { "2", "intermediate"}, _MappingsLeftList}},
			{"Left Ring Distal", new List<List<string>>{new() { "ring"}, new() { "3", "distal"}, _MappingsLeftList}},
			{"Left Little Proximal", new List<List<string>>{new() { "little", "pinkie"}, new() { "1", "proximal"}, _MappingsLeftList}},
			{"Left Little Intermediate", new List<List<string>>{new() { "little", "pinkie"}, new() { "2", "intermediate"}, _MappingsLeftList}},
			{"Left Little Distal", new List<List<string>>{new() { "little", "pinkie"}, new() { "3", "distal"}, _MappingsLeftList}},
			{HumanBodyBones.RightShoulder.ToString(), new List<List<string>>{new() { "shoulder"}, _MappingsRightList}},
			{HumanBodyBones.RightUpperArm.ToString(), new List<List<string>>{new() { "upper"}, new() { "arm"}, _MappingsRightList}},
			{HumanBodyBones.RightLowerArm.ToString(), new List<List<string>>{new() { "lower"}, new() { "arm"}, _MappingsRightList}},
			{HumanBodyBones.RightHand.ToString(), new List<List<string>>{new() { "hand", "wrist"}, _MappingsRightList}},
			{"Right Thumb Proximal", new List<List<string>>{new() { "thumb"}, new() { "1", "proximal"}, _MappingsRightList}},
			{"Right Thumb Intermediate", new List<List<string>>{new() { "thumb"}, new() { "2", "intermediate"}, _MappingsRightList}},
			{"Right Thumb Distal", new List<List<string>>{new() { "thumb"}, new() { "3", "distal"}, _MappingsRightList}},
			{"Right Index Proximal", new List<List<string>>{new() { "index"}, new() { "1", "proximal"}, _MappingsRightList}},
			{"Right Index Intermediate", new List<List<string>>{new() { "index"}, new() { "2", "intermediate"}, _MappingsRightList}},
			{"Right Index Distal", new List<List<string>>{new() { "index"}, new() { "3", "distal"}, _MappingsRightList}},
			{"Right Middle Proximal", new List<List<string>>{new() { "middle"}, new() { "1", "proximal"}, _MappingsRightList}},
			{"Right Middle Intermediate", new List<List<string>>{new() { "middle"}, new() { "2", "intermediate"}, _MappingsRightList}},
			{"Right Middle Distal", new List<List<string>>{new() { "middle"}, new() { "3", "distal"}, _MappingsRightList}},
			{"Right Ring Proximal", new List<List<string>>{new() { "ring"}, new() { "1", "proximal"}, _MappingsRightList}},
			{"Right Ring Intermediate", new List<List<string>>{new() { "ring"}, new() { "2", "intermediate"}, _MappingsRightList}},
			{"Right Ring Distal", new List<List<string>>{new() { "ring"}, new() { "3", "distal"}, _MappingsRightList}},
			{"Right Little Proximal", new List<List<string>>{new() { "little", "pinkie"}, new() { "1", "proximal"}, _MappingsRightList}},
			{"Right Little Intermediate", new List<List<string>>{new() { "little", "pinkie"}, new() { "2", "intermediate"}, _MappingsRightList}},
			{"Right Little Distal", new List<List<string>>{new() { "little", "pinkie"}, new() { "3", "distal"}, _MappingsRightList}},
			{HumanBodyBones.LeftUpperLeg.ToString(), new List<List<string>>{new() { "upper"}, new() { "leg"}, _MappingsLeftList}},
			{HumanBodyBones.LeftLowerLeg.ToString(), new List<List<string>>{new() { "lower"}, new() { "leg"}, _MappingsLeftList}},
			{HumanBodyBones.LeftFoot.ToString(), new List<List<string>>{new() { "foot"}, _MappingsLeftList}},
			{HumanBodyBones.LeftToes.ToString(), new List<List<string>>{new() { "toes"}, _MappingsLeftList}},
			{HumanBodyBones.RightUpperLeg.ToString(), new List<List<string>>{new() { "upper"}, new() { "leg"}, _MappingsRightList}},
			{HumanBodyBones.RightLowerLeg.ToString(), new List<List<string>>{new() { "lower"}, new() { "leg"}, _MappingsRightList}},
			{HumanBodyBones.RightFoot.ToString(), new List<List<string>>{new() { "foot"}, _MappingsRightList}},
			{HumanBodyBones.RightToes.ToString(), new List<List<string>>{new() { "toes"}, _MappingsRightList}}
		};

		private static string TranslateHumanoidSTFtoUnity(string STFName, string LocomotionType, bool NoJaw)
		{
			if(LocomotionType.StartsWith("digi"))
			{
				switch(STFName)
				{
					case "LeftToes":
						return HumanBodyBones.LeftFoot.ToString();
					case "RightToes":
						return HumanBodyBones.RightFoot.ToString();
					case "LeftFoot":
						return null;
					case "RightFoot":
						return null;
				}
			}
			if(NoJaw && STFName == "Jaw") return null;
			return STFName;
		}

		private static Dictionary<string, GameObject> Map(Transform[] Bones)
		{
			var mappings = new Dictionary<string, GameObject>();
			//var skeleton = new List<Transform>();
			foreach(var bone in Bones)
			{
				foreach(var mapping in NameMappings)
				{
					var and_list = mapping.Value;
					var and_condition = true;
					foreach(var or_list in and_list)
					{
						var or_condition = false;
						foreach(var or_arg in or_list)
						{
							if(bone.name.ToLower().Contains(or_arg))
							{
								or_condition = true;
								break;
							}
						}
						if(!or_condition)
						{
							and_condition = false;
						}
					}
					if(and_condition)
					{
						if(mappings.ContainsKey(mapping.Key))
						{
							if(mappings[mapping.Key].name.Length > bone.name.Length)
							{
								mappings[mapping.Key] = bone.gameObject;
							}
						}
						else
						{
							mappings.Add(mapping.Key, bone.gameObject);
						}
					}
				}
			}
			return mappings;
		}

		public static Avatar GenerateAvatar(Transform ArmatureRootNode, Transform RootNode, string LocomotionType, bool NoJaw)
		{
			var potentialBoneList = RootNode.GetComponentsInChildren<Transform>().Where(t => !t.name.StartsWith('$')).ToArray();

			var mappings = Map(potentialBoneList).ToList()
					.FindAll(mapping => !string.IsNullOrWhiteSpace(mapping.Key) && mapping.Value != null)
					.Select(mapping => new KeyValuePair<string, GameObject>(TranslateHumanoidSTFtoUnity(mapping.Key, LocomotionType, NoJaw), mapping.Value))
					.Where(mapping => !string.IsNullOrWhiteSpace(mapping.Key)).ToList();

			var humanDescription = new HumanDescription
			{
				armStretch = 0.05f,
				feetSpacing = 0f,
				hasTranslationDoF = false,
				legStretch = 0.05f,
				lowerArmTwist = 0.5f,
				lowerLegTwist = 0.5f,
				upperArmTwist = 0.5f,
				upperLegTwist = 0.5f,
				skeleton = potentialBoneList.Select(t => {
					return new SkeletonBone()
					{
						name = t.name,
						position = t.localPosition,
						rotation = t.localRotation,
						scale = t.localScale,
					};
				}).ToArray(),
				human = mappings.Select(mapping => {
					var bone = new HumanBone {
						humanName = mapping.Key,
						boneName = mapping.Value.name,
						limit = new HumanLimit {useDefaultValues = true},
					};
					return bone;
				}).ToArray(),
			};

			var avatar = AvatarBuilder.BuildHumanAvatar(RootNode.gameObject, humanDescription);
			avatar.name = ArmatureRootNode.name + "Avatar";

			if (!avatar.isValid)
			{
				throw new System.Exception("Invalid humanoid avatar");
			}
			return avatar;
		}
	}
}
