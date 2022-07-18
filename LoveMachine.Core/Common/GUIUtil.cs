using UnityEngine;

namespace LoveMachine.Core
{
    internal static class GUIUtil
    {
        internal static void RangeSlider(string label, string tooltip,
            ref float lower, ref float upper, float lowerDefault, float upperDefault)
        {
            GUILayout.BeginHorizontal();
            {
                LabelWithTooltip(label, tooltip);
                PercentLabel(lower);
                UnityEngine.RangeSlider.Create(ref lower, ref upper, 0f, 1f);
                PercentLabel(upper);
                if (ResetButton)
                {
                    lower = lowerDefault;
                    upper = upperDefault;
                }
            }
            GUILayout.EndHorizontal();
        }

        internal static int IntSlider(string label, string tooltip,
            int value, int defaultValue, int min, int max)
        {
            GUILayout.BeginHorizontal();
            {
                LabelWithTooltip(label, tooltip);
                value = (int)GUILayout.HorizontalSlider(value, min, max);
                value = int.Parse(GUILayout.TextField(value.ToString(), GUILayout.Width(50)));
                if (ResetButton)
                {
                    value = defaultValue;
                }
                value = Mathf.Clamp(value, min, max);
            }
            GUILayout.EndHorizontal();
            return value;
        }

        internal static float PercentSlider(string label, string tooltip,
            float value, float defaultValue)
        {
            GUILayout.BeginHorizontal();
            {
                LabelWithTooltip(label, tooltip);
                value = GUILayout.HorizontalSlider(value, 0f, 1f);
                PercentLabel(value);
                if (ResetButton)
                {
                    value = defaultValue;
                }
                value = Mathf.Clamp(value, 0f, 1f);
            }
            GUILayout.EndHorizontal();
            return value;
        }

        internal static bool Toggle(string label, string tooltip, bool value, bool defaultValue)
        {
            GUILayout.BeginHorizontal();
            {
                LabelWithTooltip(label, tooltip);
                value = GUILayout.Toggle(value, value ? "On" : "Off");
                if (ResetButton)
                {
                    value = defaultValue;
                }
            }
            GUILayout.EndHorizontal();
            return value;
        }

        internal static void SmallSpace() => GUILayout.Space(10);

        private static bool ResetButton => GUILayout.Button("Reset", GUILayout.ExpandWidth(false));

        private static void LabelWithTooltip(string label, string tooltip)
        {
            float labelWidth = GUI.skin.label.CalcSize(new GUIContent("Some pretty long text")).x;
            var content = new GUIContent
            {
                text = label,
                tooltip = tooltip
            };
            GUILayout.Label(content, GUILayout.Width(labelWidth));
        }

        private static void PercentLabel(float value)
        {
            float labelWidth = GUI.skin.label.CalcSize(new GUIContent("100%")).x;
            string text = (int)(value * 100) + "%";
            GUILayout.Label(text, GUILayout.Width(labelWidth));
        }
    }
}
