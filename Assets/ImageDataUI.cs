using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.IO;
using System.Collections.Generic;
using System.Text.RegularExpressions;

public class ImageDataUI : MonoBehaviour
{
    public GameObject panel; // UI Panel inside Scroll View
    public GameObject objectPrefab; // Prefab with Name, Image Grid, and Description
    public GameObject imagePrefab; // RawImage prefab for images

    private Dictionary<string, List<string>> objectToImages = new Dictionary<string, List<string>>();
    private Dictionary<string, string> objectDescriptions = new Dictionary<string, string>(); // New dictionary for descriptions
    private string saveFilePath;
    private string imageFolderPath;

    void Start()
    {
        saveFilePath = Path.Combine(Application.persistentDataPath, "ImageData.json");
        imageFolderPath = Path.Combine(Application.persistentDataPath, "CapturedImages");

        LoadImageData();
        DisplayObjectDetails();
    }

    void LoadImageData()
    {
        if (File.Exists(saveFilePath))
        {
            string json = File.ReadAllText(saveFilePath);
            ImageDataWrapper loadedData = JsonUtility.FromJson<ImageDataWrapper>(json);

            objectToImages = new Dictionary<string, List<string>>();
            objectDescriptions = new Dictionary<string, string>();

            if (loadedData != null && loadedData.data != null)
            {
                foreach (var entry in loadedData.data)
                {
                    foreach (string obj in entry.detectedObjects)
                    {
                        string objectName = ExtractObjectName(obj);
                        string objectDesc = ExtractDescription(obj);

                        if (!objectToImages.ContainsKey(objectName))
                        {
                            objectToImages[objectName] = new List<string>();
                            objectDescriptions[objectName] = objectDesc; // Store description
                        }
                        objectToImages[objectName].Add(entry.imageName);
                    }
                }
            }
            Debug.Log("🔄 Image data loaded!");
        }
        else
        {
            Debug.LogError("⚠ No image data found!");
        }
    }

    void DisplayObjectDetails()
    {
        foreach (var entry in objectToImages)
        {
            string objectName = entry.Key;
            List<string> images = entry.Value;
            string objectDesc = objectDescriptions.ContainsKey(objectName) ? objectDescriptions[objectName] : "";

            // 1️⃣ Spawn Object Container
            GameObject objectContainer = Instantiate(objectPrefab, panel.transform);
            Debug.Log($"✅ Spawned UI for: {objectName}");

            // 2️⃣ Set Object Name
            TMP_Text nameText = objectContainer.transform.Find("ObjectName").GetComponent<TMP_Text>();
            nameText.text = $"🔍 {objectName}";

            // 3️⃣ Set Description
            TMP_Text descText = objectContainer.transform.Find("Description").GetComponent<TMP_Text>();
            descText.text = objectDesc;

            // 4️⃣ Image Grid
            Transform imageGrid = objectContainer.transform.Find("ImageGrid");

            foreach (string imageName in images)
            {
                string imagePath = Path.Combine(imageFolderPath, imageName);

                if (File.Exists(imagePath))
                {
                    GameObject imageObject = Instantiate(imagePrefab, imageGrid);
                    RawImage rawImage = imageObject.GetComponent<RawImage>();

                    Texture2D texture = LoadTexture(imagePath);
                    if (texture != null)
                    {
                        rawImage.texture = texture;
                        rawImage.SetNativeSize(); // Adjust image size
                        Debug.Log($"🖼 Image Loaded: {imageName}");
                    }
                    else
                    {
                        Debug.LogError($"⚠ Image Load Failed: {imageName}");
                    }
                }
                else
                {
                    Debug.LogError($"❌ Image Not Found: {imagePath}");
                }
            }
        }
    }

    string ExtractObjectName(string rawText)
    {
        return Regex.Replace(rawText, @"\s*\(.*?\)\s*", ""); // Remove anything in brackets
    }

    string ExtractDescription(string rawText)
    {
        Match match = Regex.Match(rawText, @"\((.*?)\)");
        return match.Success ? match.Groups[1].Value : "";
    }

    Texture2D LoadTexture(string filePath)
    {
        byte[] imageBytes = File.ReadAllBytes(filePath);
        Texture2D texture = new Texture2D(2, 2);
        if (texture.LoadImage(imageBytes))
        {
            return texture;
        }
        return null;
    }
}
