using UnityEngine;
using UnityEngine.EventSystems;
using UnityEditor;
using NECInspector.Core;
using NECInspector.Inspection;
using NECInspector.PanelSandbox;

namespace NECInspector.Editor
{
    public static class SceneSetupHelper
    {
        // ====================================================================
        // Create Inspection Scene Hierarchy
        // ====================================================================
        [MenuItem("NEC Inspector/Scene Setup/Create Inspection Scene Hierarchy")]
        public static void CreateInspectionScene()
        {
            // --- Cameras ---
            CreateSeparator("---Cameras---");
            var cameraRig = CreateGameObject("ZCameraRig");
            Undo.RegisterCreatedObjectUndo(cameraRig, "Create ZCameraRig");

            // --- UI ---
            CreateSeparator("---UI---");
            var canvas = CreateWorldSpaceCanvas("WorldCanvas");

            CreateChildGameObject(canvas, "InspectionHUD");
            CreateChildGameObject(canvas, "ViolationFlaggingPanel");
            CreateChildGameObject(canvas, "NECReferencePanel");
            CreateChildGameObject(canvas, "InspectionReviewPanel");

            // EventSystem
            if (Object.FindAnyObjectByType<EventSystem>() == null)
            {
                var es = new GameObject("EventSystem");
                es.AddComponent<EventSystem>();
                es.AddComponent<StandaloneInputModule>();
                Undo.RegisterCreatedObjectUndo(es, "Create EventSystem");
            }

            // --- Scenario ---
            CreateSeparator("---Scenario---");
            var scenarioRoot = CreateGameObject("ScenarioRoot");
            scenarioRoot.AddComponent<InspectionManager>();
            scenarioRoot.AddComponent<InspectionScenarioRunner>();
            Undo.RegisterCreatedObjectUndo(scenarioRoot, "Create ScenarioRoot");

            // --- Environment ---
            CreateSeparator("---Environment---");
            var envRoot = CreateGameObject("EnvironmentRoot");
            Undo.RegisterCreatedObjectUndo(envRoot, "Create EnvironmentRoot");

            // --- Lighting ---
            CreateSeparator("---Lighting---");
            var light = CreateGameObject("DirectionalLight");
            var lightComp = light.AddComponent<Light>();
            lightComp.type = LightType.Directional;
            lightComp.color = new Color(1f, 0.96f, 0.9f);
            lightComp.intensity = 1f;
            light.transform.rotation = Quaternion.Euler(50f, -30f, 0f);
            Undo.RegisterCreatedObjectUndo(light, "Create DirectionalLight");

            Debug.Log("[NEC Inspector] Inspection scene hierarchy created. Wire up component references in the Inspector.");
        }

        // ====================================================================
        // Create Sandbox Scene Hierarchy
        // ====================================================================
        [MenuItem("NEC Inspector/Scene Setup/Create Sandbox Scene Hierarchy")]
        public static void CreateSandboxScene()
        {
            // --- Cameras ---
            CreateSeparator("---Cameras---");
            var cameraRig = CreateGameObject("ZCameraRig");
            Undo.RegisterCreatedObjectUndo(cameraRig, "Create ZCameraRig");

            // --- UI ---
            CreateSeparator("---UI---");
            var canvas = CreateWorldSpaceCanvas("WorldCanvas");

            CreateChildGameObject(canvas, "PanelDesignHUD");
            CreateChildGameObject(canvas, "ComplianceResultsPanel");

            // EventSystem
            if (Object.FindAnyObjectByType<EventSystem>() == null)
            {
                var es = new GameObject("EventSystem");
                es.AddComponent<EventSystem>();
                es.AddComponent<StandaloneInputModule>();
                Undo.RegisterCreatedObjectUndo(es, "Create EventSystem");
            }

            // --- Sandbox ---
            CreateSeparator("---Sandbox---");
            var sandboxRoot = CreateGameObject("SandboxRoot");
            sandboxRoot.AddComponent<PanelDesignManager>();
            sandboxRoot.AddComponent<PanelDesignRunner>();
            Undo.RegisterCreatedObjectUndo(sandboxRoot, "Create SandboxRoot");

            // --- Panel ---
            CreateSeparator("---Panel---");
            var panelRoot = CreateGameObject("PanelRoot");
            CreateChildGameObject(panelRoot, "PanelBody");
            CreateChildGameObject(panelRoot, "BreakerSlots");
            CreateChildGameObject(panelRoot, "BreakerTray");
            Undo.RegisterCreatedObjectUndo(panelRoot, "Create PanelRoot");

            // --- Lighting ---
            CreateSeparator("---Lighting---");
            var light = CreateGameObject("DirectionalLight");
            var lightComp = light.AddComponent<Light>();
            lightComp.type = LightType.Directional;
            lightComp.color = new Color(1f, 0.96f, 0.9f);
            lightComp.intensity = 1f;
            light.transform.rotation = Quaternion.Euler(50f, -30f, 0f);
            Undo.RegisterCreatedObjectUndo(light, "Create DirectionalLight");

            Debug.Log("[NEC Inspector] Sandbox scene hierarchy created. Wire up component references in the Inspector.");
        }

