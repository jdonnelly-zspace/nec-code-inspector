using UnityEngine;

namespace NECInspector.Tools
{
    /// <summary>
    /// Default inspection tool. Illuminates dark areas to read wire markings and labels.
    /// Uses a spotlight that follows the stylus aim direction.
    /// </summary>
    public class Flashlight : VirtualTool
    {
        [Header("Flashlight Settings")]
        [SerializeField] private Light _spotlight;
        [SerializeField] private float _range = 5f;
        [SerializeField] private float _spotAngle = 30f;
        [SerializeField] private float _intensity = 2f;
        [SerializeField] private Color _lightColor = Color.white;

        private void Awake()
        {
            toolName = "Flashlight";
            description = "Illuminate components to read labels and wire markings.";

            if (_spotlight == null)
            {
                var lightObj = new GameObject("FlashlightSpot");
                lightObj.transform.SetParent(transform);
                lightObj.transform.localPosition = Vector3.zero;
                lightObj.transform.localRotation = Quaternion.identity;
                _spotlight = lightObj.AddComponent<Light>();
                _spotlight.type = LightType.Spot;
            }

            _spotlight.range = _range;
            _spotlight.spotAngle = _spotAngle;
            _spotlight.intensity = _intensity;
            _spotlight.color = _lightColor;
            _spotlight.enabled = false;
        }

        public override void Activate()
        {
            base.Activate();
            if (_spotlight != null) _spotlight.enabled = true;
        }

        public override void Deactivate()
        {
            base.Deactivate();
            if (_spotlight != null) _spotlight.enabled = false;
        }

        public override void OnPointAt(RaycastHit hit)
        {
            // Spotlight follows aim direction - handled by parent transform
        }
    }
}
