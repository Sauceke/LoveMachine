using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;

namespace LoveMachinePrototyper
{
    public static class FindUtil
    {
        public static T[] FindAll<T>(string pattern)
            where T : Component
        {
            return PrototyperConfig.UseRegexes.Value
                ? GameObject.FindObjectsOfType<T>()
                    .Where(go => MatchesEndOfPath(go.transform, pattern))
                    .ToArray()
                : new[] { GameObject.Find(pattern)?.GetComponent<T>() }
                    .Where(go => go != null)
                    .ToArray();
        }

        public static T FindFirst<T>(string pattern)
            where T : Component
        {
            return FindAll<T>(pattern).FirstOrDefault();
        }

        public static bool MatchesEndOfPath(Transform transform, string pattern)
        {
            Match match;
            if (pattern.Contains("/"))
            {
                string path = "";
                while (transform != null)
                {
                    path = "/" + transform.name + path;
                    transform = transform.parent;
                }
                match = Regex.Match(path, pattern);
                return match.Success
                    && match.Index + match.Length == path.Length
                    && (pattern.StartsWith("/") ? match.Index == 0 : path[match.Index - 1] == '/');
            }
            match = Regex.Match(transform.name, pattern);
            return match.Success && match.Value == transform.name;
        }

        public static bool Matches(string name, string pattern)
        {
            if (!PrototyperConfig.UseRegexes.Value)
            {
                return name == pattern;
            }
            var match = Regex.Match(name, pattern);
            return match.Success && match.Value == name;
        }
    }
}
