using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEditor;
using UnityEditor.Events;
using UnityEditor.SceneManagement;
using TMPro;
using NECInspector.Data;
using NECInspector.UI;

namespace NECInspector.Editor
{
    /// <summary>
    /// Adds the installation code picker to the open main menu scene: builds the panel under the
    /// menu's canvas, assigns every field on <see cref="CodeProfilePickerPanel"/> and
    /// <see cref="MainMenuPanel"/>, and adds a button that opens the picker.
    /// Safe to run again: it does nothing if the picker is already wired.
    /// </summary>
    public static class CodePickerSceneSetup
    {
        private const string ITEM_PREFAB_PATH = "Assets/_Project/Prefabs/UI/CodeListItem.prefab";
        private const string CATALOG_PATH = "Assets/_Project/ScriptableObjects/ScenarioCatalog.asset";

        [MenuItem("NEC Inspector/Scene Setup/Add Code Picker To Main Menu")]
        public static void AddToMainMenu()
        {
            var menu = Object.FindAnyObjectByType<MainMenuPanel>(FindObjectsInactive.Include);
            if (menu == null)
            {
                Debug.LogError("[NEC Inspector] Open the main menu scene first: no MainMenuPanel was found in it.");
                return;
            }

            var menuSO = new SerializedObject(menu);
            if (menuSO.FindProperty("_codePickerPanel").objectReferenceValue != null)
            {
                Debug.Log("[NEC Inspector] The code picker is already wired to the main menu.");
                return;
            }

            var canvas = menu.GetComponentInParent<Canvas>(true);
            if (canvas == null)
            {
                Debug.LogError("[NEC Inspector] The MainMenuPanel is not under a Canvas. Put it under the world-space menu canvas first.");
                return;
            }

            var itemPrefab = GetOrCreateItemPrefab();
            var catalog = AssetDatabase.LoadAssetAtPath<ScenarioCatalogSO>(CATALOG_PATH);
            if (catalog == null)
                Debug.LogWarning("[NEC Inspector] No scenario catalog found; run NEC Inspector > Generate Scenario Catalog, then assign it on the picker. Scenario counts will be blank until then.");

            var picker = BuildPanel(canvas.transform, itemPrefab, catalog);
            AssignMenuFields(menuSO, picker);
            AddOpenButton(menu, canvas.transform);

            EditorSceneManager.MarkSceneDirty(menu.gameObject.scene);
            Selection.activeGameObject = picker.gameObject;
            Debug.Log("[NEC Inspector] Code picker added. Check the layout in the Scene view, then save the scene.");
        }

        // ------------------------------------------------------------------
        // Picker panel
        // ------------------------------------------------------------------
        private static CodeProfilePickerPanel BuildPanel(Transform parent, GameObject itemPrefab, ScenarioCatalogSO catalog)
        {
            var root = NewUI("CodeProfilePickerPanel", parent);
            Anchor(root, new Vector2(0.15f, 0.1f), new Vector2(0.85f, 0.9f));
            root.gameObject.AddComponent<Image>().color = new Color(0.08f, 0.1f, 0.14f, 0.96f);

            var title = NewText("Title", root, "Installation Code", 44, TextAlignmentOptions.Center);
            Anchor(title.rectTransform, new Vector2(0f, 0.9f), new Vector2(1f, 1f));

            var current = NewText("CurrentCodeText", root, "", 30, TextAlignmentOptions.Center);
            Anchor(current.rectTransform, new Vector2(0f, 0.82f), new Vector2(1f, 0.9f));

            var list = NewUI("ListContent", root);
            Anchor(list, new Vector2(0.03f, 0.3f), new Vector2(0.55f, 0.8f));
            var layout = list.gameObject.AddComponent<VerticalLayoutGroup>();
            layout.spacing = 12;
            layout.childControlWidth = true;
            layout.childControlHeight = true;
            layout.childForceExpandWidth = true;
            layout.childForceExpandHeight = false;

            var detail = NewText("DetailText", root, "", 28, TextAlignmentOptions.TopLeft);
            Anchor(detail.rectTransform, new Vector2(0.58f, 0.3f), new Vector2(0.97f, 0.8f));

            var closeText = NewText("Label", null, "Close", 32, TextAlignmentOptions.Center);
            var close = NewButton("CloseButton", root, closeText);
            Anchor(close.GetComponent<RectTransform>(), new Vector2(0.35f, 0.05f), new Vector2(0.65f, 0.18f));

            var picker = root.gameObject.AddComponent<CodeProfilePickerPanel>();
            var so = new SerializedObject(picker);
            so.FindProperty("_listContent").objectReferenceValue = list;
            so.FindProperty("_listItemPrefab").objectReferenceValue = itemPrefab;
            so.FindProperty("_currentCodeText").objectReferenceValue = current;
            so.FindProperty("_detailText").objectReferenceValue = detail;
            so.FindProperty("_scenarioCatalog").objectReferenceValue = catalog;
            so.FindProperty("_closeButton").objectReferenceValue = close;
            so.ApplyModifiedPropertiesWithoutUndo();

            root.gameObject.SetActive(false);   // MainMenuPanel.ShowCodePicker() shows it
            Undo.RegisterCreatedObjectUndo(root.gameObject, "Create code picker");
            return picker;
        }

