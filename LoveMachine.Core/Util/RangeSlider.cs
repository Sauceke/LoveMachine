using System.Linq;

namespace UnityEngine
{
    public struct RangeSlider
    {
        private static readonly GUIStyle sliderStyle = GUI.skin.horizontalSlider;
        private static readonly GUIStyle thumbStyle = GUI.skin.horizontalSliderThumb;
        private static readonly GUIStyle bandStyle = new GUIStyle
        {
            normal = new GUIStyleState
            {
                background = BandTexture()
            }
        };
        private readonly Rect position;
        private float value1;
        private float value2;
        private readonly float min;
        private readonly float max;
        private readonly int id;

        public RangeSlider(Rect position, float value1, float value2, float min, float max, int id)
        {
            this.position = position;
            this.value1 = value1;
            this.value2 = value2;
            this.min = min;
            this.max = max;
            this.id = id;
        }

        public static void Create(ref float lower, ref float upper, float min, float max, params GUILayoutOption[] options)
        {
            var position = GUILayoutUtility.GetRect(GUIContent.none, sliderStyle, options);
            int id = GUIUtility.GetControlID("Slider".GetHashCode(), FocusType.Passive, position);
            new RangeSlider(position, lower, upper, min, max, id).Handle(out lower, out upper);
        }

        public void Handle(out float lower, out float upper)
        {
            switch (CurrentEventType)
            {
                case EventType.MouseDown:
                    OnMouseDown();
                    break;
                case EventType.MouseDrag:
                    OnMouseDrag();
                    break;
                case EventType.MouseUp:
                    OnMouseUp();
                    break;
                case EventType.Repaint:
                    OnRepaint();
                    break;
            }
            lower = Mathf.Min(value1, value2);
            upper = Mathf.Max(value1, value2);
            return;
        }

        private void OnMouseDown()
        {
            if (!position.Contains(Event.current.mousePosition))
            {
                return;
            }
            GUIUtility.hotControl = id;
            Event.current.Use();
            GUI.changed = true;
            UpdateValues();
        }

        private void OnMouseDrag()
        {
            if (GUIUtility.hotControl != id)
                return;
            Event.current.Use();
            GUI.changed = true;
            UpdateValues();
        }

        private void OnMouseUp()
        {
            if (GUIUtility.hotControl == id)
            {
                Event.current.Use();
                GUIUtility.hotControl = 0;
            }
        }

        private void OnRepaint()
        {
            sliderStyle.Draw(position, GUIContent.none, id);
            bandStyle.Draw(BandRect(), GUIContent.none, id);
            thumbStyle.Draw(ThumbRect(value1), GUIContent.none, id);
            thumbStyle.Draw(ThumbRect(value2), GUIContent.none, id);
        }

        private void UpdateValues()
        {
            float value = AbscissToSliderValue(Event.current.mousePosition.x);
            float distance1 = Mathf.Abs(value - value1);
            float distance2 = Mathf.Abs(value - value2);
            if (distance1 < distance2)
            {
                value1 = value;
            }
            else
            {
                value2 = value;
            }
        }

        private EventType CurrentEventType => Event.current.GetTypeForControl(id);

        private Rect BandRect()
        {
            float startX = SliderValueToAbsciss(Mathf.Min(value1, value2));
            float endX = SliderValueToAbsciss(Mathf.Max(value1, value2));
            return new Rect(
                sliderStyle.padding.left + startX,
                position.y + sliderStyle.padding.top + ThumbHeight / 4,
                endX - startX,
                position.height - sliderStyle.padding.vertical - ThumbHeight / 2);
        }

        private Rect ThumbRect(float value) => new Rect(
            sliderStyle.padding.left + SliderValueToAbsciss(value) - ThumbWidth / 2,
            position.y + sliderStyle.padding.top,
            ThumbWidth,
            position.height - sliderStyle.padding.vertical);

        private float AbscissToSliderValue(float x) =>
            Mathf.Lerp(min, max, Mathf.InverseLerp(position.x, position.x + position.size.x, x));

        private float SliderValueToAbsciss(float value) =>
            Mathf.Lerp(position.x, position.x + position.size.x, Mathf.InverseLerp(min, max, value));

        private float ThumbWidth => thumbStyle.CalcSize(GUIContent.none).x;

        private float ThumbHeight => thumbStyle.CalcSize(GUIContent.none).y;

        private static Texture2D BandTexture()
        {
            var texture = new Texture2D(Screen.width, Screen.height);
            var color = new Color(1, 1, 1, 0.5f);
            var pixels = Enumerable.Repeat(color, Screen.width * Screen.height).ToArray();
            texture.SetPixels(pixels);
            texture.Apply();
            return texture;
        }
    }
}
