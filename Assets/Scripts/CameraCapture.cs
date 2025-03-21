using UnityEngine;
using System.IO;
using System.Collections.Generic;
using UnityEngine.XR;

public class CameraCapture : MonoBehaviour
{
    public Camera captureCamera; // Assign in Inspector
    public RenderTexture renderTexture; // Assign in Inspector
    public string folderName = "CapturedImages"; // Folder to store images
    private int imageCount = 0;
    public string saveFilePath; // Path for saving JSON

    private Dictionary<string, List<string>> imageDetails = new Dictionary<string, List<string>>();

    void Start()
    {
        // Ensure dictionary is initialized
        imageDetails = new Dictionary<string, List<string>>();

        // Create directory if it doesn't exist
        string path = Path.Combine(Application.persistentDataPath, folderName);
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }

        // Define save file path
        saveFilePath = Path.Combine(Application.persistentDataPath, "ImageData.json");

        // Load previous image data
        LoadImageData();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) || IsVRButtonPressed())
        {
            TakeScreenshot();
        }
    }

    bool IsVRButtonPressed()
    {
        InputDevice device = InputDevices.GetDeviceAtXRNode(XRNode.RightHand);
        bool isPressed = false;
        return device.TryGetFeatureValue(CommonUsages.primaryButton, out isPressed) && isPressed;
    }

    void TakeScreenshot()
    {
        if (captureCamera == null)
        {
            Debug.LogError("❌ Capture Camera is NOT assigned!");
            return;
        }

        if (renderTexture == null)
        {
            Debug.LogError("❌ RenderTexture is NOT assigned!");
            return;
        }

        // Generate unique file name
        string fileName = "Image_" + imageCount + ".png";
        string filePath = Path.Combine(Application.persistentDataPath, folderName, fileName);
        imageCount++;

        // Capture the screenshot
        RenderTexture activeRT = RenderTexture.active;
        RenderTexture.active = renderTexture;

        Texture2D screenshot = new Texture2D(renderTexture.width, renderTexture.height, TextureFormat.RGB24, false);
        screenshot.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
        screenshot.Apply();

        byte[] bytes = screenshot.EncodeToPNG();
        File.WriteAllBytes(filePath, bytes);

        RenderTexture.active = activeRT;
        Destroy(screenshot);

        // Find visible objects and store their details
        List<string> detectedObjects = GetObjectsInView();
        imageDetails[fileName] = detectedObjects;

        // Save updated data to JSON
        SaveImageData();

        Debug.Log($"📸 Screenshot saved: {filePath}");
        Debug.Log($"🔍 Objects in Image: {fileName} → {string.Join(", ", detectedObjects)}");
    }

    List<string> GetObjectsInView()
    {
        List<string> detectedObjects = new List<string>();

        if (captureCamera == null)
        {
            Debug.LogError("❌ Capture Camera is NOT assigned in Inspector.");
            return detectedObjects;
        }

        Plane[] planes = GeometryUtility.CalculateFrustumPlanes(captureCamera);

        foreach (ObjectDetails obj in FindObjectsOfType<ObjectDetails>())
        {
            Collider objCollider = obj.GetComponent<Collider>();
            if (objCollider != null && GeometryUtility.TestPlanesAABB(planes, objCollider.bounds))
            {
                detectedObjects.Add($"{obj.objectName} ({obj.description})");
            }
        }
        return detectedObjects;
    }

    void SaveImageData()
    {
        List<ImageEntry> imageEntries = new List<ImageEntry>();

        foreach (var entry in imageDetails)
        {
            imageEntries.Add(new ImageEntry(entry.Key, entry.Value));
        }

        string json = JsonUtility.ToJson(new ImageDataWrapper { data = imageEntries }, true);
        File.WriteAllText(saveFilePath, json);
        Debug.Log("✅ Image data saved to: " + saveFilePath);
    }

    void LoadImageData()
    {
        if (File.Exists(saveFilePath))
        {
            string json = File.ReadAllText(saveFilePath);
            ImageDataWrapper loadedData = JsonUtility.FromJson<ImageDataWrapper>(json);

            imageDetails = new Dictionary<string, List<string>>();
            if (loadedData != null && loadedData.data != null)
            {
                foreach (var entry in loadedData.data)
                {
                    imageDetails[entry.imageName] = entry.detectedObjects;
                }
            }

            Debug.Log("🔄 Image data loaded from: " + saveFilePath);
        }
        else
        {
            Debug.Log("⚠ No previous image data found.");
        }
    }

    public List<string> GetDetailsForImage(string imageName)
    {
        return imageDetails.ContainsKey(imageName) ? imageDetails[imageName] : new List<string> { "No details found." };
    }
}
