using UnityEngine;
using NECInspector.Skills;

namespace NECInspector.Data
{
    public enum CertificateType
    {
        SkillAttainment,
        ScenarioMastery,
        SandboxProficiency,
        OverallProficiency
    }

    [CreateAssetMenu(fileName = "CertificateTemplate", menuName = "NEC Inspector/Certificate Template")]
    public class CertificateTemplateSO : ScriptableObject
    {
        [Header("Certificate Info")]
        public string certificateId;
        public string certificateTitle;
        [TextArea(2, 4)]
        public string descriptionTemplate;   // Use {StudentName}, {Date}, {Score}, {Difficulty}
        public CertificateType type;

        [Header("Requirements")]
        [Tooltip("Minimum combined accuracy (0-1) to earn this certificate")]
        public float minimumAccuracy = 0.8f;
        [Tooltip("Skills (concept IDs) that must be attained (empty = any)")]
        public string[] requiredSkills;
        [Tooltip("Tier at which each required skill must be attained (a higher tier also counts)")]
        public SkillTier requiredTier = SkillTier.Practitioner;
        [Tooltip("Scenario IDs that must be completed (empty = any)")]
        public string[] requiredScenarios;
        [Tooltip("Must complete sandbox mode")]
        public bool requiresSandbox;

        [Header("Visual")]
        public Sprite backgroundImage;
        public Sprite sealImage;
        public Color accentColor = new Color(0.1f, 0.3f, 0.6f);
    }
}
