using System;
using System.Collections.Generic;
using UnityEngine;

namespace NECInspector.Tools
{
    /// <summary>
    /// Manages the set of available virtual tools. Handles tool switching.
    /// Attach to a world-space UI panel docked at the edge of the display.
    /// </summary>
    public class ToolBelt : MonoBehaviour
    {
        [SerializeField] private List<VirtualTool> _tools = new();
        [SerializeField] private int _defaultToolIndex = 0;

        private VirtualTool _activeTool;
        public VirtualTool ActiveTool => _activeTool;

        public event Action<VirtualTool> OnToolChanged;

        private void Start()
        {
            // Deactivate all tools initially
            foreach (var tool in _tools)
                tool?.Deactivate();

            // Activate default tool
            if (_tools.Count > 0 && _defaultToolIndex < _tools.Count)
                SelectTool(_defaultToolIndex);
        }

        public void SelectTool(int index)
        {
            if (index < 0 || index >= _tools.Count) return;

            _activeTool?.Deactivate();
            _activeTool = _tools[index];
            _activeTool?.Activate();

            OnToolChanged?.Invoke(_activeTool);
        }

        public void SelectTool(VirtualTool tool)
        {
            int index = _tools.IndexOf(tool);
            if (index >= 0) SelectTool(index);
        }

        public List<VirtualTool> GetAvailableTools() => _tools;
    }
}
