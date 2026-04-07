using UnityEngine;

namespace NECInspector.PanelSandbox
{
    [RequireComponent(typeof(LineRenderer))]
    public class WireConnection : MonoBehaviour
    {
        [Header("Wire Properties")]
        [SerializeField] private string _wireGauge = "12 AWG";

        [Header("Endpoints")]
        [SerializeField] private Transform _startPoint;
        [SerializeField] private Transform _endPoint;

        [Header("Visual")]
        [SerializeField] private Color _defaultColor = Color.black;
        [SerializeField] private Color _errorColor = Color.red;
        [SerializeField] private float _wireWidth = 0.005f;

        private LineRenderer _lineRenderer;

        public string WireGauge => _wireGauge;
        public Transform StartPoint => _startPoint;
        public Transform EndPoint => _endPoint;
        public PlacedBreaker ConnectedBreaker { get; set; }
        public bool IsConnected => ConnectedBreaker != null && _startPoint != null && _endPoint != null;

        private void Awake()
        {
            _lineRenderer = GetComponent<LineRenderer>();
            _lineRenderer.positionCount = 2;
            _lineRenderer.startWidth = _wireWidth;
            _lineRenderer.endWidth = _wireWidth;
            _lineRenderer.startColor = _defaultColor;
            _lineRenderer.endColor = _defaultColor;
        }

        private void Update()
        {
            if (_startPoint != null && _endPoint != null)
            {
                UpdateVisual();
            }
        }

        public void SetGauge(string gauge)
        {
            _wireGauge = gauge;
            // Thicker wires for larger gauges
            _wireWidth = gauge switch
            {
                "14 AWG" => 0.003f,
                "12 AWG" => 0.005f,
                "10 AWG" => 0.007f,
                "8 AWG" => 0.009f,
                "6 AWG" => 0.012f,
                _ => 0.005f
            };
            _lineRenderer.startWidth = _wireWidth;
            _lineRenderer.endWidth = _wireWidth;
        }

        public void UpdateVisual()
        {
            _lineRenderer.SetPosition(0, _startPoint.position);
            _lineRenderer.SetPosition(1, _endPoint.position);
        }

        public void SetError(bool isError)
        {
            var color = isError ? _errorColor : _defaultColor;
            _lineRenderer.startColor = color;
            _lineRenderer.endColor = color;
        }

        /// <summary>
        /// Validates that wire gauge is appropriate for the connected breaker's amperage.
        /// </summary>
        public bool Validate()
        {
            if (ConnectedBreaker?.BreakerData == null) return false;

            int maxAmps = GetMaxAmpsForGauge(_wireGauge);
            return ConnectedBreaker.BreakerData.ampRating <= maxAmps;
        }

        /// <summary>
        /// Returns the maximum ampacity for a given copper wire gauge per NEC Table 310.16.
        /// </summary>
        public static int GetMaxAmpsForGauge(string gauge)
        {
            return gauge switch
            {
                "14 AWG" => 15,
                "12 AWG" => 20,
                "10 AWG" => 30,
                "8 AWG" => 40,
                "6 AWG" => 55,
                "4 AWG" => 70,
                "3 AWG" => 85,
                "2 AWG" => 95,
                "1 AWG" => 110,
                "1/0 AWG" => 125,
                "2/0 AWG" => 145,
                "3/0 AWG" => 165,
                "4/0 AWG" => 195,
                _ => 0
            };
        }
    }
}
