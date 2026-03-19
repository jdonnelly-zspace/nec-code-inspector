using System;
using UnityEngine;
using zSpace.Core.Utility;


public class PopupCanvas<T> : ZSingleton<T> where T : MonoBehaviour
{
    [SerializeField] protected Canvas canvas;
    [SerializeField] protected LayerMask cullingMask = ~0; // Default to all layers
    protected int _originalCullingMask = 0;
    private bool maskWasSet = false;

    protected void ShowCanvas(bool setParent = true, bool setCullingMask = true)
    {

        // Set the camera culling mask to the specified value
        if (Camera.main != null)
        {
            if (setCullingMask)
            {
                // Store the original culling mask
                _originalCullingMask = Camera.main.cullingMask;
                Camera.main.cullingMask = cullingMask;
                maskWasSet = true;
            }
            canvas.worldCamera = Camera.main;
            if (setParent)
                canvas.transform.SetParent(Camera.main.transform.parent, false);
        }

        // Show the canvas
        if (canvas != null)
        {
            canvas.gameObject.SetActive(true);
        }
    }

    protected void HideCanvas(bool resetCamera = true)
    {
        // reset the camera to the original settings
        if (resetCamera && Camera.main != null && maskWasSet)
        {
            Camera.main.cullingMask = _originalCullingMask;
            maskWasSet = false;
        }
        // Hide the canvas
        if (canvas != null)
        {
            canvas.gameObject.SetActive(false);
        }
    }
}