#if UNITY_EDITOR

using System.Text.RegularExpressions;
using nna.processors;
using UnityEngine;

namespace nna.ava.common
{
	public abstract class Base_AVA_Collider_NameProcessor : INameProcessor
	{
		public const string _Type = "ava.collider";
		public string Type => _Type;
		public const uint _Order = 100; // Run after most constraint types would
		public uint Order => _Order;

		public const string _Match_Sphere = @"(?i)\$ColSphere(?<inside_bounds>In)?(?<radius>R[0-9]*[.][0-9]+)(?<side>(([._\-|:][lr])|[._\-|:\s]?(right|left))$)?$";
		public const string _Match_Capsule = @"(?i)\$ColCapsule(?<inside_bounds>In)?(?<radius>R[0-9]*[.][0-9]+)(?<height>H[0-9]*[.][0-9]+)(?<side>(([._\-|:][lr])|[._\-|:\s]?(right|left))$)?$";
		public const string _Match_Plane = @"(?i)\$ColPlane(?<inside_bounds>In)?(?<side>(([._\-|:][lr])|[._\-|:\s]?(right|left))$)?$";

		public int CanProcessName(NNAContext Context, string Name)
		{
			{ if(Regex.Match(Name, _Match_Sphere) is var match && match.Success) return match.Index; }
			{ if(Regex.Match(Name, _Match_Capsule) is var match && match.Success) return match.Index; }
			{ if(Regex.Match(Name, _Match_Plane) is var match && match.Success) return match.Index; }
			return -1;
		}

		public void Process(NNAContext Context, Transform Node, string Name)
		{
			{
				if(Regex.Match(Name, _Match_Sphere) is var match && match.Success)
				{
					Context.AddResultById(
						ParseUtil.GetNameComponentId(Name),
						BuildSphereCollider(Context, Node,
							match.Groups["inside_bounds"].Success,
							float.Parse(match.Groups["radius"].Value[1..])
						)
					);
					return;
				}
			}
			{
				if(Regex.Match(Name, _Match_Capsule) is var match && match.Success)
				{
					Context.AddResultById(
						ParseUtil.GetNameComponentId(Name),
						BuildCapsuleCollider(Context, Node,
							match.Groups["inside_bounds"].Success,
							float.Parse(match.Groups["radius"].Value[1..]),
							float.Parse(match.Groups["height"].Value[1..])
						)
					);
					return;
				}
			}
			{
				if(Regex.Match(Name, _Match_Plane) is var match && match.Success)
				{
					Context.AddResultById(
						ParseUtil.GetNameComponentId(Name),
						BuildPlaneCollider(Context, Node)
					);
					return;
				}
			}
		}

		protected abstract object BuildSphereCollider(NNAContext Context, Transform Node, bool InsideBounds, float Radius);
		protected abstract object BuildCapsuleCollider(NNAContext Context, Transform Node, bool InsideBounds, float Radius, float Height);
		protected abstract object BuildPlaneCollider(NNAContext Context, Transform Node);
	}
}

#endif
