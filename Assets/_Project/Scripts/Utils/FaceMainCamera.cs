using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class FaceMainCamera : MonoBehaviour
{
    public Vector3 axisOffset = Vector3.zero;

    // Update is called once per frame
    void Update()
    {
        if (Camera.main != null)
        {
            Vector3 directionToCamera = Camera.main.transform.position - transform.position;
            Quaternion targetRotation = Quaternion.LookRotation(directionToCamera, Vector3.up);
            targetRotation *= Quaternion.Euler(axisOffset);
            transform.rotation = targetRotation;
        }
    }
}
