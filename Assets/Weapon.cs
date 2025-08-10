using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public abstract class Weapon : MonoBehaviour
{
    public abstract float holderHealth { get; set; }
    public abstract float attackPower { get; set; }
    public abstract float speed { get; set; }
    public abstract string weaponText { get; set; }
    public abstract float cooldownTime { get; set; }
    public abstract float lastAttackTime { get; set; }

    public abstract float critChance { get; set; }

    public abstract bool inAttackRange { get; set; }

    public abstract bool blockedPath { get; set; }
    public abstract Vector3 blockerPosition { get; set; }

    public Unit closestEnemy = null;

    public List<Unit> enemies = null;

    public void SetEnemies(List<Unit> enemies)
    {
        this.enemies = enemies;
    }


    internal abstract Vector3 getLocalTransform();

    internal virtual Vector2 GetMovement()
    {
        if (enemies == null || enemies.Count == 0)
        {
            return Vector2.zero;
        }

        Unit closestEnemy = null;
        float closestDistance = float.MaxValue;
        foreach (var enemy in enemies)
        {
            if (enemy != null && !enemy.isActiveAndEnabled)
            {
                continue;
            }
            float distance = Vector3.Distance(transform.position, enemy.transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestEnemy = enemy;
            }
        }
        this.closestEnemy = closestEnemy;

        if (inAttackRange)
        {
            return Vector2.zero;
        }

        if (closestEnemy != null)
        {
            Vector3 direction = (closestEnemy.transform.position - transform.position).normalized;
            if (blockedPath)
            {
                bool blockerInFront = Vector3.Dot(direction, (blockerPosition - transform.position).normalized) > 0;
                if (blockerInFront)
                {
                    if (direction.y < direction.x)
                    {
                        direction.y = -Math.Sign(direction.y) * direction.x;
                    }
                    else
                    {
                        direction.x = -Math.Sign(direction.x) * direction.y;
                    }
                }

                direction = direction.normalized;
            }
            return new Vector2(direction.x, direction.y) * speed;
        }

        return Vector2.zero;
    }

    internal void InitWeapon(string weaponText)
    {
        this.weaponText = weaponText;
        if (GetComponentInChildren<TextMeshPro>() != null)
        {
            GetComponentInChildren<TextMeshPro>().text = weaponText;
        }
    }
}
