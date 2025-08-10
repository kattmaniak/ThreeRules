using UnityEngine;

public class Sword : Weapon
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

    public Collider2D attackCollider;

    internal override Vector3 getLocalTransform()
    {
        return new Vector3(0.13f, -0.13f, 0);
    }

    public Sword()
    {
        holderHealth = 100f;
        attackPower = 10f;
        speed = 0.5f;
        cooldownTime = 1f;
        lastAttackTime = 0f;
        critChance = 0.01f;
        inAttackRange = false;
        blockedPath = false;
        blockerPosition = Vector3.zero;
    }

    public void Attack(Unit enemy)
    {
        if (Time.time >= lastAttackTime + cooldownTime)
        {
            enemy.TakeDamage(this);
            inAttackRange = false;
            lastAttackTime = Time.time;
        }
    }


    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.isTrigger)
        {
            return;
        }
        if (enemies.Contains(collision.gameObject.GetComponent<Unit>()))
        {
            inAttackRange = true;
            blockedPath = false;
            Attack(collision.gameObject.GetComponent<Unit>());
        }
        else if (collision.gameObject != gameObject.GetComponentInParent<Unit>().gameObject)
        {
            blockedPath = true;
            blockerPosition = collision.transform.position;
        }
    }

    public void OnTriggerStay2D(Collider2D collision)
    {
        if (enemies.Contains(collision.gameObject.GetComponent<Unit>()))
        {
            inAttackRange = true;
            Attack(collision.gameObject.GetComponent<Unit>());
        }
    }

    public void OnTriggerExit2D(Collider2D collision)
    {
        if(!isActiveAndEnabled)
        {
            return;
        }

        if (enemies.Contains(collision.gameObject.GetComponent<Unit>()))
        {
            blockedPath = false;
            inAttackRange = false;
        }
        else if (gameObject.GetComponentInParent<Unit>() == null || collision.gameObject != gameObject.GetComponentInParent<Unit>().gameObject || !collision.isTrigger)
        {
            blockedPath = false;
            inAttackRange = false;
            blockerPosition = Vector3.zero;
        }
    }
}
