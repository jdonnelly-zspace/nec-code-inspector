// Minimal stand-ins for uGUI and TextMeshPro, only to let the compiler check the rest of each file
// against the real UnityEngine / UnityEditor libraries. Shapes follow the real packages.
using UnityEngine;
using UnityEngine.Events;

namespace UnityEngine.UI
{
    public class Graphic : MonoBehaviour
    {
        public Color color;
        public RectTransform rectTransform => (RectTransform)transform;
    }
    public class MaskableGraphic : Graphic { }
    public class Image : MaskableGraphic { public Sprite sprite; }
    public class Selectable : MonoBehaviour { public Graphic targetGraphic; }
    public class Button : Selectable
    {
        public class ButtonClickedEvent : UnityEvent { }
        public ButtonClickedEvent onClick = new ButtonClickedEvent();
    }
    public class Slider : Selectable
    {
        public float minValue, maxValue, value;
        public UnityEvent<float> onValueChanged = new UnityEvent<float>();
    }
    public class LayoutGroup : MonoBehaviour { public RectOffset padding; }
    public class HorizontalOrVerticalLayoutGroup : LayoutGroup
    {
        public float spacing;
        public bool childControlWidth, childControlHeight, childForceExpandWidth, childForceExpandHeight;
    }
    public class VerticalLayoutGroup : HorizontalOrVerticalLayoutGroup { }
    public class LayoutElement : MonoBehaviour { public float preferredHeight; }
    public class CanvasScaler : MonoBehaviour { }
    public class GraphicRaycaster : MonoBehaviour { }
    public static class DefaultControls
    {
        public struct Resources { public Sprite standard, background, knob, checkmark, dropdown, inputField, mask; }
        public static GameObject CreateSlider(Resources resources) => new GameObject("Slider");
    }
}

namespace UnityEngine.EventSystems
{
    public class UIBehaviour : MonoBehaviour { }
    public class EventSystem : UIBehaviour { }
    public class BaseInputModule : UIBehaviour { }
    public class PointerInputModule : BaseInputModule { }
    public class StandaloneInputModule : PointerInputModule { }
}

namespace TMPro
{
    public enum TextAlignmentOptions { TopLeft, Top, TopRight, Left, Center, Right, BottomLeft, Bottom, BottomRight }
    public class TMP_Text : UnityEngine.UI.MaskableGraphic
    {
        public string text;
        public float fontSize;
        public TextAlignmentOptions alignment;
    }
    public class TextMeshProUGUI : TMP_Text { }
    public class TMP_InputField : UnityEngine.UI.Selectable
    {
        public string text;
        public UnityEngine.Events.UnityEvent<string> onValueChanged = new UnityEngine.Events.UnityEvent<string>();
    }
    public static class TMP_DefaultControls
    {
        public struct Resources { public Sprite standard, background, inputField, knob, checkmark, dropdown, mask; }
        public static GameObject CreateInputField(Resources resources) => new GameObject("InputField");
    }
}

// Project classes that depend on packages not installed here (their real files are removed in the .csproj),
// reduced to the members the panel sandbox scripts use.
namespace Utils
{
    public class GlowEffect : MonoBehaviour
    {
        public void StartGlow() { }
        public void StopGlow() { }
    }
}

namespace NECInspector.PanelSandbox
{
    public class PanelDesignManager : MonoBehaviour
    {
        public static PanelDesignManager Instance { get; private set; }
        public BreakerSlot GetSlot(int index, BusSide side) => null;
    }
}
