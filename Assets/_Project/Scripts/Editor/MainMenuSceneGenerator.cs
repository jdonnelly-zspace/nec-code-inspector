using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEditor;
using UnityEditor.Events;
using UnityEditor.SceneManagement;
using TMPro;
using NECInspector.Data;
using NECInspector.UI;

namespace NECInspector.Editor
{
    /// <summary>
    /// Builds and saves the MainMenu scene: a world-space canvas with the mode, scenario,
    /// difficulty, settings and installation code panels, all wired to <see cref="MainMenuPanel"/>.
    /// Re-running replaces the scene file, so do hand edits in the scene only after you stop using this.
    /// </summary>
    public static class MainMenuSceneGenerator
    {
        private const string SCENE_PATH = "Assets/_Project/Scenes/MainMenu/MainMenu.unity";
        private const string CATALOG_PATH = "Assets/_Project/ScriptableObjects/ScenarioCatalog.asset";

        private static readonly Color PANEL = new Color(0.08f, 0.1f, 0.14f, 0.96f);

        [MenuItem("NEC Inspector/Scene Setup/Generate Main Menu Scene")]
        public static void Generate()
        {
            if (File.Exists(SCENE_PATH) &&
                !EditorUtility.DisplayDialog("Replace main menu scene?",
                    "A MainMenu scene already exists. Generating again replaces it and loses any edits made to it.",
                    "Replace", "Cancel"))
                return;

            if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) return;

            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            Directory.CreateDirectory(Path.GetDirectoryName(SCENE_PATH));

            // Project convention: the camera is a ZCamera rig added by hand, never a plain Camera
            new GameObject("ZCameraRig");
            BuildLight();
            BuildEventSystem();

            var canvasGO = BuildCanvas();
            BuildMenu(canvasGO.transform);

            // The picker and its menu button are added by the same tool used on existing scenes
            CodePickerSceneSetup.AddToMainMenu();

            EditorSceneManager.SaveScene(scene, SCENE_PATH);
            Debug.Log($"[NEC Inspector] Main menu scene saved to {SCENE_PATH}. Add a ZCamera rig, check the layout, and add the scene to Build Settings.");
        }

        // ------------------------------------------------------------------
        // Scene basics
        // ------------------------------------------------------------------
        private static void BuildLight()
        {
            var go = new GameObject("DirectionalLight");
            var light = go.AddComponent<Light>();
            light.type = LightType.Directional;
            light.color = new Color(1f, 0.96f, 0.9f);
            go.transform.rotation = Quaternion.Euler(50f, -30f, 0f);
        }

        private static void BuildEventSystem()
        {
            var es = new GameObject("EventSystem");
            es.AddComponent<EventSystem>();
            es.AddComponent<StandaloneInputModule>();
        }

        private static GameObject BuildCanvas()
        {
            var go = new GameObject("MenuCanvas");
            var canvas = go.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.WorldSpace;   // zSpace stereo needs world space

            var rt = go.GetComponent<RectTransform>();
            rt.sizeDelta = new Vector2(1920, 1080);
            rt.localScale = Vector3.one * 0.001f;
            rt.position = new Vector3(0f, 1.5f, 2f);

            go.AddComponent<CanvasScaler>();
            go.AddComponent<GraphicRaycaster>();
            return go;
        }

