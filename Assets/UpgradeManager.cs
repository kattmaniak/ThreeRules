using System.Collections.Generic;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;

public class UpgradeManager : MonoBehaviour
{
    private static UpgradeManager instance;

    public static UpgradeManager GetInstance()
    {
        if (instance == null)
        {
            Debug.LogError("UpgradeManager instance is null.");
        }
        return instance;
    }

    private List<Upgrade> selectableUpgrades = new List<Upgrade>();

    public TMP_Text notice;


    // Start is called before the first frame update
    void Start()
    {
        selectableUpgrades.Clear();
        instance = this;
        Debug.Log("UpgradeManager initialized.");

        if (Application.platform == RuntimePlatform.WebGLPlayer)
        {
            StartCoroutine(webGetUpgrades());
        }

        if (Database.GetUnits().Count == 0)
            {
                notice.text = "Select your starting army.";
            }

        List<string> upgrades = new List<string>();
        foreach (var upgrade in Database.GetAllUpgrades())
        {
            upgrades.Add(upgrade);
        }

        GameObject upgradePrefab = Resources.Load<GameObject>("Upgrade");
        for (int i = 0; i < 3; i++)
        {
            int choice = Random.Range(0, upgrades.Count);
            string[] upgrade = upgrades[choice].Split(';');
            if (upgradePrefab != null)
            {
                GameObject newUpgrade = Instantiate(upgradePrefab, new Vector3(-5f + i * 5f, 0, 0), Quaternion.identity);
                Upgrade upgradeComponent = newUpgrade.GetComponent<Upgrade>();
                if (upgradeComponent != null)
                {
                    upgradeComponent.InitUpgrade(upgrade[0], upgrade[1], upgrade[2]);
                }
                else
                {
                    Debug.LogWarning("Upgrade component not found on instantiated prefab.");
                }
                selectableUpgrades.Add(upgradeComponent);
                upgrades.RemoveAt(choice);
            }
            else
            {
                Debug.LogError("Upgrade prefab not found in Resources.");
            }
        }

    }

    private IEnumerator webGetUpgrades()
    {
        string url = Path.Combine(Application.streamingAssetsPath, "upgrades.txt");
        using (UnityEngine.Networking.UnityWebRequest www = UnityEngine.Networking.UnityWebRequest.Get(url))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityEngine.Networking.UnityWebRequest.Result.ConnectionError || www.result == UnityEngine.Networking.UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError("Error loading upgrades: " + www.error);
            }
            else
            {
                string[] lines = www.downloadHandler.text.Split('\n');
                List<string> upgrades = new List<string>(lines);
                Database.SetAllUpgrades(upgrades);
            }
        }
    }

    public void SelectUpgrade(Upgrade upgrade)
    {
        for (int i = 0; i < selectableUpgrades.Count; i++)
        {
            if (selectableUpgrades[i] != upgrade)
            {
                Destroy(selectableUpgrades[i].gameObject);
                selectableUpgrades.RemoveAt(i);
                i--;
            }
        }

        Invoke(nameof(LoadPrep), 0.5f);
    }
    
    private void LoadPrep()
    {
        Debug.Log("Loading preparation scene.");
        SceneManager.LoadScene("PrepScene");
    }
}
