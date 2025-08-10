using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class Database
{
    private static List<Tuple<string, Vector3>> units = new List<Tuple<string, Vector3>>();

    private static List<Tuple<string, Vector3>> enemies = new List<Tuple<string, Vector3>>();

    private static List<int> kingsHP = new List<int>
    {
        0, 3, 3, 3, 3, 3, 3, 3, 3, 3
    };

    private static int battleKing = 1;

    private static List<string> allUpgrades = new List<string>();

    private static List<string> activeUpgrades = new List<string>();



    static Database()
    {
        InitData();
    }

    public static void ResetDatabase()
    {
        units.Clear();
        enemies.Clear();
        kingsHP = new List<int> { 0, 3, 3, 3, 3, 3, 3, 3, 3, 3 };
        battleKing = 1;
        allUpgrades.Clear();
        activeUpgrades.Clear();
        InitData();
    }

    public static void AddUnit(string unitName, Vector3 position)
    {
        units.Add(new Tuple<string, Vector3>(unitName, position));
    }

    public static List<Tuple<string, Vector3>> GetUnits()
    {
        return units;
    }

    public static List<Tuple<string, Vector3>> GetEnemies()
    {
        return enemies;
    }

    public static int GetKingHP(int index)
    {
        if (index < 1 || index > 9)
        {
            Debug.LogError("Index out of bounds for enemy kings HP.");
            return 0;
        }
        return kingsHP[index];
    }

    public static void DamageKing(int index)
    {
        if (index < 1 || index > 9)
        {
            Debug.LogError("Index out of bounds for enemy kings HP.");
            return;
        }
        if (kingsHP[index] > 0)
        {
            kingsHP[index]--;
            Debug.Log("King " + index + " damaged. Remaining HP: " + kingsHP[index]);
        }
        else
        {
            Debug.LogWarning("King " + index + " is already defeated.");
        }
    }

    public static bool AllKingsDefeated()
    {
        for (int i = 1; i <= 9; i++)
        {
            if (kingsHP[i] > 0 && i != 3)
            {
                return false;
            }
        }
        return true;
    }

    public static void UpdateUnitPosition(string unitName, Vector3 newPosition)
    {
        for (int i = 0; i < units.Count; i++)
        {
            if (units[i].Item1 == unitName)
            {
                units[i] = new Tuple<string, Vector3>(unitName, newPosition);
                return;
            }
        }
    }

    public static void InitData()
    {
        string upgradesFilePath = Path.Combine(Application.streamingAssetsPath, "upgrades.txt");
        if (File.Exists(upgradesFilePath))
        {
            string[] lines = File.ReadAllLines(upgradesFilePath);
            foreach (string line in lines)
            {
                allUpgrades.Add(line);
            }
        }
        else
        {
            Debug.LogError("Upgrades file not found at: " + upgradesFilePath);
        }
    }

    internal static List<string> GetAllUpgrades()
    {
        if (units.Count == 0)
        {
            List<string> defaultUpgrades = new List<string>
            {
                "Swordsmen;Three Swordsmen Units, Strong melee fighters;UI3",
                "Peasants;Three Scythe Units, Powerful but frail AOE;UL3",
                "Archers;Three Archer Units, Long-range attackers;UD3",
            };
            return defaultUpgrades;
        }
        return allUpgrades;
    }

    internal static List<string> GetActiveUpgrades()
    {
        return activeUpgrades;
    }

    internal static void AddActiveUpgrade(string upgrade)
    {
        activeUpgrades.Add(upgrade);
        Debug.Log("Active upgrade added: " + upgrade);
    }

    internal static void SelectKingForBattle(int king)
    {
        Debug.Log("King " + king + " selected for battle.");
        battleKing = king;

        enemies.Clear();

        int numberOfEnemies = 4 - GetKingHP(king) + king + UnityEngine.Random.Range(0, 2);

        for (int i = 0; i < numberOfEnemies; i++)
        {
            string enemyType = "I";
            float enemyTypeChance = UnityEngine.Random.Range(0f, 1f);
            if (enemyTypeChance < 0.05f)
            {
                enemyType = "+";
            }
            else if (enemyTypeChance < 0.35f)
            {
                enemyType = "L";
            }
            else if (enemyTypeChance < 0.65f)
            {
                enemyType = "D";
            }
            enemyType += (i + 101).ToString();
            Vector3 enemyPosition = new Vector3(UnityEngine.Random.Range(-3f, 3f), UnityEngine.Random.Range(-3f, 3f), 0);
            enemies.Add(new Tuple<string, Vector3>(enemyType, enemyPosition));
        }
    }
    
    internal static int GetBattleKing()
    {
        return battleKing;
    }

    internal static void InitWeapon(string item1, Unit unitComponent)
    {
        if (item1[0] == 'I')
        {
            unitComponent.InitWeapon(UnityEngine.Object.Instantiate(Resources.Load<GameObject>("Sword")), item1);
        }
        else if (item1[0] == 'D')
        {
            unitComponent.InitWeapon(UnityEngine.Object.Instantiate(Resources.Load<GameObject>("Bow")), item1);
        }
        else if (item1[0] == 'L')
        {
            unitComponent.InitWeapon(UnityEngine.Object.Instantiate(Resources.Load<GameObject>("Scythe")), item1);
        }
        else if (item1[0] == '+')
        {
            unitComponent.InitWeapon(UnityEngine.Object.Instantiate(Resources.Load<GameObject>("Rosary")), item1);
        }
        else
        {
            Debug.LogWarning("Unit type not recognized, defaulting to Sword.");
            unitComponent.InitWeapon(UnityEngine.Object.Instantiate(Resources.Load<GameObject>("Sword")), "I" + item1[1..]);
        }
    }
}
