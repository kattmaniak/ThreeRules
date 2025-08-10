using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{

    public static void StartGame()
    {
        SceneManager.LoadScene("UpgradeScene");
    }
    public static void ExitGame()
    {
        Debug.Log("Exiting game.");
        Application.Quit();
    }
}
