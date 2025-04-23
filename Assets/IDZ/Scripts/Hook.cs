using UnityEngine;
using System.Collections;

public class Hook : MonoBehaviour
{
    public float pullSpeed = 15f;
    public LayerMask validLayers;

    private GameObject playerObject;
    private bool hasHooked = false;

    void Start()
    {
        playerObject = GameObject.Find("XR Interaction Setup Variant Variant");

        if (playerObject == null)
        {
            Debug.LogError("❌ Player GameObject not found in scene!");
        }
        else
        {
            Debug.Log("✅ Player found: " + playerObject.name);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (hasHooked || playerObject == null) return;

        if (((1 << other.gameObject.layer) & validLayers) != 0)
        {
            Debug.Log("🪝 Hooked onto: " + other.gameObject.name);
            hasHooked = true;
            StartCoroutine(PullPlayerToHook());
        }
    }

    IEnumerator PullPlayerToHook()
    {
        while (Vector3.Distance(playerObject.transform.position, transform.position) > 0.3f)
        {
            playerObject.transform.position = Vector3.MoveTowards(
                playerObject.transform.position,
                transform.position,
                pullSpeed * Time.deltaTime
            );

            yield return null;
        }

        Debug.Log("✅ Player reached hook point.");
    }
}
