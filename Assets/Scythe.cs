using System.Collections.Generic;
using UnityEngine;

public class Scythe : Weapon
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

    private List<Unit> enemiesInRange = new List<Unit>();

    public Animator animator;

    internal override Vector3 getLocalTransform()
    {
        return new Vector3(0.4f, 0.25f, 0);
    }

    public Scythe()
    {
        holderHealth = 70f;
        attackPower = 22f;
        speed = 0.6f;
        cooldownTime = 2.5f;
        lastAttackTime = 0f;
        critChance = 0.01f;
        inAttackRange = false;
        blockedPath = false;
        blockerPosition = Vector3.zero;
    }


    public void Attack()
    {
        if (Time.time - lastAttackTime < cooldownTime)
        {
            return;
        }

        if (!inAttackRange || enemiesInRange.Count == 0)
        {
            return;
        }

        lastAttackTime = Time.time;

        enemiesInRange.RemoveAll(enemy => enemy == null || !enemy.isActiveAndEnabled);

        for (int i = 0; i < enemiesInRange.Count; i++)
        {
            Unit enemy = enemiesInRange[i];
            if (enemy == null || !enemy.isActiveAndEnabled)
            {
                continue;
            }
            enemy.TakeDamage(this);
        }
        if (enemiesInRange.Count > 0)
        {
            animator.Play("ScytheSlash");
        }
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {

        if (collision.isTrigger || CombatManager.instance == null)
        {
            return;
        }
        Unit unit = collision.GetComponent<Unit>();
        if (unit != null && !enemiesInRange.Contains(unit) && CombatManager.instance.AreEnemies(unit, GetComponentInParent<Unit>()))
        {
            enemiesInRange.Add(unit);
            inAttackRange = true;
            Attack();
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.isTrigger || CombatManager.instance == null)
        {
            return;
        }
        Unit unit = collision.GetComponent<Unit>();
        if (unit != null && !enemiesInRange.Contains(unit) && CombatManager.instance.AreEnemies(unit, GetComponentInParent<Unit>()))
        {
            enemiesInRange.Add(unit);
            inAttackRange = true;
            Attack();
        }
        else if (unit != null && enemiesInRange.Contains(unit))
        {
            inAttackRange = true;
            Attack();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.isTrigger || CombatManager.instance == null)
        {
            return;
        }
        Unit unit = collision.GetComponent<Unit>();
        if (unit != null && enemiesInRange.Contains(unit))
        {
            enemiesInRange.Remove(unit);
            if (enemiesInRange.Count == 0)
            {
                inAttackRange = false;
            }
        }
    }

}