        // ====================================================================
        // Validate Current Scene
        // ====================================================================
        [MenuItem("NEC Inspector/Scene Setup/Validate Current Scene")]
        public static void ValidateScene()
        {
            int issues = 0;

            // Check EventSystem
            var eventSystems = Object.FindObjectsByType<EventSystem>(FindObjectsSortMode.None);
            if (eventSystems.Length == 0)
            {
                Debug.LogWarning("[Validate] MISSING: No EventSystem found in scene.");
                issues++;
            }
            else if (eventSystems.Length > 1)
            {
                Debug.LogWarning($"[Validate] WARNING: {eventSystems.Length} EventSystems found. Should be exactly 1.");
                issues++;
            }
            else
            {
                Debug.Log("[Validate] OK: EventSystem present.");
            }

            // Check Canvas render mode
            var canvases = Object.FindObjectsByType<Canvas>(FindObjectsSortMode.None);
            foreach (var canvas in canvases)
            {
                if (canvas.renderMode != RenderMode.WorldSpace)
                {
                    Debug.LogError($"[Validate] FAIL: Canvas '{canvas.name}' uses {canvas.renderMode}. Must be WorldSpace for zSpace.");
                    issues++;
                }
                else
                {
                    Debug.Log($"[Validate] OK: Canvas '{canvas.name}' is WorldSpace.");
                }
            }

            // Check InspectionManager references
            var inspMgr = Object.FindAnyObjectByType<InspectionManager>();
            if (inspMgr != null)
            {
                Debug.Log("[Validate] OK: InspectionManager found.");
                // Check SerializedObject for null references
                var so = new SerializedObject(inspMgr);
                var scenarioProp = so.FindProperty("_scenarioDefinition");
                if (scenarioProp != null && scenarioProp.objectReferenceValue == null)
                {
                    Debug.LogWarning("[Validate] WARNING: InspectionManager._scenarioDefinition is not assigned.");
                    issues++;
                }
            }

            // Check PanelDesignManager
            var panelMgr = Object.FindAnyObjectByType<PanelDesignManager>();
            if (panelMgr != null)
            {
                Debug.Log("[Validate] OK: PanelDesignManager found (sandbox scene).");
            }

            // Check for regular Camera (should use ZCamera instead)
            var cameras = Object.FindObjectsByType<Camera>(FindObjectsSortMode.None);
            foreach (var cam in cameras)
            {
                if (cam.gameObject.name != "ZCameraRig" && !cam.gameObject.name.Contains("Preview"))
                {
                    Debug.LogWarning($"[Validate] WARNING: Regular Camera found on '{cam.gameObject.name}'. zSpace scenes should use ZCamera via ZCameraRig.");
                    issues++;
                }
            }

            // Summary
            if (issues == 0)
                Debug.Log("[Validate] Scene validation passed with no issues.");
            else
                Debug.LogWarning($"[Validate] Scene validation found {issues} issue(s). See warnings above.");
        }

        // ====================================================================
        // Helper Methods
        // ====================================================================
        private static GameObject CreateGameObject(string name)
        {
            var go = new GameObject(name);
            return go;
        }

        private static GameObject CreateChildGameObject(GameObject parent, string name)
        {
            var go = new GameObject(name);
            go.transform.SetParent(parent.transform, false);
            return go;
        }

        private static void CreateSeparator(string name)
        {
            var sep = new GameObject(name);
            sep.tag = "EditorOnly";
            Undo.RegisterCreatedObjectUndo(sep, $"Create {name}");
        }

        private static GameObject CreateWorldSpaceCanvas(string name)
        {
            var canvasGO = new GameObject(name);
            var canvas = canvasGO.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.WorldSpace;

            var rt = canvasGO.GetComponent<RectTransform>();
            rt.sizeDelta = new Vector2(1920, 1080);
            rt.localScale = Vector3.one * 0.001f; // 1mm per pixel — typical for World Space

            canvasGO.AddComponent<UnityEngine.UI.CanvasScaler>();
            canvasGO.AddComponent<UnityEngine.UI.GraphicRaycaster>();
            Undo.RegisterCreatedObjectUndo(canvasGO, "Create WorldCanvas");

            return canvasGO;
        }
    }
}