        // ------------------------------------------------------------------
        // Menu
        // ------------------------------------------------------------------
        private static void BuildMenu(Transform canvas)
        {
            var root = CodePickerSceneSetup.NewUI("MainMenu", canvas);
            CodePickerSceneSetup.Anchor(root, Vector2.zero, Vector2.one);
            var menu = root.gameObject.AddComponent<MainMenuPanel>();

            var title = Label("Title", root, "Code Inspector", 64, TextAlignmentOptions.Center, 0.88f, 1f);
            var subtitle = Label("Subtitle", root, "Build the skills behind your electrician credential", 30, TextAlignmentOptions.Center, 0.82f, 0.88f);
            var activeCode = Label("ActiveCodeText", root, "", 26, TextAlignmentOptions.Center, 0.77f, 0.82f);

            // --- Mode selection ---
            var mode = Panel("ModeSelectionPanel", root, 0.3f, 0.05f, 0.7f, 0.75f);
            var modeLayout = mode.gameObject.AddComponent<VerticalLayoutGroup>();
            ConfigureStack(modeLayout, 16, 30);
            var itemPrefab = CodePickerSceneSetup.GetOrCreateItemPrefab();
            Row(mode, "Inspection Scenarios", menu.ShowScenarioSelection);
            Row(mode, "Panel Sandbox", menu.ShowSandboxMode);
            Row(mode, "Settings", menu.ShowSettings);

            // --- Scenario selection (list left, detail right) ---
            var scenarios = Panel("ScenarioSelectionPanel", root, 0.05f, 0.05f, 0.45f, 0.75f);
            var listContent = CodePickerSceneSetup.NewUI("ScenarioListContent", scenarios);
            CodePickerSceneSetup.Anchor(listContent, new Vector2(0.05f, 0.18f), new Vector2(0.95f, 0.95f));
            ConfigureStack(listContent.gameObject.AddComponent<VerticalLayoutGroup>(), 12, 0);
            var noScenarios = Label("NoScenariosText", scenarios, "", 26, TextAlignmentOptions.TopLeft, 0.62f, 0.95f);
            noScenarios.color = new Color(1f, 0.85f, 0.5f);
            BackButton(scenarios, menu);

            var detail = Panel("ScenarioDetailPanel", root, 0.5f, 0.05f, 0.95f, 0.75f);
            var detailTitle = Label("ScenarioTitle", detail, "", 40, TextAlignmentOptions.TopLeft, 0.85f, 0.97f);
            var detailDesc = Label("ScenarioDescription", detail, "", 28, TextAlignmentOptions.TopLeft, 0.5f, 0.85f);
            var detailDiffs = Label("ScenarioDifficulties", detail, "", 26, TextAlignmentOptions.TopLeft, 0.4f, 0.5f);
            var detailBest = Label("ScenarioBestScore", detail, "", 26, TextAlignmentOptions.TopLeft, 0.3f, 0.4f);
            Place(detail, "Difficulty", menu.ShowDifficultySelection, 0.05f, 0.08f, 0.48f, 0.22f);
            Place(detail, "Start", menu.LaunchSelectedScenario, 0.52f, 0.08f, 0.95f, 0.22f);

            // --- Difficulty ---
            var difficulty = Panel("DifficultyPanel", root, 0.25f, 0.1f, 0.75f, 0.7f);
            difficulty.SetAsLastSibling();
            var diffText = Label("DifficultyDescription", difficulty, "", 26, TextAlignmentOptions.TopLeft, 0.5f, 0.95f);
            Place(difficulty, "Beginner", menu.SetDifficultyBeginner, 0.03f, 0.3f, 0.33f, 0.45f);
            Place(difficulty, "Standard", menu.SetDifficultyStandard, 0.35f, 0.3f, 0.65f, 0.45f);
            Place(difficulty, "Expert", menu.SetDifficultyExpert, 0.67f, 0.3f, 0.97f, 0.45f);
            Place(difficulty, "Done", menu.ShowScenarioSelection, 0.35f, 0.05f, 0.65f, 0.2f);

            // --- Settings ---
            var settings = Panel("SettingsPanel", root, 0.25f, 0.05f, 0.75f, 0.75f);
            Label("SettingsTitle", settings, "Settings", 40, TextAlignmentOptions.Center, 0.9f, 1f);
            var master = SliderRow(settings, "Master Volume", 0.74f);
            var sfx = SliderRow(settings, "Effects Volume", 0.6f);
            var ambient = SliderRow(settings, "Ambient Volume", 0.46f);
            var nameInput = NameInput(settings, 0.28f);
            var back = Place(settings, "Back", menu.ShowModeSelection, 0.35f, 0.04f, 0.65f, 0.16f);
            UnityEventTools.AddPersistentListener(back.onClick, new UnityAction(menu.SaveStudentName));

            // The menu starts on mode selection and hides the rest itself
            var so = new SerializedObject(menu);
            Set(so, "_titleText", title);
            Set(so, "_subtitleText", subtitle);
            Set(so, "_activeCodeText", activeCode);
            Set(so, "_modeSelectionPanel", mode.gameObject);
            Set(so, "_scenarioSelectionPanel", scenarios.gameObject);
            Set(so, "_scenarioListContent", listContent);
            Set(so, "_scenarioListItemPrefab", itemPrefab);
            Set(so, "_scenarioCatalog", AssetDatabase.LoadAssetAtPath<ScenarioCatalogSO>(CATALOG_PATH));
            Set(so, "_noScenariosText", noScenarios);
            Set(so, "_scenarioDetailPanel", detail.gameObject);
            Set(so, "_scenarioTitle", detailTitle);
            Set(so, "_scenarioDescription", detailDesc);
            Set(so, "_scenarioDifficulties", detailDiffs);
            Set(so, "_scenarioBestScore", detailBest);
            Set(so, "_difficultyPanel", difficulty.gameObject);
            Set(so, "_difficultyDescription", diffText);
            Set(so, "_settingsPanel", settings.gameObject);
            Set(so, "_masterVolumeSlider", master);
            Set(so, "_sfxVolumeSlider", sfx);
            Set(so, "_ambientVolumeSlider", ambient);
            Set(so, "_studentNameInput", nameInput);
            so.ApplyModifiedPropertiesWithoutUndo();

            if (AssetDatabase.LoadAssetAtPath<ScenarioCatalogSO>(CATALOG_PATH) == null)
                Debug.LogWarning("[NEC Inspector] No scenario catalog yet: run NEC Inspector > Generate Scenario Catalog, then assign it on the MainMenu object.");

            // Panels other than mode selection start hidden (Start() would hide them anyway)
            scenarios.gameObject.SetActive(false);
            detail.gameObject.SetActive(false);
            difficulty.gameObject.SetActive(false);
            settings.gameObject.SetActive(false);
        }

