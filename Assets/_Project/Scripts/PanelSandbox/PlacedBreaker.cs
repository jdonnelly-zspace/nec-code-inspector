using System;
using UnityEngine;

namespace NECInspector.PanelSandbox
{
    public class PlacedBreaker : MonoBehaviour
    {
        [Header("Breaker Info")]
        [SerializeField] private BreakerData _breakerData;
        [SerializeField] private string _assignedCircuitName;

        [Header("Connections")]
        [SerializeField] private WireConnection _connectedWire;

        public BreakerData BreakerData => _breakerData;
        public string AssignedCircuitName => _assignedCircuitName;
        public WireConnection ConnectedWire => _connectedWire;
        public BreakerSlot CurrentSlot { get; set; }
        public bool IsPlaced => CurrentSlot != null;
        public bool IsWired => _connectedWire != null;

        public event Action<PlacedBreaker> OnCircuitAssigned;
        public event Action<PlacedBreaker> OnWireConnected;

        public void Initialize(BreakerData data)
        {
            _breakerData = data;
            gameObject.name = $"Breaker_{data.DisplayName}";
        }

        public void AssignCircuit(string circuitName)
        {
            _assignedCircuitName = circuitName;
            OnCircuitAssigned?.Invoke(this);
        }

        public void ConnectWire(WireConnection wire)
        {
            _connectedWire = wire;
            wire.ConnectedBreaker = this;
            OnWireConnected?.Invoke(this);
        }

        public void DisconnectWire()
        {
            if (_connectedWire != null)
            {
                _connectedWire.ConnectedBreaker = null;
                _connectedWire = null;
            }
        }

        /// <summary>
        /// Returns the load in VA for this breaker's circuit.
        /// For 120V single-pole: amps * 120. For 240V double-pole: amps * 240.
        /// </summary>
        public float GetLoadVA()
        {
            if (_breakerData == null) return 0f;
            float voltage = _breakerData.poleCount == 2 ? 240f : 120f;
            return _breakerData.ampRating * voltage;
        }
    }
}
