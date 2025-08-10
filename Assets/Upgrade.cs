using TMPro;
using UnityEngine;

public class Upgrade : MonoBehaviour
{
    public TMP_Text upgradeTitle;
    public TMP_Text upgradeDescription;

    private string effect;

    public void InitUpgrade(string title, string description, string effect)
    {
        upgradeTitle.text = title;
        upgradeDescription.text = description;
        this.effect = effect;
    }


    public string GetEffect()
    {
        return effect;
    }

    private void OnMouseDown()
    {
        Debug.Log("Upgrade selected: " + effect);

        ActivateUpgrade();

        UpgradeManager.GetInstance().SelectUpgrade(this);
    }

    private void ActivateUpgrade()
    {
        string[] effectParts = effect.Split(':');
        foreach (var part in effectParts)
        {
            switch (part[0])
            {
                case 'U':
                    // Add a new unit
                    string unitType = part[1].ToString();
                    int unitCount = int.Parse(part[2..]);
                    int unitStartID = Database.GetUnits().Count + 1;
                    for (int i = 0; i < unitCount; i++)
                    {
                        Vector3 spawnPosition = new Vector3(UnityEngine.Random.Range(-1f, 1f), UnityEngine.Random.Range(-1f, 1f), 0); // Adjust as needed
                        Database.AddUnit(unitType + (unitStartID + i), spawnPosition);
                    }
                    break;
                case 'A': // Attack power
                case 'S': // Speed
                case 'W': // Weapon speed / Cooldown
                case 'H': // Health
                case 'C': // Critical hit chance
                    // Add an active upgrade
                    Database.AddActiveUpgrade(part);
                    break;
                default:
                    Debug.LogError("Unknown upgrade effect: " + effect);
                    break;
            }
        }
    }
}
