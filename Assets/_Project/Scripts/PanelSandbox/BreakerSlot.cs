using System;
using UnityEngine;

namespace NECInspector.PanelSandbox
{
    public enum BusSide { Left, Right }

    public class BreakerSlot : MonoBehaviour
    {
        [Header("Slot Configuration")]
        [SerializeField] private int _slotIndex;
        [SerializeField] private BusSide _busSide;

        [Header("State")]
        [SerializeField] private bool _isOccupied;
        [SerializeField] private PlacedBreaker _placedBreaker;

        public int SlotIndex => _slotIndex;
        public BusSide BusSide => _busSide;
        public bool IsOccupied => _isOccupied;
        public PlacedBreaker PlacedBreaker => _placedBreaker;

        public event Action<BreakerSlot, PlacedBreaker> OnBreakerPlaced;
        public event Action<BreakerSlot> OnBreakerRemoved;

        /// <summary>
        /// Attempt to place a breaker in this slot.
        /// Returns false if the slot is occupied or incompatible.
        /// </summary>
        public bool TryPlace(PlacedBreaker breaker)
        {
            if (_isOccupied) return false;
            if (breaker == null) return false;
            if (!IsCompatible(breaker.BreakerData)) return false;

            _placedBreaker = breaker;
            _isOccupied = true;
            breaker.CurrentSlot = this;

            // Snap breaker to slot position
            breaker.transform.position = transform.position;
            breaker.transform.rotation = transform.rotation;

            OnBreakerPlaced?.Invoke(this, breaker);
            return true;
        }

        /// <summary>
        /// Remove the current breaker from this slot.
        /// </summary>
        public PlacedBreaker Remove()
        {
            if (!_isOccupied) return null;

            var breaker = _placedBreaker;
            breaker.CurrentSlot = null;
            _placedBreaker = null;
            _isOccupied = false;

            OnBreakerRemoved?.Invoke(this);
            return breaker;
        }

        /// <summary>
        /// Check if a breaker type can fit in this slot.
        /// Double-pole breakers need the adjacent slot to be free.
        /// </summary>
        public bool IsCompatible(BreakerData data)
        {
            if (data == null) return false;

            // Single pole breakers fit in any empty slot
            if (data.poleCount == 1) return true;

            // Double pole breakers need the adjacent slot (next index on same side)
            // This check is simplified — scene setup must ensure adjacent slots exist
            if (data.poleCount == 2)
            {
                var adjacentSlot = GetAdjacentSlot();
                return adjacentSlot != null && !adjacentSlot.IsOccupied;
            }

            return false;
        }

        /// <summary>
        /// Gets the next slot on the same bus side (for double-pole breakers).
        /// Returns null if no adjacent slot exists.
        /// </summary>
        public BreakerSlot GetAdjacentSlot()
        {
            var manager = PanelDesignManager.Instance;
            if (manager == null) return null;
            return manager.GetSlot(_slotIndex + 1, _busSide);
        }

        public void SetHighlighted(bool highlighted)
        {
            var glow = GetComponent<Utils.GlowEffect>();
            if (glow != null)
            {
                if (highlighted) glow.StartGlow();
                else glow.StopGlow();
            }
        }
    }
}
