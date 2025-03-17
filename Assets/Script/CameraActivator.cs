using UnityEngine;
using UnityEngine.InputSystem;

public class CameraActivatorVR : MonoBehaviour
{
    [Header("Camera Activation Settings")]
    public GameObject cameraObject; // Camera GameObject to activate/deactivate
    public Transform rightHandRay;  // Reference to the right-hand VR controller
    public InputActionProperty grabAction; // Grab action input

    private bool isActive = false;

    void Update()
    {
        if (grabAction.action.WasPressedThisFrame())
        {
            ActivateCamera();
        }

        if (grabAction.action.WasReleasedThisFrame())
        {
            DeactivateCamera();
        }
    }

    void ActivateCamera()
    {
        if (!isActive)
        {
            cameraObject.SetActive(true);
            isActive = true;
        }
    }

    void DeactivateCamera()
    {
        if (isActive)
        {
            cameraObject.SetActive(false);
            isActive = false;
        }
    }
}
