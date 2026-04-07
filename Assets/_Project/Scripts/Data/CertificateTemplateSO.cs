using UnityEngine;

namespace NECInspector.Data
{
    public enum CertificateType
    {
        ChapterCompletion,
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
        public string descriptionTemplate;   // Use {StudentName}, {Date}, {Score}, {Chapter}
        public CertificateType type;

        [Header("Requirements")]
        [Tooltip("Minimum combined accuracy (0-1) to earn this certificate")]
        public float minimumAccuracy = 0.8f;
        [Tooltip("NEC chapters that must be mastered (empty = any)")]
        public string[] requiredChapters;
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
