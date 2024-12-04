/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/.
 */

using System.Collections.Generic;

namespace nna.UnityToNNAUtils
{
	[System.Serializable]
	public struct SerializerResult
	{
		public string NNAType;
		public bool IsJsonComplete;
		public string DeviatingJsonType;
		public string JsonTargetNode;
		public string JsonComponentId;
		public string JsonResult;
		public bool IsNameComplete;
		public string DeviatingNameType;
		public string NameTargetNode;
		public string NameResult;
		public UnityEngine.Object Origin;
		public SerializerResultConfidenceLevel Confidence;
	}


	public enum SerializerResultConfidenceLevel
	{
		MANUAL,
		GENERATED
	}


	public class NNASerializerContext
	{
		public NNASerializerContext(List<UnityEngine.Object> UnityObjects)
		{
			foreach(var o in UnityObjects)
			{
				RegisterObject(o);
			}
		}
		public NNASerializerContext(UnityEngine.Object[] UnityObjects)
		{
			foreach(var o in UnityObjects)
			{
				RegisterObject(o);
			}
		}
		public NNASerializerContext(UnityEngine.Object UnityObject)
		{
			RegisterObject(UnityObject);
		}

		private readonly Dictionary<UnityEngine.Object, string> IdMap = new();

		private void RegisterObject(UnityEngine.Object UnityObject)
		{
			if(!IdMap.ContainsKey(UnityObject))
			{
				IdMap.Add(UnityObject, UnityObject.name + "_" + System.Guid.NewGuid().ToString().Split("-")[0]);
			}
		}
		
		public string GetId(UnityEngine.Object UnityObject)
		{
			if(IdMap.TryGetValue(UnityObject, out var ret)) return ret;
			else return UnityObject.name;
		}
	}

	/// <summary>
	/// NNA Json Serializers manually convert Unity Objects into NNA compatible Json.
	/// </summary>
	/// Once C# 11 becomes available in Unity, convert to using static virtual interface members (https://learn.microsoft.com/en-us/dotnet/csharp/whats-new/tutorials/static-virtual-interface-members)
	public interface INNASerializer
	{
		System.Type Target {get;}
		List<SerializerResult> Serialize(NNASerializerContext Context, UnityEngine.Object UnityObject);
	}
}
