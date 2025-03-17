using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video; 
public class SkyboxVideoManager : MonoBehaviour
{
    public Material skyboxMaterial;  // Assign Skybox Material in Inspector
    public VideoPlayer videoPlayer;  // Assign Video Player in Inspector
    public string nextSceneName = "NextScene"; // Change to your scene name

    void Start()
    {
        // Ensure video is not playing at start
        videoPlayer.enabled = false;
    }   

    public void PlaySkyboxVideo()
    {
        // Enable video player and start playing
        videoPlayer.enabled = true;
        videoPlayer.Play();

        // Assign the Skybox material
        RenderSettings.skybox = skyboxMaterial;

        // Change scene after 3 seconds
        Invoke("ChangeScene", 3f);
    }

    void ChangeScene()
    {
        SceneManager.LoadScene(nextSceneName);
    }
}
