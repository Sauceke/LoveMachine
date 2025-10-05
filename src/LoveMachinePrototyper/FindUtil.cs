using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;

namespace LoveMachinePrototyper
{
    public static class FindUtil
    {
        public static Transform[] FindAll(string pattern)
        {
            return PrototyperConfig.UseRegexes.Value
                ? GameObject.FindObjectsOfType<Transform>()
                    .Where(go => MatchesEndOfPath(go, pattern))
                    .ToArray()
                : new[] { GameObject.Find(pattern)?.transform };
        }

        public static Transform FindFirst(string pattern)
        {
            return FindAll(pattern).FirstOrDefault();
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
    }
}
