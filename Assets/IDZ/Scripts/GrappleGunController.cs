using UnityEngine;
using UnityEngine.InputSystem;

public class GrappleGunController : MonoBehaviour
{
    public Transform firePoint;
    public GameObject hookPrefab;
    public float hookSpeed = 30f;
    public float maxDistance = 30f;
    public LayerMask grappleLayer;
    public InputActionProperty rightTriggerAction;

    public LineRenderer ropeLine;
    public string hookTipName = "HookTip";
    public float pullSpeed = 15f;

    private GameObject currentHook;
    private Transform hookTipTransform;
    private GameObject playerObject;
    private bool isGrappling = false;
    private bool isPulling = false;
    private Vector3 grapplePoint;

    private Vector3 startPosition;
    private float journeyLength;
    private float startTime;

    void Start()
    {
        // Cache player reference
        playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject == null)
        {
            Debug.LogError("No GameObject with tag 'Player' found in scene!");
        }
    }

    void Update()
    {
        float triggerValue = rightTriggerAction.action.ReadValue<float>();

        if (triggerValue > 0.8f && !isGrappling)
        {
            TryGrapple();
        }

        if (isGrappling && currentHook != null)
        {
            float distanceCovered = (Time.time - startTime) * hookSpeed;
            float fractionOfJourney = distanceCovered / journeyLength;

            currentHook.transform.position = Vector3.Lerp(startPosition, grapplePoint, fractionOfJourney);

            if (hookTipTransform != null)
            {
                ropeLine.enabled = true;
                ropeLine.SetPosition(0, firePoint.position);
                ropeLine.SetPosition(1, hookTipTransform.position);
            }

            if (fractionOfJourney >= 1f && !isPulling)
            {
                isPulling = true;
            }
        }

        if (isPulling && playerObject != null)
        {
            float step = pullSpeed * Time.deltaTime;
            playerObject.transform.position = Vector3.MoveTowards(playerObject.transform.position, grapplePoint, step);

            if (Vector3.Distance(playerObject.transform.position, grapplePoint) < 0.5f)
            {
                isPulling = false;
            }
        }

        if (!isGrappling)
        {
            ropeLine.enabled = false;
        }
    }

    void TryGrapple()
    {
        RaycastHit hit;
        if (Physics.Raycast(firePoint.position, firePoint.forward, out hit, maxDistance, grappleLayer))
        {
            grapplePoint = hit.point;
            isGrappling = true;

            Quaternion rotationWithX90 = Quaternion.Euler(90f, 0f, 0f);
            currentHook = Instantiate(hookPrefab, firePoint.position, rotationWithX90);

            hookTipTransform = currentHook.transform.Find(hookTipName);
            if (hookTipTransform == null)
                Debug.LogWarning("HookTip not found inside the hook prefab.");

            startPosition = firePoint.position;
            journeyLength = Vector3.Distance(startPosition, grapplePoint);
            startTime = Time.time;

            Debug.Log("Hook fired toward: " + grapplePoint);

            Invoke(nameof(ResetGrapple), 5f);
        }
        else
        {
            Debug.Log("No valid grapple point hit.");
        }
    }

    void ResetGrapple()
    {
        isGrappling = false;
        isPulling = false;

        if (currentHook != null)
        {
            Destroy(currentHook);
            currentHook = null;
        }

        ropeLine.enabled = false;
    }

    public bool IsGrappling() => isGrappling;
    public Vector3 GetGrapplePoint() => grapplePoint;
}
