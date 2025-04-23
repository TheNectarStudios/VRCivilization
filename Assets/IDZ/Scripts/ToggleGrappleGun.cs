using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class ToggleGrappleGun : MonoBehaviour
{
    public GameObject grappleGun;
    public InputActionProperty rightGripAction;

    [Tooltip("Name of the runtime-spawned right hand object")]
    public string rightHandObjectName = "RightHand Model";

    private bool isGunActive = false;
    private bool isButtonHeld = false;

    private MeshRenderer[] objectsToHide;

    void Start()
    {
        // Start delayed search for the right hand mesh
        StartCoroutine(FindHandMeshDelayed());
    }

    void Update()
    {
        float gripValue = rightGripAction.action.ReadValue<float>();

        if (gripValue > 0.8f && !isButtonHeld)
        {
            isButtonHeld = true;
            ToggleGun();
        }
        else if (gripValue < 0.2f)
        {
            isButtonHeld = false;
        }
    }

    void ToggleGun()
    {
        isGunActive = !isGunActive;
        grappleGun.SetActive(isGunActive);

        if (objectsToHide == null || objectsToHide.Length == 0) return;

        foreach (MeshRenderer mr in objectsToHide)
        {
            if (mr != null)
                mr.enabled = !isGunActive;
        }
    }

    IEnumerator FindHandMeshDelayed()
    {
        yield return new WaitForSeconds(0.5f); // Give XR rig time to instantiate the hand

        GameObject rightHandObj = GameObject.Find(rightHandObjectName);

        if (rightHandObj != null)
        {
            objectsToHide = rightHandObj.GetComponentsInChildren<MeshRenderer>();
            Debug.Log("[GrappleGun] Found and cached right hand mesh renderers.");
        }
        else
        {
            Debug.LogWarning("[GrappleGun] Couldn't find right hand object! Double-check the name: " + rightHandObjectName);
        }
    }
}
