using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    private Weapon weapon;
    private Vector3 direction;

    private bool fired = false;

    public void Init(Weapon weapon)
    {
        this.weapon = weapon;
    }

    public void Fire(Vector3 direction)
    {
        if (fired)
        {
            return;
        }

        this.direction = direction.normalized;
        transform.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg);
        fired = true;

        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.velocity = this.direction * 3;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Unit unit = collision.GetComponent<Unit>();
        Unit parentUnit = weapon.GetComponentInParent<Unit>();
        if (unit != null && unit != parentUnit && CombatManager.instance.AreEnemies(unit, parentUnit))
        {
            unit.TakeDamage(weapon);
            Destroy(gameObject);
        }
    }
}
