
#if UNITY_EDITOR
using UnityEditor;

#endif
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Localization;
using UnityEngine.Rendering;

[RequireComponent(typeof(Collider))]
public class HoverTooltip : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public float scale = 0.1f; // Scale of the tooltip text
    [SerializeField] private Vector3 textOffset;
    [SerializeField] private LocalizedString localizedString;
    [SerializeField] private string key;

    public bool locked = false;
    private ObjectHoverTextManager textManager { get { return ObjectHoverTextManager.Instance; } }

    private void Start()
    {
        if (key == null || key.Length == 0) key = System.Guid.NewGuid().ToString();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!locked) HideText();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!locked) ShowText();
    }

    public void ShowText()
    {
        var worldPos = transform.TransformPoint(textOffset);
        textManager.ShowText(localizedString, worldPos, key, scale);
    }

    public void HideText()
    {
        textManager.ClearText(key);
    }

    private void OnDisable()
    {
        textManager.ClearText(key);
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Handles.color = new Color(0.025f, 0.15f, 0.25f);
        Gizmos.color = new Color(0.05f, 0.29f, 0.5f, 0.9f);
        var worldPosition = transform.TransformPoint(textOffset);
        // Force to draw on top
        Handles.zTest = CompareFunction.Always;

        Handles.DrawWireCube(worldPosition, Vector3.one * scale);
        Gizmos.DrawCube(worldPosition, Vector3.one * scale);

        // Reset zTest to default (optional but recommended)
        Handles.zTest = CompareFunction.LessEqual;
    }
#endif
}