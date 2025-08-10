using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuButton : MonoBehaviour
{
    // Start is called before the first frame update
    public void MainMenu()
    {
        Debug.Log("Returning to main menu.");
        Database.ResetDatabase();
        SceneManager.LoadScene("MenuScene");
    }
}
