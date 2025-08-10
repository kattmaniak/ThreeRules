using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PrepManager : MonoBehaviour
{
    public TMP_Text kingHealth;

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
    }


    public void StartBattleScene()
    {
        Debug.Log("Battle scene started.");
        SceneManager.LoadScene("Scenes/KingsScene");
    }
}