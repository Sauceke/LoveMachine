using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;

namespace LoveMachinePrototyper
{
    internal static class FindUtil
    {
        public static T[] FindAll<T>(string pattern)
            where T : Object
        {
            return PrototyperConfig.UseRegexes.Value
                ? GameObject.FindObjectsOfType<T>()
                    .Where(go => MatchesEntireName(go.name, pattern))
                    .ToArray()
                : typeof(T) == typeof(GameObject)
                    ? new[] { GameObject.Find(pattern) as T }
                    : new[] { GameObject.Find(pattern)?.GetComponent<T>() };
        }

        public static T FindFirst<T>(string pattern)
            where T : Object
        {
            return FindAll<T>(pattern).FirstOrDefault();
        }

        private static bool MatchesEntireName(string name, string pattern)
        {
            var match = Regex.Match(name, pattern);
            return match.Success && match.Value == name;
        }
    }
}
