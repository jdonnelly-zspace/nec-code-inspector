using UnityEngine;

/// <summary>
/// Stub for the Quick Outline third-party asset.
/// Replace with the real Quick Outline package from the Unity Asset Store when available.
/// Provides outline/glow rendering on meshes — used by GlowEffect and InspectableComponent.
/// </summary>
[DisallowMultipleComponent]
public class Outline : MonoBehaviour
{
    public enum Mode
    {
        OutlineAll = 0,
        OutlineVisible = 1,
        OutlineHidden = 2,
        OutlineAndSilhouette = 3,
        SilhouetteOnly = 4
    }

    [SerializeField] private Mode _outlineMode = Mode.OutlineAll;
    [SerializeField] private Color _outlineColor = Color.white;
    [SerializeField] private float _outlineWidth = 2f;
    [SerializeField] private bool _pulse = false;

    private Renderer[] _renderers;
    private MaterialPropertyBlock _propBlock;
    private static readonly int _OutlineColorID = Shader.PropertyToID("_OutlineColor");
    private static readonly int _OutlineWidthID = Shader.PropertyToID("_OutlineWidth");

    public Mode OutlineMode
    {
        get => _outlineMode;
        set => _outlineMode = value;
    }

    public Color OutlineColor
    {
        get => _outlineColor;
        set
        {
            _outlineColor = value;
            ApplyProperties();
        }
    }

    public float OutlineWidth
    {
        get => _outlineWidth;
        set
        {
            _outlineWidth = value;
            ApplyProperties();
        }
    }

    public bool Pulse
    {
        get => _pulse;
        set => _pulse = value;
    }

    private void Awake()
    {
        _renderers = GetComponentsInChildren<Renderer>();
        _propBlock = new MaterialPropertyBlock();
    }

    private void OnEnable()
    {
        ApplyProperties();
    }

    private void OnDisable()
    {
        ClearProperties();
    }

    private void ApplyProperties()
    {
        if (_renderers == null || _propBlock == null) return;

        foreach (var renderer in _renderers)
        {
            if (renderer == null) continue;
            renderer.GetPropertyBlock(_propBlock);
            _propBlock.SetColor(_OutlineColorID, _outlineColor);
            _propBlock.SetFloat(_OutlineWidthID, _outlineWidth);
            renderer.SetPropertyBlock(_propBlock);
        }
    }

    private void ClearProperties()
    {
        if (_renderers == null || _propBlock == null) return;

        foreach (var renderer in _renderers)
        {
            if (renderer == null) continue;
            renderer.SetPropertyBlock(null);
        }
    }
}
