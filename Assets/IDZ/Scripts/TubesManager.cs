using System.Collections.Generic;
using UnityEngine;

public class TestTube : MonoBehaviour
{
    public Transform[] snapPoints; // Snap positions inside the tube
    private GameObject[] occupiedSlots; // Tracks occupied slots
    private static TestTube selectedTube = null; // Stores selected tube
    private static GameObject selectedSphere = null; // The selected sphere to move

    private void Start()
    {
        occupiedSlots = new GameObject[snapPoints.Length]; // Initialize slot tracking
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Red") && !other.CompareTag("Blue") && !other.CompareTag("Green"))
            return; // Ignore if not a sphere

        if (System.Array.Exists(occupiedSlots, obj => obj == other.gameObject))
            return; // Ignore if already inside

        AddSphere(other.gameObject);
    }

    private void OnMouseDown()
    {
        if (selectedTube == null) // No tube selected, select this one
        {
            selectedSphere = GetTopSphere();
            if (selectedSphere != null)
            {
                selectedTube = this;
                Debug.Log($"Selected {selectedSphere.name} from {gameObject.name}");
            }
        }
        else if (selectedTube == this) // Clicking the same tube cancels selection
        {
            Debug.Log($"Deselected {selectedSphere.name} from {gameObject.name}");
            selectedTube = null;
            selectedSphere = null;
        }
        else // Second tube clicked, attempt transfer
        {
            if (selectedSphere != null)
            {
                TransferSphere(selectedTube, this);
            }
        }
    }

    private GameObject GetTopSphere()
    {
        for (int i = occupiedSlots.Length - 1; i >= 0; i--)
        {
            if (occupiedSlots[i] != null) return occupiedSlots[i];
        }
        return null;
    }

    private Transform GetFirstEmptySnapPoint()
    {
        for (int i = 0; i < snapPoints.Length; i++)
        {
            if (occupiedSlots[i] == null) return snapPoints[i];
        }
        return null;
    }

    private void TransferSphere(TestTube fromTube, TestTube toTube)
    {
        Transform emptySlot = toTube.GetFirstEmptySnapPoint();
        if (emptySlot == null) return; // No space available

        Debug.Log($"Transferring {selectedSphere.name} from {fromTube.name} to {toTube.name}");

        fromTube.RemoveTopSphere(); // Clear the slot before adding to the new tube
        toTube.AddSphere(selectedSphere);

        selectedTube = null; // Reset selection
        selectedSphere = null;
    }

    public void RemoveTopSphere()
    {
        for (int i = occupiedSlots.Length - 1; i >= 0; i--)
        {
            if (occupiedSlots[i] != null)
            {
                occupiedSlots[i].transform.SetParent(null); // Remove parenting
                occupiedSlots[i] = null; // Clear the slot properly
                break;
            }
        }
    }

    private System.Collections.IEnumerator SnapSphere(GameObject sphere, Transform snapPoint)
    {
        Vector3 startPos = sphere.transform.position;
        Vector3 endPos = snapPoint.position;
        float duration = 0.2f;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            sphere.transform.position = Vector3.Lerp(startPos, endPos, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        sphere.transform.position = endPos;
        sphere.transform.SetParent(transform); // Make sure sphere is a child of the tube
    }

    public void AddSphere(GameObject sphere)
    {
        Transform snapPoint = GetFirstEmptySnapPoint();
        if (snapPoint != null)
        {
            StartCoroutine(SnapSphere(sphere, snapPoint));
            
            // Assign the sphere to the next available slot
            for (int i = 0; i < occupiedSlots.Length; i++)
            {
                if (occupiedSlots[i] == null)
                {
                    occupiedSlots[i] = sphere;
                    break;
                }
            }

            Rigidbody rb = sphere.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.isKinematic = true;
                rb.useGravity = false;
            }
        }
    }
}
