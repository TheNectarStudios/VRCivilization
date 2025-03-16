using UnityEngine;
using System.IO;
using UnityEngine.XR;

public class CameraCapture : MonoBehaviour
{
    public Camera captureCamera; // Assign the camera in the inspector
    public RenderTexture renderTexture; // Assign in the Inspector
    public string folderName = "CapturedImages"; // Folder to store images
    private int imageCount = 0;

    void Start()
    {
        // Create directory if it doesn't exist
        string path = Path.Combine(Application.persistentDataPath, folderName);
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }
    }

    void Update()
    {
        // Works with Mouse Click (PC) and VR Button (Oculus, OpenXR, SteamVR)
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
        if (renderTexture == null)
        {
            Debug.LogError("RenderTexture is not assigned!");
            return;
        }

        // Generate unique file name
        string fileName = "Image_" + imageCount + ".png";
        string filePath = Path.Combine(Application.persistentDataPath, folderName, fileName);
        imageCount++;

        // Capture from the assigned RenderTexture
        RenderTexture activeRT = RenderTexture.active;
        RenderTexture.active = renderTexture;

        Texture2D screenshot = new Texture2D(renderTexture.width, renderTexture.height, TextureFormat.RGB24, false);
        screenshot.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
        screenshot.Apply();

        // Save as PNG
        byte[] bytes = screenshot.EncodeToPNG();
        File.WriteAllBytes(filePath, bytes);

        // Restore previous render texture
        RenderTexture.active = activeRT;
        Destroy(screenshot);

        Debug.Log("Screenshot saved at: " + filePath);
    }
}
