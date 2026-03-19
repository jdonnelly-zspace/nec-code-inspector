using UnityEngine;
using TMPro;

namespace NECInspector.Tools
{
    /// <summary>
    /// Virtual digital multimeter. Touch probes to components to read simulated values.
    /// Values are defined on InspectableComponent via MeasurementData.
    /// </summary>
    public class Multimeter : VirtualTool
    {
        [Header("Multimeter Settings")]
        [SerializeField] private TMP_Text _displayText;
        [SerializeField] private MeasurementMode _currentMode = MeasurementMode.Voltage;

        public enum MeasurementMode { Voltage, Current, Resistance, Continuity }

        private void Awake()
        {
            toolName = "Digital Multimeter";
            description = "Measure voltage, current, resistance, and continuity.";
        }

        public override void Activate()
        {
            base.Activate();
            UpdateDisplay("---");
        }

        public override void Deactivate()
        {
            base.Deactivate();
        }

        public override void OnUse(RaycastHit hit)
        {
            var measurable = hit.collider.GetComponent<MeasurementPoint>();
            if (measurable == null) return;

            string reading = _currentMode switch
            {
                MeasurementMode.Voltage => $"{measurable.voltage:F1} V",
                MeasurementMode.Current => $"{measurable.current:F2} A",
                MeasurementMode.Resistance => measurable.resistance < 1f ? $"{measurable.resistance * 1000:F0} mΩ" : $"{measurable.resistance:F1} Ω",
                MeasurementMode.Continuity => measurable.hasContinuity ? "BEEP" : "OL",
                _ => "---"
            };

            UpdateDisplay(reading);
            Debug.Log($"[Multimeter] {_currentMode}: {reading} on {hit.collider.name}");
        }

        public void SetMode(MeasurementMode mode)
        {
            _currentMode = mode;
            UpdateDisplay("---");
        }

        private void UpdateDisplay(string text)
        {
            if (_displayText != null) _displayText.text = text;
        }
    }

    /// <summary>
    /// Attach to components that can be measured with the multimeter.
    /// Provides simulated electrical readings.
    /// </summary>
    public class MeasurementPoint : MonoBehaviour
    {
        public float voltage = 120f;
        public float current = 0f;
        public float resistance = 0f;
        public bool hasContinuity = true;
    }
}
