using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    public GameObject tutorialPanel;

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
}
