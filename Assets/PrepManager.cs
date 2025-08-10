using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PrepManager : MonoBehaviour
{
    public TMP_Text kingHealth;

    public TMP_Text activeUpgradesText;

    // Start is called before the first frame update
    void Start()
    {
        List<Tuple<string, Vector3>> units = Database.GetUnits();

        foreach (var unit in units)
        {
            GameObject unitPrefab = Resources.Load<GameObject>("Unit");
            if (unitPrefab != null)
            {
                GameObject newUnit = Instantiate(unitPrefab, unit.Item2, Quaternion.identity);
                Unit unitComponent = newUnit.GetComponent<Unit>();
                if (unitComponent != null)
                {
                    unitComponent.InitBody("3");
                    Database.InitWeapon(unit.Item1, unitComponent);
                }
                else
                {
                    Debug.LogError("Unit component not found on instantiated prefab.");
                }
            }
            else
            {
                Debug.LogError("Unit prefab not found in Resources.");
            }
        }

        int kingHP = Database.GetKingHP(3);
        kingHealth.text = ".";
        for (int i = 0; i < kingHP; i++)
        {
            kingHealth.text += "0";
        }

        List<string> activeUpgrades = Database.GetActiveUpgrades();
        if (activeUpgrades.Count == 0)
        {
            activeUpgradesText.text = "No active upgrades.";
            return;
        }
        List<string> affectsAll = new List<string>();
        List<string> affectsSwordsmen = new List<string>();
        List<string> affectsArchers = new List<string>();
        List<string> affectsPeasants = new List<string>();
        List<string> affectsPriests = new List<string>();

        foreach (var upgrade in activeUpgrades)
        {
            string[] splitUpgrade = upgrade.Split(':');
            foreach (var part in splitUpgrade)
            {
                if (part[1] == 'X')
                {
                    affectsAll.Add(part);
                }
                else if (part[1] == 'I')
                {
                    affectsSwordsmen.Add(part);
                }
                else if (part[1] == 'D')
                {
                    affectsArchers.Add(part);
                }
                else if (part[1] == 'L')
                {
                    affectsPeasants.Add(part);
                }
                else if (part[1] == '+')
                {
                    affectsPriests.Add(part);
                }
            }
        }
        activeUpgradesText.text = "";
        if (affectsAll.Count > 0)
        {
            activeUpgradesText.text += "All units:\n";
            foreach (var upgrade in affectsAll)
            {
                activeUpgradesText.text += ParseUpgradeText(upgrade) + "\n";
            }
        }
        if (affectsSwordsmen.Count > 0)
        {
            activeUpgradesText.text += "Swordsmen:\n";
            foreach (var upgrade in affectsSwordsmen)
            {
                activeUpgradesText.text += ParseUpgradeText(upgrade) + "\n";
            }
        }
        if (affectsArchers.Count > 0)
        {
            activeUpgradesText.text += "Archers:\n";
            foreach (var upgrade in affectsArchers)
            {
                activeUpgradesText.text += ParseUpgradeText(upgrade) + "\n";
            }
        }
        if (affectsPeasants.Count > 0)
        {
            activeUpgradesText.text += "Peasants:\n";
            foreach (var upgrade in affectsPeasants)
            {
                activeUpgradesText.text += ParseUpgradeText(upgrade) + "\n";
            }
        }
        if (affectsPriests.Count > 0)
        {
            activeUpgradesText.text += "Priests:\n";
            foreach (var upgrade in affectsPriests)
            {
                activeUpgradesText.text += ParseUpgradeText(upgrade) + "\n";
            }
        }
    }

    private string ParseUpgradeText(string part)
    {
        string res = "";
        switch (part[0])
        {
            case 'A':
                res = "Attack Power: " + (part[2] == '-' ? "" : "+") + (int)(float.Parse(part[2..]) * 100) + "%";
                break;
            case 'H':
                res = "Health: " + (part[2] == '-' ? "" : "+") + (int)(float.Parse(part[2..]) * 100) + "%";
                break;
            case 'S':
                res = "Speed: " + (part[2] == '-' ? "" : "+") + (int)(float.Parse(part[2..]) * 100) + "%";
                break;
            case 'W':
                res = "Cooldown: " + (part[2] == '-' ? "" : "+") + (int)(float.Parse(part[2..]) * 100) + "%";
                break;
            case 'C':
                res = "Critical Chance: " + (part[2] == '-' ? "" : "+") + (int)(float.Parse(part[2..]) * 100) + "%";
                break;
        }
        return res;
    }


    public void StartBattleScene()
    {
        Debug.Log("Battle scene started.");
        SceneManager.LoadScene("Scenes/KingsScene");
    }
}