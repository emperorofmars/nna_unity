#if UNITY_EDITOR

using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEditor;

namespace nna.util
{
	public static class AssetResourceUtil
	{
		public static T FindAsset<T>(NNAContext Context, string SearchPattern, bool AssetsOnly = true, string Suffix = null) where T : UnityEngine.Object
		{
			var search_path = "Assets/";
			if(Context.GetAssetMappingBaseDir() is var mappingBase && !string.IsNullOrEmpty(mappingBase))
				search_path += mappingBase;

			var resolvedSearchPattern = SearchPattern;
			if(Regex.Matches(SearchPattern, @"\{(?<key>[^\{\}]+)\}") is var matchGroup && matchGroup.Count > 0)
				foreach(Match match in matchGroup)
					if(match.Success)
						resolvedSearchPattern = resolvedSearchPattern.Replace(match.Value, Context.GetMetaCustomValue(match.Groups["key"].Value) ?? "");

			if(Suffix != null && !Suffix.StartsWith('.')) Suffix = '.' + Suffix;
			var resultPaths = AssetDatabase.FindAssets(resolvedSearchPattern)
				.Select(guid => AssetDatabase.GUIDToAssetPath(guid))
				.Where(r => (Suffix == null || r.ToLower().EndsWith(Suffix.ToLower())) && (!AssetsOnly || r.StartsWith(search_path)))
				.OrderBy(r => Path.GetFileNameWithoutExtension(r).Length);

			if(resultPaths.Count() > 0 && resultPaths.First() is var path)
			{
				return AssetDatabase.LoadAssetAtPath<T>(path);
			}
			else return null;
		}
	}

}

#endif
