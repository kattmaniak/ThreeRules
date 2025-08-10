using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class KingButton : MonoBehaviour
{

    public int king;
    public TMP_Text body;
    public TMP_Text crown;
    public TMP_Text hpText;

    private int hp;

    private bool buttonActive = true;

    // Start is called before the first frame update
    void Start()
    {
        hp = Database.GetKingHP(king);

        body.text = king.ToString();

        if (hp <= 0)
        {
            crown.rectTransform.localPosition = new Vector3(0.2f, 0.1f, -0.1f);
            crown.rectTransform.rotation = Quaternion.Euler(0, 0, -60);
            body.rectTransform.localPosition = new Vector3(-0.1f, 0.1f, -0.1f);
            body.rectTransform.rotation = Quaternion.Euler(0, 0, 85);
            hpText.text = "";
            buttonActive = false;
        }
        else
        {
            hpText.text = ".";
            for (int i = 0; i < hp; i++)
            {
                hpText.text += "0";
            }
        }

    }


    private void OnMouseDown()
    {
        if (!buttonActive)
        {
            Debug.LogWarning("Button is not active.");
            return;
        }

        foreach (var obj in gameObject.scene.GetRootGameObjects())
        {
            if (obj.GetComponent<KingButton>() != null)
            {
                obj.GetComponent<KingButton>().buttonActive = false;
            }
        }


        Debug.Log("King " + king + " selected.");
        Database.SelectKingForBattle(king);

        Invoke(nameof(LoadCombat), 0.5f);
    }

    private void LoadCombat()
    {
        Debug.Log("Loading combat scene for King " + king);
        SceneManager.LoadScene("CombatScene");
    }
}
