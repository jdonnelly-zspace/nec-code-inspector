using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;


public class GlowEffect : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Runtime Glow Settings")]
    public Color color = new Color(0, 1, 1, 0.75f); //default is 75% cyan - "targets" use an 80% yellow
    public float outlineWidth = 2.5f; //"targets" use a 2f width
    public bool pulse = false;
    public bool onlyShowWhenHovered = false;

    private List<Outline> outlineObjects;
    private bool isGlowActive = false;
    private bool isPointerOver = false;

    public void StartGlow()
    {
        if (isGlowActive) return;

        ApplyOrEnableOutlines();
        SetOutlinesEnabled(!onlyShowWhenHovered || isPointerOver);
        isGlowActive = true;
    }

    private void ApplyOrEnableOutlines()
    {
        if (outlineObjects == null)
            outlineObjects = new List<Outline>();
        else
            outlineObjects.Clear();

        Outline outline = gameObject.GetComponent<Outline>();
        if (!outline)
        {
            outline = gameObject.AddComponent<Outline>();
            outline.enabled = false;
            outline.OutlineMode = Outline.Mode.OutlineAll;
        }

        outline.OutlineColor = color;
        outline.OutlineWidth = outlineWidth;
        outline.Pulse = pulse;

        outlineObjects.Add(outline);
    }

    public void StopGlow()
    {
        if (!isGlowActive) return;

        SetOutlinesEnabled(false);
        isGlowActive = false;
    }

    public IEnumerator WaitStopGlow(float durationSeconds)
    {
        yield return new WaitForSeconds(durationSeconds);
        StopGlow();
        Destroy(this);
    }

    private void SetOutlinesEnabled(bool enabled)
    {
        if (outlineObjects == null) return;

        foreach (var outline in outlineObjects)
        {
            if (outline != null)
                outline.enabled = enabled;
        }
    }

    private void SetPulse(bool enablePulse)
    {
        if (outlineObjects == null) return;

        foreach (var outline in outlineObjects)
        {
            if (outline != null)
                outline.Pulse = enablePulse;
        }
    }

    private void DestroyOutlines()
    {
        if (outlineObjects != null)
        {
            foreach (Outline outline in outlineObjects)
            {
                if (outline != null)
                    outline.enabled = false;
            }
            outlineObjects = null;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        isPointerOver = true;

        if (onlyShowWhenHovered && isGlowActive)
        {
            SetOutlinesEnabled(true);
        }

        if (pulse)
        {
            SetPulse(false);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isPointerOver = false;

        if (onlyShowWhenHovered && isGlowActive)
        {
            SetOutlinesEnabled(false);
        }

        if (pulse)
        {
            SetPulse(true);
        }

    }

    private void OnEnable()
    {
        StartGlow();
    }

    private void OnDisable()
    {
        StopGlow();
    }

    private void OnDestroy()
    {
        StopGlow();
    }
}