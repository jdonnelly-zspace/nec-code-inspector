using UnityEngine;
using UnityEditor;
using NECInspector.Data;
using NECInspector.Skills;

namespace NECInspector.Editor
{
    /// <summary>
    /// Creates or updates the certificate template assets from
    /// Assets/_Project/Content/Certificates/certificates.json.
    /// </summary>
    public static class CertificateTemplateGenerator
    {
        private const string CONTENT_PATH = "Assets/_Project/Content/Certificates/certificates.json";
        private const string CERT_DIR = "Assets/_Project/ScriptableObjects/Certificates";

        [MenuItem("NEC Inspector/Generate Certificate Templates")]
        public static void Generate()
        {
            var file = ContentFileLoader.Load<CertificateFile>(CONTENT_PATH, ContentFileValidator.Validate);
            if (file == null) return;

            ContentFileLoader.EnsureFolder("Assets/_Project", "ScriptableObjects");
            ContentFileLoader.EnsureFolder("Assets/_Project/ScriptableObjects", "Certificates");

            AssetDatabase.StartAssetEditing();
            try
            {
                foreach (var entry in file.templates)
                    CreateOrUpdate($"{CERT_DIR}/CertTemplate_{entry.assetName}.asset", entry);
            }
            finally
            {
                AssetDatabase.StopAssetEditing();
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Debug.Log($"[NEC Inspector] Certificate templates generated: {file.templates.Length} templates.");
        }

        private static void CreateOrUpdate(string path, CertificateEntry entry)
        {
            var asset = AssetDatabase.LoadAssetAtPath<CertificateTemplateSO>(path);
            if (asset == null)
            {
                asset = ScriptableObject.CreateInstance<CertificateTemplateSO>();
                AssetDatabase.CreateAsset(asset, path);
            }

            asset.certificateId = entry.certificateId;
            asset.certificateTitle = entry.certificateTitle;
            asset.descriptionTemplate = entry.descriptionTemplate;
            asset.type = ContentFileLoader.ParseEnum<CertificateType>(entry.type);
            asset.minimumAccuracy = entry.minimumAccuracy;
            asset.requiredSkills = entry.requiresAllSkills ? ConceptIds.All : (entry.requiredSkills ?? new string[0]);
            if (SkillTiers.TryParse(entry.requiredTier, out var tier))
                asset.requiredTier = tier;
            asset.requiredScenarios = entry.requiredScenarios ?? new string[0];
            asset.requiresSandbox = entry.requiresSandbox;
            asset.accentColor = new Color(entry.accentColor[0], entry.accentColor[1], entry.accentColor[2], entry.accentColor[3]);

            EditorUtility.SetDirty(asset);
        }
    }
}
