using UnityEngine;

namespace LoveMachine.Core
{
    internal static class GUIUtil
    {
        internal static void PercentBar(string label, string tooltip, float value)
        {
            GUILayout.BeginHorizontal();
            {
                LabelWithTooltip(label, tooltip);
                float lower = 0f;
                float upper = value;
                Core.RangeSlider.Create(ref lower, ref upper, 0f, 1f);
                PercentLabel(upper);
            }
            GUILayout.EndHorizontal();
            SingleSpace();
        }

        internal static void RangeSlider(string label, string tooltip,
            ref float lower, ref float upper, float lowerDefault, float upperDefault,
            float min, float max)
        {
            GUILayout.BeginHorizontal();
            {
                LabelWithTooltip(label, tooltip);
                GUILayout.Label(lower.ToString("N2"), GUILayout.ExpandWidth(false));
                Core.RangeSlider.Create(ref lower, ref upper, min, max);
                GUILayout.Label(upper.ToString("N2"), GUILayout.ExpandWidth(false));
                if (ResetButton)
                {
                    lower = lowerDefault;
                    upper = upperDefault;
                }
            }
            GUILayout.EndHorizontal();
            SingleSpace();
        }

        internal static void PercentRangeSlider(string label, string tooltip,
            ref float lower, ref float upper, float lowerDefault, float upperDefault)
        {
            GUILayout.BeginHorizontal();
            {
                LabelWithTooltip(label, tooltip);
                PercentLabel(lower);
                Core.RangeSlider.Create(ref lower, ref upper, 0f, 1f);
                PercentLabel(upper);
                if (ResetButton)
                {
                    lower = lowerDefault;
                    upper = upperDefault;
                }
            }
            GUILayout.EndHorizontal();
            SingleSpace();
        }

        public static int IntSlider(string label, string tooltip,
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
            SingleSpace();
            return value;
        }

        public static bool Toggle(string label, string tooltip, bool value, bool defaultValue)
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
            SingleSpace();
            return value;
        }

        public static int MultiChoice(string label, string tooltip, string[] choices, int value)
        {
            GUILayout.BeginHorizontal();
            {
                LabelWithTooltip(label, tooltip);
                value = GUILayout.SelectionGrid(
                    selected: value,
                    texts: choices,
                    xCount: 4);
            }
            GUILayout.EndHorizontal();
            SingleSpace();
            return value;
        }

        public static void SingleSpace() => GUILayout.Space(10);

        private static bool ResetButton => GUILayout.Button("Reset", GUILayout.ExpandWidth(false));

        public static void LabelWithTooltip(string label, string tooltip)
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
            string text = (int)(value * 100f) + "%";
            GUILayout.Label(text, GUILayout.Width(labelWidth));
        }
    }
}