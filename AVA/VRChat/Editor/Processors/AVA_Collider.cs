#if UNITY_EDITOR
#if NNA_AVA_VRCSDK3_FOUND

using System.Text.RegularExpressions;
using nna.processors;
using UnityEditor;
using UnityEngine;
using VRC.SDK3.Dynamics.PhysBone.Components;

namespace nna.ava.vrchat
{
	public class AVA_Collider_VRCNameProcessor : INameProcessor
	{
		public const string _Type = "ava.collider";
		public string Type => _Type;
		public const uint _Order = 0;
		public uint Order => _Order;

		public const string _Match_Sphere = @"(?i)ColSphere(?<inside_bounds>In)?(?<radius>R[0-9]*[.][0-9]+)(?<side>(([._\-|:][lr])|[._\-|:\s]?(right|left))$)?$";
		public const string _Match_Capsule = @"(?i)ColCapsule(?<inside_bounds>In)?(?<radius>R[0-9]*[.][0-9]+)(?<height>H[0-9]*[.][0-9]+)(?<side>(([._\-|:][lr])|[._\-|:\s]?(right|left))$)?$";
		public const string _Match_Plane = @"(?i)ColPlane(?<inside_bounds>In)?(?<side>(([._\-|:][lr])|[._\-|:\s]?(right|left))$)?$";

		public int CanProcessName(NNAContext Context, string Name)
		{
			{ if(Regex.Match(Name, _Match_Sphere) is var match && match.Success) return match.Index; }
			{ if(Regex.Match(Name, _Match_Capsule) is var match && match.Success) return match.Index; }
			{ if(Regex.Match(Name, _Match_Plane) is var match && match.Success) return match.Index; }
			return -1;
		}

		public void Process(NNAContext Context, Transform Node, string Name)
		{
			{ if(Regex.Match(Name, _Match_Sphere) is var match && match.Success) BuildSphereCollider(Context, Node, match); }
			{ if(Regex.Match(Name, _Match_Capsule) is var match && match.Success) BuildCapsuleCollider(Context, Node, match); }
			{ if(Regex.Match(Name, _Match_Plane) is var match && match.Success) BuildPlaneCollider(Context, Node, match); }
		}

		private static void BuildSphereCollider(NNAContext Context, Transform Node, Match NameMatch)
		{
			var collider = Node.gameObject.AddComponent<VRCPhysBoneCollider>();
			collider.shapeType = VRC.Dynamics.VRCPhysBoneColliderBase.ShapeType.Sphere;
			collider.insideBounds = NameMatch.Groups["inside_bounds"].Success;
			collider.radius = float.Parse(NameMatch.Groups["radius"].Value[1..]);
			
			Context.AddResultById(ParseUtil.GetNameComponentId(Node.name, NameMatch.Index), collider);
		}

		private static void BuildCapsuleCollider(NNAContext Context, Transform Node, Match NameMatch)
		{
			var collider = Node.gameObject.AddComponent<VRCPhysBoneCollider>();
			collider.shapeType = VRC.Dynamics.VRCPhysBoneColliderBase.ShapeType.Capsule;
			collider.insideBounds = NameMatch.Groups["inside_bounds"].Success;
			collider.radius = float.Parse(NameMatch.Groups["radius"].Value[1..]);
			collider.height = float.Parse(NameMatch.Groups["height"].Value[1..]);
			
			Context.AddResultById(ParseUtil.GetNameComponentId(Node.name, NameMatch.Index), collider);
		}

		private static void BuildPlaneCollider(NNAContext Context, Transform Node, Match NameMatch)
		{
			var collider = Node.gameObject.AddComponent<VRCPhysBoneCollider>();
			collider.shapeType = VRC.Dynamics.VRCPhysBoneColliderBase.ShapeType.Plane;
			collider.insideBounds = NameMatch.Groups["inside_bounds"].Success;

			Context.AddResultById(ParseUtil.GetNameComponentId(Node.name, NameMatch.Index), collider);
		}
	}

	/*public class AVA_Collider_VRCJsonProcessor : IJsonProcessor
	{
		public const string _Type = "ava.collider";
		public string Type => _Type;
		public const uint _Order = 0;
		public uint Order => _Order;

		public void Process(NNAContext Context, Transform Node, JObject Json)
		{
			var collider = Node.gameObject.AddComponent<VRCPhysBoneCollider>();
			if(Json.ContainsKey("id") && ((string)Json["id"]).Length > 0) collider.name = (string)Json["id"];

			JsonUtility.FromJsonOverwrite(Json["parsed"].ToString(), collider);
		}
	}

	public class AVA_Collider_VRCSerializer : INNASerializer
	{
		public static readonly System.Type _Target = typeof(VRCPhysBoneCollider);
		public System.Type Target => _Target;

		public List<SerializerResult> Serialize(NNASerializerContext Context, UnityEngine.Object UnityObject)
		{
			var collider = (VRCPhysBoneCollider)UnityObject;
			var retJson = new JObject {
				{"t", AVA_Collider_VRCJsonProcessor._Type},
				{"pos_offset", TRSUtil.SerializeVector3(collider.position)},
				{"rot_offset", TRSUtil.SerializeQuat(collider.rotation)}
			};
			if(UnityObject.name.StartsWith("$nna:")) retJson.Add("id", UnityObject.name[5..]);

			var parsed = JObject.Parse(JsonUtility.ToJson(collider));
			parsed.Remove("rootTransform");
			retJson.Add("parsed", parsed);

			return new List<SerializerResult>{new(){
				NNAType = AVA_Collider_VRCJsonProcessor._Type,
				Origin = UnityObject,
				JsonResult = retJson.ToString(Newtonsoft.Json.Formatting.None),
				JsonComponentId = Context.GetId(UnityObject),
				JsonTargetNode = collider.rootTransform ? collider.rootTransform.name : collider.transform.name,
				IsJsonComplete = true,
				Confidence = SerializerResultConfidenceLevel.MANUAL,
			}};
		}
	}*/

	[InitializeOnLoad]
	public class Register_AVA_Collider_VRC
	{
		static Register_AVA_Collider_VRC()
		{
			NNARegistry.RegisterNameProcessor(new AVA_Collider_VRCNameProcessor(), DetectorVRC.NNA_VRC_AVATAR_CONTEXT);
			//NNARegistry.RegisterJsonProcessor(new AVA_Collider_VRCJsonProcessor(), DetectorVRC.NNA_VRC_AVATAR_CONTEXT);
			//NNAExportRegistry.RegisterSerializer(new AVA_Collider_VRCSerializer());
		}
	}
}

#endif
#endif