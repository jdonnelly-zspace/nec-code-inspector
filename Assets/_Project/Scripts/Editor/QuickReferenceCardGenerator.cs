using UnityEngine;
using UnityEditor;
using NECInspector.Core;
using NECInspector.Data;

namespace NECInspector.Editor
{
    /// <summary>
    /// Creates or updates the quick reference card assets from the files in
    /// Assets/_Project/Content/QuickReference (one file per installation code).
    /// </summary>
    public static class QuickReferenceCardGenerator
    {
        private const string CONTENT_DIR = "Assets/_Project/Content/QuickReference";
        private const string CARD_DIR = "Assets/_Project/ScriptableObjects/QuickReferenceCards";

        [MenuItem("NEC Inspector/Generate Quick Reference Cards")]
        public static void Generate()
        {
            ContentFileLoader.EnsureFolder("Assets/_Project", "ScriptableObjects");
            ContentFileLoader.EnsureFolder("Assets/_Project/ScriptableObjects", "QuickReferenceCards");

            int count = 0;
            AssetDatabase.StartAssetEditing();
            try
            {
                foreach (string path in System.IO.Directory.GetFiles(CONTENT_DIR, "*.json"))
                {
                    var file = ContentFileLoader.Load<QuickReferenceFile>(path.Replace('\\', '/'), ContentFileValidator.Validate);
                    if (file == null) continue;

                    foreach (var entry in file.cards)
                    {
                        CreateOrUpdate($"{CARD_DIR}/{entry.cardId}.asset", entry);
                        count++;
                    }
                }
            }
            finally
            {
                AssetDatabase.StopAssetEditing();
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Debug.Log($"[NEC Inspector] Generated {count} quick reference cards.");
        }

        private static void CreateOrUpdate(string path, QuickReferenceEntry entry)
        {
            var card = AssetDatabase.LoadAssetAtPath<QuickReferenceCardSO>(path);
            if (card == null)
            {
                card = ScriptableObject.CreateInstance<QuickReferenceCardSO>();
                AssetDatabase.CreateAsset(card, path);
            }

            card.cardId = entry.cardId;
            card.profileId = entry.profileId;
            card.title = entry.title;
            card.category = ContentFileLoader.ParseEnum<CardCategory>(entry.category);
            card.summary = entry.summary;
            card.keyRule = entry.keyRule;
            card.codeReferences = entry.codeReferences;
            card.keywords = entry.keywords;
            card.minimumDifficulty = ContentFileLoader.ParseEnum<DifficultyLevel>(entry.minimumDifficulty);

            EditorUtility.SetDirty(card);
        }
    }
}
