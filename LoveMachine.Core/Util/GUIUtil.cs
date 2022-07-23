using BepInEx.Configuration;
using UnityEngine;

namespace LoveMachine.Core
{
    internal static class GUIUtil
    {
        internal static void DrawRangeSlider(ConfigEntry<int> min, ConfigEntry<int> max)
        {
            float labelWidth = GUI.skin.label.CalcSize(new GUIContent("100%")).x;
            GUILayout.BeginHorizontal();
            {
                float lower = min.Value;
                float upper = max.Value;
                GUILayout.Label(lower + "%", GUILayout.Width(labelWidth));
                RangeSlider.Create(ref lower, ref upper, 0, 100);
                GUILayout.Label(upper + "%", GUILayout.Width(labelWidth));
                if (GUILayout.Button("Reset", GUILayout.ExpandWidth(false)))
                {
                    lower = (int)min.DefaultValue;
                    upper = (int)max.DefaultValue;
                }
                min.Value = (int)lower;
                max.Value = (int)upper;
            }
            GUILayout.EndHorizontal();
        }
    }
}
