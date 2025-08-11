using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    public GameObject tutorialPanel;

    [SerializeField] Slider volumeSlider;

    public Button exitGameButton;

    public static void StartGame()
    {
        SceneManager.LoadScene("UpgradeScene");
    }
    public static void ExitGame()
    {
        Debug.Log("Exiting game.");
        Application.Quit();
    }

    public void ShowTutorial()
    {
        tutorialPanel.SetActive(true);
    }

    public void HideTutorial()
    {
        tutorialPanel.SetActive(false);
    }

    public void ChangeVolume()
    {
        float volume = volumeSlider.value;
        AudioListener.volume = volume;
        PlayerPrefs.SetFloat("Volume", volume);
        PlayerPrefs.Save();
    }

    private void Start()
    {
        if (PlayerPrefs.HasKey("Volume"))
        {
            float savedVolume = PlayerPrefs.GetFloat("Volume");
            AudioListener.volume = savedVolume;
            volumeSlider.value = savedVolume;
        }
        else
        {
            AudioListener.volume = 0.5f; // Default volume
            volumeSlider.value = 0.5f;
        }

        if (Application.platform == RuntimePlatform.WebGLPlayer)
        {
            exitGameButton.gameObject.SetActive(false);
        }
    }
}