        private static void AssignMenuFields(SerializedObject menuSO, CodeProfilePickerPanel picker)
        {
            menuSO.FindProperty("_codePickerPanel").objectReferenceValue = picker;
            menuSO.ApplyModifiedProperties();
        }

        // ------------------------------------------------------------------
        // Menu button
        // ------------------------------------------------------------------
        private static void AddOpenButton(MainMenuPanel menu, Transform canvas)
        {
            var modeSO = new SerializedObject(menu);
            var modePanel = modeSO.FindProperty("_modeSelectionPanel").objectReferenceValue as GameObject;
            var parent = modePanel != null ? modePanel.transform : canvas;

            var label = NewText("Label", null, "Installation Code", 32, TextAlignmentOptions.Center);
            var button = NewButton("CodePickerButton", parent, label);

            // Sit under the existing buttons when the layout allows; otherwise the author moves it
            if (parent.GetComponent<LayoutGroup>() != null)
            {
                button.gameObject.AddComponent<LayoutElement>().preferredHeight = 100;
            }
            else
            {
                var rt = button.GetComponent<RectTransform>();
                rt.anchorMin = rt.anchorMax = new Vector2(0.5f, 0.1f);
                rt.sizeDelta = new Vector2(420, 90);
                rt.anchoredPosition = Vector2.zero;
            }

            UnityEventTools.AddPersistentListener(button.onClick, new UnityAction(menu.ShowCodePicker));
            Undo.RegisterCreatedObjectUndo(button.gameObject, "Create code picker button");
        }

        // ------------------------------------------------------------------
        // List item prefab (same shape as the scenario list item: a Button with a TMP label)
        // ------------------------------------------------------------------
        internal static GameObject GetOrCreateItemPrefab()
        {
            var existing = AssetDatabase.LoadAssetAtPath<GameObject>(ITEM_PREFAB_PATH);
            if (existing != null) return existing;

            Directory.CreateDirectory(Path.GetDirectoryName(ITEM_PREFAB_PATH));

            var label = NewText("Label", null, "Code", 30, TextAlignmentOptions.Left);
            var button = NewButton("CodeListItem", null, label);
            Anchor(label.rectTransform, Vector2.zero, Vector2.one);
            label.rectTransform.offsetMin = new Vector2(20, 0);

            var element = button.gameObject.AddComponent<LayoutElement>();
            element.preferredHeight = 80;

            var prefab = PrefabUtility.SaveAsPrefabAsset(button.gameObject, ITEM_PREFAB_PATH);
            Object.DestroyImmediate(button.gameObject);
            return prefab;
        }

        // ------------------------------------------------------------------
        // UI building helpers
        // ------------------------------------------------------------------
        internal static RectTransform NewUI(string name, Transform parent)
        {
            var go = new GameObject(name, typeof(RectTransform));
            if (parent != null) go.transform.SetParent(parent, false);
            return go.GetComponent<RectTransform>();
        }

        internal static TextMeshProUGUI NewText(string name, Transform parent, string text, float size, TextAlignmentOptions align)
        {
            var rt = NewUI(name, parent);
            var tmp = rt.gameObject.AddComponent<TextMeshProUGUI>();
            tmp.text = text;
            tmp.fontSize = size;
            tmp.alignment = align;
            return tmp;
        }

        internal static Button NewButton(string name, Transform parent, TextMeshProUGUI label)
        {
            var rt = NewUI(name, parent);
            var image = rt.gameObject.AddComponent<Image>();
            image.color = new Color(0.2f, 0.3f, 0.45f, 1f);
            var button = rt.gameObject.AddComponent<Button>();
            button.targetGraphic = image;

            label.transform.SetParent(rt, false);
            Anchor(label.rectTransform, Vector2.zero, Vector2.one);
            return button;
        }

        internal static void Anchor(RectTransform rt, Vector2 min, Vector2 max)
        {
            rt.anchorMin = min;
            rt.anchorMax = max;
            rt.offsetMin = Vector2.zero;
            rt.offsetMax = Vector2.zero;
        }
    }
}
