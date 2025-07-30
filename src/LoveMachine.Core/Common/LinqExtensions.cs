using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LoveMachine.Core.Common
{
    public static class LinqExtensions
    {
        public static T Maximize<T>(this IEnumerable<T> items, Func<T, float> keySelector)
        {
            float max = float.NegativeInfinity;
            T argMax = default;
            foreach (var item in items)
            {
                float value = keySelector(item);
                if (max <= value)
                {
                    argMax = item;
                    max = value;
                }
            }
            return argMax;
        }

        public static T Minimize<T>(this IEnumerable<T> items, Func<T, float> keySelector) =>
            Maximize(items, item => -keySelector(item));
    }
}
