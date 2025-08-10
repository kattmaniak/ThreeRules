using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CombatManager : MonoBehaviour
{
    public static CombatManager instance;



    private List<Unit> unitsInCombat = new List<Unit>();
    private List<Unit> enemiesInCombat = new List<Unit>();
    public void RegisterUnitInCombat(Unit unit)
    {
        if (!unitsInCombat.Contains(unit))
        {
            unitsInCombat.Add(unit);
            unit.SetInCombat(true);
            Debug.Log("Unit registered in combat: " + unit.name);
        }
    }

    public void RegisterEnemyInCombat(Unit enemy)
    {
        if (!enemiesInCombat.Contains(enemy))
        {
            enemiesInCombat.Add(enemy);
            enemy.SetInCombat(true);
            Debug.Log("Enemy registered in combat: " + enemy.name);
        }
    }

    public void RemoveUnitFromCombat(Unit unit)
    {
        if (unitsInCombat.Contains(unit))
        {
            unitsInCombat.Remove(unit);
            unit.SetInCombat(false);
            Debug.Log("Unit unregistered from combat: " + unit.name);
            if (unitsInCombat.Count == 0)
            {
                Debug.Log("All units have exited combat.");
                EndBattle(3);
            }
        }

        if (enemiesInCombat.Contains(unit))
        {
            enemiesInCombat.Remove(unit);
            unit.SetInCombat(false);
            Debug.Log("Enemy unregistered from combat: " + unit.name);
            if (enemiesInCombat.Count == 0)
            {
                Debug.Log("All enemies have been defeated.");
                EndBattle(Database.GetBattleKing());
            }
        }
    }

    public bool AreEnemies(Unit first, Unit second)
    {
        return unitsInCombat.Contains(first) && enemiesInCombat.Contains(second) ||
               unitsInCombat.Contains(second) && enemiesInCombat.Contains(first);
    }

    private void EndBattle(int loser)
    {
        Database.DamageKing(loser);
        Debug.Log("Battle ended.");
        if (Database.AllKingsDefeated())
        {
            Debug.Log("All kings defeated. Loading victory scene.");
            SceneManager.LoadScene("VictoryScene");
        }
        else if (loser == 3 && Database.GetKingHP(3) <= 0)
        {
            Debug.Log("King 3 defeated. Loading defeat scene.");
            SceneManager.LoadScene("DefeatScene");
        }
        Invoke(nameof(LoadUpgrade), 0.5f);
    }

    private void LoadUpgrade()
    {
        Debug.Log("Loading upgrade scene.");
        SceneManager.LoadScene("UpgradeScene");
    }

    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        Debug.Log("CombatManager initialized.");

        GameObject unitPrefab = Resources.Load<GameObject>("Unit");
        foreach (var unit in Database.GetUnits())
        {
            Vector3 unitPosition = new Vector3(unit.Item2.x - 4f, unit.Item2.y, unit.Item2.z);
            if (unitPrefab != null)
            {
                GameObject newUnit = Instantiate(unitPrefab, unitPosition, Quaternion.identity);
                Unit unitComponent = newUnit.GetComponent<Unit>();
                if (unitComponent != null)
                {
                    unitComponent.InitBody("3");
                    Database.InitWeapon(unit.Item1, unitComponent);
                    foreach (var upgrade in Database.GetActiveUpgrades())
                    {
                        ApplyUpgrade(unitComponent, unitComponent.myWeapon, upgrade);
                    }
                    RegisterUnitInCombat(unitComponent);
                }
                else
                {
                    Debug.LogWarning("Unit component not found on instantiated prefab.");
                }
            }
            else
            {
                Debug.LogError("Unit prefab not found in Resources.");
            }
        }

        foreach (var enemy in Database.GetEnemies())
        {
            Vector3 enemyPosition = new Vector3(enemy.Item2.x + 4f, enemy.Item2.y, enemy.Item2.z);
            if (unitPrefab != null)
            {
                GameObject newEnemy = Instantiate(unitPrefab, enemyPosition, Quaternion.identity);
                Unit enemyComponent = newEnemy.GetComponent<Unit>();
                if (enemyComponent != null)
                {
                    enemyComponent.InitBody(Database.GetBattleKing().ToString());
                    Database.InitWeapon(enemy.Item1, enemyComponent);
                    enemyComponent.SetEnemy();
                    RegisterEnemyInCombat(enemyComponent);
                }
                else
                {
                    Debug.LogWarning("Enemy component not found on instantiated prefab.");
                }
            }
            else
            {
                Debug.LogError("Enemy prefab not found in Resources.");
            }
        }

        foreach (Unit unit in unitsInCombat)
        {
            unit.myWeapon.SetEnemies(enemiesInCombat);
        }

        foreach (Unit enemy in enemiesInCombat)
        {
            enemy.myWeapon.SetEnemies(unitsInCombat);
        }
    }


    private void ApplyUpgrade(Unit unit, Weapon weapon, string upgrade)
    {
        if (unit.weaponID[0] == upgrade[1] || upgrade[1] == 'X')
        {
            switch (upgrade[0])
            {
                case 'H':
                    weapon.holderHealth += float.Parse(upgrade[2..]) * weapon.holderHealth;
                    unit.health = weapon.holderHealth;
                    break;
                case 'A':
                    weapon.attackPower += float.Parse(upgrade[2..]) * weapon.attackPower;
                    break;
                case 'S':
                    weapon.speed += float.Parse(upgrade[2..]) * weapon.speed;
                    break;
                case 'W':
                    weapon.cooldownTime -= float.Parse(upgrade[2..]) * weapon.cooldownTime;
                    break;
                case 'C':
                    weapon.critChance += float.Parse(upgrade[2..]);
                    break;
                default:
                    Debug.LogWarning("Unknown upgrade type: " + upgrade[0]);
                    break;
            }
        }
    }

    internal List<Unit> GetAllies(Weapon weapon)
    {
        foreach (Unit unit in unitsInCombat)
        {
            if (unit.myWeapon == weapon)
            {
                return unitsInCombat;
            }
        }
        return enemiesInCombat;
    }
}
