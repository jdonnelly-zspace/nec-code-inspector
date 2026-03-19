using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Components;
using zSpace.Core;
using zSpace.Core.Extensions;

public class ObjectHoverText : MonoBehaviour
{
    [SerializeField] protected SpriteRenderer hoverTextSpriteRenderer;
    [SerializeField] protected TextMeshPro hoverText;
    [SerializeField] private RectTransform hoverTextRectTransform;
    [SerializeField] private LocalizeStringEvent localizeStringEvent;
    private static readonly float scaleMultiplier = .015f;

    public void SetOffset(Vector3 offset, bool local = true)
    {
        if (local)
            transform.localPosition = offset;
        else
            transform.position = transform.position + offset;
        AlignToCamera();
        ScaleToCamera();
    }

    public void SetText(string text)
    {
        if (hoverText == null) return;
        hoverText.text = text;
        ResizeToFit();
    }

    public void SetText(LocalizedString stringReference)
    {
        if (localizeStringEvent == null) return;
        localizeStringEvent.StringReference = stringReference;
        localizeStringEvent.RefreshString();

        localizeStringEvent.StringReference.GetLocalizedString((System.Action<string>)(localizedText =>
        {
            if (hoverText != null)
            {
                hoverText.text = localizedText;
            }
        }));
        ResizeToFit();
    }

    public void ResizeToFit()
    {
        if (hoverText == null) return;



        // Get the preferred size of the text
        var preferredSize = hoverText.GetPreferredValues();

        // Resize the sprite renderer to fit the text
        hoverTextSpriteRenderer.size = preferredSize;

        if (hoverTextRectTransform != null)
        {
            // Resize the RectTransform to fit the text
            hoverTextRectTransform.sizeDelta = new Vector2(preferredSize.x, preferredSize.y);
        }
    }

    public void AlignToCamera()
    {
        // face the main camera if it exists
        if (Camera.main != null)
        {
            transform.rotation = Camera.main.transform.parent.transform.rotation;
        }
    }

    public void ScaleToCamera()
    {
        if (Camera.main == null) throw new System.Exception("No main camera found");

        float distance = Vector3.Distance(transform.position, Camera.main.transform.position);

        // Calculate proper perspective scaling to maintain consistent apparent size
        float perspectiveFactor = distance * Mathf.Tan(Camera.main.fieldOfView * 0.5f * Mathf.Deg2Rad);
        float size = perspectiveFactor * scaleMultiplier;

        // You can't set the lossy scale so this is a workaround
        Transform parent = transform.parent;
        transform.SetParent(null, true);
        transform.SetUniformScale(size);
        transform.SetParent(parent, true);
    }
}
