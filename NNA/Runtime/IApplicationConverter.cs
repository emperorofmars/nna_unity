using UnityEngine;

namespace nna.applicationconversion
{
	public interface IApplicationConverter
	{
		string Name {get;}
		bool CanConvert(GameObject Root);
		GameObject Convert(GameObject Root);
	}
}