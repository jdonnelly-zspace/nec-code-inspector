using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RendererHighlight : MonoBehaviour
{
    public Color color;
    public bool IsHighlighted => renderers != null;

    private Renderer[] renderers;
    private Color[] originalColors;


    public void ApplyHighlight()
    {
        renderers = GetComponentsInChildren<Renderer>().Concat(GetComponents<Renderer>()).ToArray();
        if (renderers != null)
        {
            originalColors = new Color[renderers.Length];
            for (int i = 0; i < renderers.Length; i++)
            {
                Renderer renderer = renderers[i];
                originalColors[i] = renderer.material.color;
            }
        }
    }

    private void RemoveHighlight()
    {
        if (renderers == null || originalColors == null) return;
        for (int i = 0; i < renderers.Length; i++)
            renderers[i].material.color = originalColors[i];
        renderers = null;
    }

    private void OnDisable()
    {
        RemoveHighlight();
    }

    private void OnDestroy()
    {
        RemoveHighlight();
    }


}
