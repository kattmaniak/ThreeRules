using UnityEngine;
using UnityEngine.SceneManagement;

public class Bow : Weapon
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

    public Animator animator;

    internal override Vector3 getLocalTransform()
    {
        return new Vector3(0.1f, -0.2f, 0);
    }

    public Bow()
    {
        holderHealth = 75f;
        attackPower = 7f;
        speed = 0.7f;
        cooldownTime = 2f;
        lastAttackTime = 0f;
        critChance = 0.01f;
        inAttackRange = false;
        blockedPath = false;
        blockerPosition = Vector3.zero;
    }

    // Update is called once per frame
    void Update()
    {
        if (SceneManager.GetActiveScene().name != "CombatScene")
        {
            Attack(closestEnemy);
        }
    }

    public void Attack(Unit enemy)
    {
        if (Time.time - lastAttackTime < cooldownTime)
        {
            return;
        }

        if (!inAttackRange || enemy == null)
        {
            return;
        }

        lastAttackTime = Time.time;

        GameObject arrow = Instantiate(Resources.Load<GameObject>("Arrow"), transform.position + new Vector3(0f, 0.1f, 0f), Quaternion.identity);
        Arrow arrowComponent = arrow.GetComponent<Arrow>();
        arrowComponent.Init(this);
        Vector3 direction = (enemy.transform.position - transform.position).normalized;
        arrowComponent.Fire(direction);
        animator.Play("BowShoot");
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Unit unit = collision.GetComponent<Unit>();
        if (unit != null && unit == closestEnemy)
        {
            inAttackRange = true;
            Attack(unit);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        Unit unit = collision.GetComponent<Unit>();
        if (unit != null && unit == closestEnemy)
        {
            inAttackRange = false;
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        Unit unit = collision.GetComponent<Unit>();
        if (unit != null && unit == closestEnemy)
        {
            inAttackRange = true;
            Attack(unit);
        }
    }

}
