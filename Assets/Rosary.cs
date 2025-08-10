using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Rosary : Weapon
{
    public override float holderHealth { get; set; }
    public override float attackPower { get; set; }
    public override float speed { get; set; }
    public override string weaponText { get; set; }
    public override float cooldownTime { get; set; }
    public override float lastAttackTime { get; set; }

    public override float critChance { get; set; }

    public override bool inAttackRange { get; set; }

    public override bool blockedPath { get; set; }
    public override Vector3 blockerPosition { get; set; }

    private List<Unit> alliesInRange = new List<Unit>();

    private List<Unit> allies = new List<Unit>();

    public void HealAllies()
    {
        if (Time.time - lastAttackTime < cooldownTime)
        {
            return;
        }

        if (!inAttackRange || alliesInRange.Count == 0)
        {
            return;
        }

        lastAttackTime = Time.time;

        foreach (var ally in alliesInRange)
        {
            if (ally != null && ally.isActiveAndEnabled)
            {
                ally.Heal(attackPower);
            }
        }
    }

    internal override Vector3 getLocalTransform()
    {
        return new Vector3(0.14f, -0.14f, 0);
    }

    public Rosary()
    {
        holderHealth = 100f;
        attackPower = 12f;
        speed = 0.3f;
        cooldownTime = 3.0f;
        lastAttackTime = 0f;
        critChance = 0f;
        inAttackRange = false;
        blockedPath = false;
        blockerPosition = Vector3.zero;
    }

    // Start is called before the first frame update
    void Start()
    {
        if (SceneManager.GetActiveScene().name == "CombatScene")
        {
            allies = CombatManager.instance.GetAllies(this);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (SceneManager.GetActiveScene().name == "CombatScene")
        {
            if (inAttackRange)
            {
                HealAllies();
            }
        }
    }

    internal override Vector2 GetMovement()
    {
        if (SceneManager.GetActiveScene().name != "CombatScene")
        {
            return Vector2.zero;
        }

        Unit lowestHealthAlly = null;
        float lowestHealth = float.MaxValue;

        alliesInRange.RemoveAll(ally => ally == null || !ally.isActiveAndEnabled);

        foreach (var ally in allies)
        {
            if (ally != null && ally.isActiveAndEnabled && ally.health < lowestHealth && ally.myWeapon != null && ally.myWeapon != this)
            {
                lowestHealth = ally.health / ally.myWeapon.holderHealth;
                lowestHealthAlly = ally;
            }
        }

        if (lowestHealthAlly != null && !alliesInRange.Contains(lowestHealthAlly))
        {
            Vector3 direction = (lowestHealthAlly.transform.position - transform.position).normalized;
            return new Vector2(direction.x, direction.y) * speed;
        }

        Vector2 directionToScreenCenter = (Camera.main.ScreenToWorldPoint(new Vector3(Screen.width / 2, Screen.height / 2, Camera.main.transform.position.z)) - transform.position).normalized;

        return directionToScreenCenter * 0.00001f;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Unit unit = collision.GetComponent<Unit>();
        if (unit != null && unit.isActiveAndEnabled && allies.Contains(unit))
        {
            alliesInRange.Add(unit);
            inAttackRange = true;
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        Unit unit = collision.GetComponent<Unit>();
        if (unit != null && unit.isActiveAndEnabled && allies.Contains(unit))
        {
            inAttackRange = true;
        }
        else if (unit != null && !alliesInRange.Contains(unit))
        {
            alliesInRange.Add(unit);
            inAttackRange = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        Unit unit = collision.GetComponent<Unit>();
        if (unit != null && alliesInRange.Contains(unit))
        {
            alliesInRange.Remove(unit);
            if (alliesInRange.Count == 0)
            {
                inAttackRange = false;
            }
        }
    }
}