        // ------------------------------------------------------------------
        // Builders
        // ------------------------------------------------------------------
        private static void Set(SerializedObject so, string field, Object value)
        {
            var prop = so.FindProperty(field);
            if (prop == null) { Debug.LogError($"[NEC Inspector] MainMenuPanel has no field {field}."); return; }
            prop.objectReferenceValue = value;
        }

        private static void ConfigureStack(VerticalLayoutGroup layout, float spacing, float padding)
        {
            layout.spacing = spacing;
            layout.padding = new RectOffset((int)padding, (int)padding, (int)padding, (int)padding);
            layout.childControlWidth = true;
            layout.childControlHeight = true;
            layout.childForceExpandWidth = true;
            layout.childForceExpandHeight = false;
        }

        private static RectTransform Panel(string name, Transform parent, float x0, float y0, float x1, float y1)
        {
            var rt = CodePickerSceneSetup.NewUI(name, parent);
            CodePickerSceneSetup.Anchor(rt, new Vector2(x0, y0), new Vector2(x1, y1));
            rt.gameObject.AddComponent<Image>().color = PANEL;
            return rt;
        }

        private static TextMeshProUGUI Label(string name, Transform parent, string text, float size,
            TextAlignmentOptions align, float y0, float y1)
        {
            var tmp = CodePickerSceneSetup.NewText(name, parent, text, size, align);
            CodePickerSceneSetup.Anchor(tmp.rectTransform, new Vector2(0.04f, y0), new Vector2(0.96f, y1));
            return tmp;
        }

        // A button laid out by its parent's layout group, with a fixed height
        private static Button Row(Transform parent, string text, UnityAction onClick)
        {
            var label = CodePickerSceneSetup.NewText("Label", null, text, 34, TextAlignmentOptions.Center);
            var button = CodePickerSceneSetup.NewButton(text.Replace(" ", "") + "Button", parent, label);
            button.gameObject.AddComponent<LayoutElement>().preferredHeight = 100;
            UnityEventTools.AddPersistentListener(button.onClick, onClick);
            return button;
        }

        private static Button Place(Transform parent, string text, UnityAction onClick,
            float x0, float y0, float x1, float y1)
        {
            var label = CodePickerSceneSetup.NewText("Label", null, text, 32, TextAlignmentOptions.Center);
            var button = CodePickerSceneSetup.NewButton(text.Replace(" ", "") + "Button", parent, label);
            CodePickerSceneSetup.Anchor(button.GetComponent<RectTransform>(), new Vector2(x0, y0), new Vector2(x1, y1));
            UnityEventTools.AddPersistentListener(button.onClick, onClick);
            return button;
        }

        private static void BackButton(Transform parent, MainMenuPanel menu)
        {
            Place(parent, "Back", menu.ShowModeSelection, 0.3f, 0.04f, 0.7f, 0.14f);
        }

        private static Slider SliderRow(Transform parent, string text, float y)
        {
            Label(text.Replace(" ", "") + "Label", parent, text, 28, TextAlignmentOptions.Left, y, y + 0.09f)
                .rectTransform.anchorMax = new Vector2(0.4f, y + 0.09f);

            var go = DefaultControls.CreateSlider(UIResources());
            go.name = text.Replace(" ", "") + "Slider";
            go.transform.SetParent(parent, false);
            var rt = go.GetComponent<RectTransform>();
            rt.anchorMin = new Vector2(0.42f, y + 0.02f);
            rt.anchorMax = new Vector2(0.96f, y + 0.07f);
            rt.offsetMin = rt.offsetMax = Vector2.zero;

            var slider = go.GetComponent<Slider>();
            slider.minValue = 0f;
            slider.maxValue = 1f;
            slider.value = 1f;
            return slider;
        }

        private static TMP_InputField NameInput(Transform parent, float y)
        {
            Label("StudentNameLabel", parent, "Name", 28, TextAlignmentOptions.Left, y, y + 0.09f)
                .rectTransform.anchorMax = new Vector2(0.4f, y + 0.09f);

            var res = new TMP_DefaultControls.Resources
            {
                inputField = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/InputFieldBackground.psd"),
                standard = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/UISprite.psd")
            };
            var go = TMP_DefaultControls.CreateInputField(res);
            go.name = "StudentNameInput";
            go.transform.SetParent(parent, false);
            var rt = go.GetComponent<RectTransform>();
            rt.anchorMin = new Vector2(0.42f, y);
            rt.anchorMax = new Vector2(0.96f, y + 0.09f);
            rt.offsetMin = rt.offsetMax = Vector2.zero;
            return go.GetComponent<TMP_InputField>();
        }

        private static DefaultControls.Resources UIResources() => new DefaultControls.Resources
        {
            standard = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/UISprite.psd"),
            background = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/Background.psd"),
            knob = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/Knob.psd")
        };
    }
}
