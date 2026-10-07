using UnityEngine;
using UnityEditor;
using NECInspector.Data;
using NECInspector.Skills;

namespace NECInspector.Editor
{
    public static class CertificateTemplateGenerator
    {
        [MenuItem("NEC Inspector/Generate Certificate Templates")]
        public static void Generate()
        {
            const string CERT_DIR = "Assets/_Project/ScriptableObjects/Certificates";

            if (!AssetDatabase.IsValidFolder(CERT_DIR))
            {
                if (!AssetDatabase.IsValidFolder("Assets/_Project/ScriptableObjects"))
                    AssetDatabase.CreateFolder("Assets/_Project", "ScriptableObjects");
                AssetDatabase.CreateFolder("Assets/_Project/ScriptableObjects", "Certificates");
            }

            AssetDatabase.StartAssetEditing();
            try
            {
                // Skill Attainment (any skills)
                CreateOrUpdate($"{CERT_DIR}/CertTemplate_ChapterCompletion.asset",
                    certificateId: "CERT-CHAPTER-001",
                    certificateTitle: "Skill Proficiency",
                    descriptionTemplate: "This certifies that {StudentName} has demonstrated proficiency in electrical code skills on {Date}, achieving {Score} accuracy at {Difficulty} difficulty.",
                    type: CertificateType.SkillAttainment,
                    minimumAccuracy: 0.7f,
                    requiredSkills: new string[0],
                    requiredScenarios: new string[0],
                    requiresSandbox: false,
                    accentColor: new Color(0.2f, 0.4f, 0.8f, 1f) // Blue
                );

                // Scenario Mastery
                CreateOrUpdate($"{CERT_DIR}/CertTemplate_ScenarioMastery.asset",
                    certificateId: "CERT-SCENARIO-001",
                    certificateTitle: "Scenario Mastery Certificate",
                    descriptionTemplate: "This certifies that {StudentName} has achieved mastery-level performance in electrical inspection on {Date}, with {Score} accuracy at {Difficulty} difficulty.",
                    type: CertificateType.ScenarioMastery,
                    minimumAccuracy: 0.85f,
                    requiredSkills: new string[0],
                    requiredScenarios: new string[0],
                    requiresSandbox: false,
                    accentColor: new Color(0.85f, 0.65f, 0.13f, 1f) // Gold
                );

                // Sandbox Proficiency
                CreateOrUpdate($"{CERT_DIR}/CertTemplate_SandboxProficiency.asset",
                    certificateId: "CERT-SANDBOX-001",
                    certificateTitle: "Panel Design Proficiency",
                    descriptionTemplate: "This certifies that {StudentName} has demonstrated proficiency in code-compliant panel design on {Date}, with {Score} compliance rate at {Difficulty} difficulty.",
                    type: CertificateType.SandboxProficiency,
                    minimumAccuracy: 0.8f,
                    requiredSkills: new string[0],
                    requiredScenarios: new string[0],
                    requiresSandbox: true,
                    accentColor: new Color(0.2f, 0.7f, 0.3f, 1f) // Green
                );

                // Overall Proficiency
                CreateOrUpdate($"{CERT_DIR}/CertTemplate_OverallProficiency.asset",
                    certificateId: "CERT-OVERALL-001",
                    certificateTitle: "NEC Code Inspector - Overall Proficiency",
                    descriptionTemplate: "This certifies that {StudentName} has achieved overall proficiency across all NEC Code Inspector modules on {Date}, demonstrating comprehensive electrical code knowledge with {Score} average accuracy at {Difficulty} difficulty.",
                    type: CertificateType.OverallProficiency,
                    minimumAccuracy: 0.8f,
                    requiredSkills: ConceptIds.All,
                    requiredScenarios: new string[0],
                    requiresSandbox: true,
                    accentColor: new Color(0.1f, 0.15f, 0.4f, 1f) // Dark blue
                );
            }
            finally
            {
                AssetDatabase.StopAssetEditing();
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Debug.Log("[NEC Inspector] Certificate templates generated: 4 templates (Skill, Scenario, Sandbox, Overall).");
        }

        private static void CreateOrUpdate(string path, string certificateId, string certificateTitle,
            string descriptionTemplate, CertificateType type, float minimumAccuracy,
            string[] requiredSkills, string[] requiredScenarios, bool requiresSandbox, Color accentColor)
        {
            var asset = AssetDatabase.LoadAssetAtPath<CertificateTemplateSO>(path);
            if (asset == null)
            {
                asset = ScriptableObject.CreateInstance<CertificateTemplateSO>();
                AssetDatabase.CreateAsset(asset, path);
            }

            asset.certificateId = certificateId;
            asset.certificateTitle = certificateTitle;
            asset.descriptionTemplate = descriptionTemplate;
            asset.type = type;
            asset.minimumAccuracy = minimumAccuracy;
            asset.requiredSkills = requiredSkills;
            asset.requiredScenarios = requiredScenarios;
            asset.requiresSandbox = requiresSandbox;
            asset.accentColor = accentColor;

            EditorUtility.SetDirty(asset);
        }
    }
}
