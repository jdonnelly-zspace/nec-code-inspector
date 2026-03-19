using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Used to represent a copy of a draggable object that should be dropped in a specific spot.
/// </summary>
public class DropTarget : MonoBehaviour
{
    private Renderer _renderer;

    [SerializeField] private bool applyCustomAlpha = true;
    [Range(0f, 1.0f)]
    [SerializeField] private float alpha = 0.3f;

    void Awake()
    {
        _renderer = GetComponent<Renderer>();
        if (applyCustomAlpha) ApplyAlpha();
    }

    void OnEnable()
    {
        if (_renderer != null)
            _renderer.enabled = true;
    }

    void OnDisable()
    {
        if (_renderer != null)
            _renderer.enabled = false;
    }

    private void ApplyAlpha()
    {
        if (applyCustomAlpha && _renderer != null)
        {
            var material = _renderer.material;
            // Create a new instance so the asset isn't affected
            _renderer.material = new Material(material);
            _renderer.material.color = new Color(material.color.r, material.color.g, material.color.b, alpha);
        }
    }
}
