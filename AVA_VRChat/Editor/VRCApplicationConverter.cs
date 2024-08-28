#if UNITY_EDITOR
#if AVA_VRCSDK3_FOUND

using nna.applicationconversion;
using nna.ava.components;
using UnityEditor;
using UnityEngine;

namespace nna.ava.applicationconversion.vrc
{
	public class VRCApplicationConverter : IApplicationConverter
	{
		public const string _Name = "VRChat";
		public string Name => _Name;

		public bool CanConvert(GameObject Root)
		{
			return Root.GetComponent<AVA_Avatar>() != null;
		}

		public GameObject Convert(GameObject Root)
		{
			throw new System.NotImplementedException();
		}
	}
	
	[InitializeOnLoad]
	public class Register_AVA_Processors
	{
		static Register_AVA_Processors()
		{
			NNARegistry.RegisterApplicationConverter(new VRCApplicationConverter());
		}
	}
}



#endif
#endif
