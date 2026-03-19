using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Applies a highlight color to all materials on all renderers on this GameObject and its children.
/// Restores the original materials/colors when disabled.
/// </summary>
public class HighlightObject : MonoBehaviour
{
    public Color highlightColor = Color.yellow;
    public Material[] skipMaterials;
    public float highlightIntensity = 1f;
    public float highlightSpeed = 1f;

    [SerializeField] private List<Renderer> renderers;
    private Dictionary<Renderer, Material[]> startMaterials = new Dictionary<Renderer, Material[]>();
    private Material glowMaterial;
    private static int colorID = Shader.PropertyToID("_Color");
    private static int pulseSpeedID = Shader.PropertyToID("_PulseSpeed");
    private static int minAlphaID = Shader.PropertyToID("_MinAlpha");
    private static int maxAlphaID = Shader.PropertyToID("_MaxAlpha");

    private void Start()
    {
        glowMaterial = Resources.Load<Material>("Materials/OverlayMat");
    }

    private void InitRendererValues()
    {
        if (renderers == null || renderers.Count == 0 || startMaterials == null || startMaterials.Count == 0)
        {
            renderers = (new Renderer[] { GetComponent<Renderer>() })
                .Concat(GetComponentsInChildren<Renderer>(includeInactive: true))
                .Where(r => r != null)
                .Distinct()
                .ToList();
            startMaterials.Clear();
            foreach (var renderer in renderers)
            {
                var mats = renderer.materials;
                startMaterials[renderer] = mats.ToArray();
            }
        }
    }

    public void StartGlow()
    {
        InitRendererValues();
        if (glowMaterial == null)
            glowMaterial = Resources.Load<Material>("Materials/OverlayMat");

        foreach (var renderer in renderers)
        {
            // Skip if this renderer is already an overlay
            if (renderer.gameObject.name == "OverlayObject") continue;

            // Create overlay object as a child of the root highlight object
            GameObject overlayObject = new GameObject("OverlayObject");
            overlayObject.transform.SetParent(renderer.transform, false);

            if (renderer is MeshRenderer meshRenderer && renderer.GetComponent<MeshFilter>() is MeshFilter meshFilter)
            {
                // MeshRenderer + MeshFilter (static mesh)
                MeshFilter overlayMeshFilter = overlayObject.AddComponent<MeshFilter>();
                overlayMeshFilter.sharedMesh = meshFilter.sharedMesh;

                MeshRenderer overlayRenderer = overlayObject.AddComponent<MeshRenderer>();
                overlayRenderer.materials = CreateGlowMaterials(meshRenderer.materials);
            }
            else if (renderer is SkinnedMeshRenderer skinnedRenderer)
            {
                // SkinnedMeshRenderer (deformable mesh)
                SkinnedMeshRenderer overlaySkinned = overlayObject.AddComponent<SkinnedMeshRenderer>();
                overlaySkinned.sharedMesh = skinnedRenderer.sharedMesh;
                overlaySkinned.bones = skinnedRenderer.bones;
                overlaySkinned.rootBone = skinnedRenderer.rootBone;
                overlaySkinned.bounds = new Bounds(overlaySkinned.bounds.center, Vector3.one * 2f);
                overlaySkinned.materials = CreateGlowMaterials(skinnedRenderer.materials);
                overlaySkinned.updateWhenOffscreen = true; // Optional: prevents culling issues
            }
        }
    }

    public void StopGlow()
    {
        // Find the overlay child object and destroy it
        Transform overlayObject = transform.Find("OverlayObject");
        if (overlayObject != null)
        {
            Destroy(overlayObject.gameObject);
        }
    }

    private bool MaterialsAreEquivalent(Material a, Material b)
    {
        if (a == null || b == null) return false;
        string aName = a.name.Replace(" (Instance)", "").Replace("(Instance)", "").Trim();
        string bName = b.name.Replace(" (Instance)", "").Replace("(Instance)", "").Trim();
        return a.shader == b.shader && aName == bName;
    }

    // Helper method to create glow materials
    private Material[] CreateGlowMaterials(Material[] sourceMaterials)
    {
        Material[] newMaterials = new Material[sourceMaterials.Length];
        for (int i = 0; i < sourceMaterials.Length; i++)
        {
            if (skipMaterials != null && skipMaterials.Any(mat => MaterialsAreEquivalent(mat, sourceMaterials[i])))
            {
                newMaterials[i] = sourceMaterials[i];
            }
            else
            {
                newMaterials[i] = new Material(glowMaterial);
                newMaterials[i].SetColor(colorID, highlightColor);
                newMaterials[i].SetFloat(pulseSpeedID, highlightSpeed);
                newMaterials[i].SetFloat(minAlphaID, 0.1f);
                newMaterials[i].SetFloat(maxAlphaID, 0.5f);
            }
        }
        return newMaterials;
    }

}
