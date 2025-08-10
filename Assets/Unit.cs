using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class Unit : MonoBehaviour
{
    public float health = 100f;

    public TMP_Text body;

    public Weapon myWeapon;
    public string weaponID;
    public string unitBody = "3";

    public Collider2D hitbox;

    public Rigidbody2D rb;

    internal float movementTimer = 0.2f;
    private Vector3 movementIntensity;
    private bool inCombat = false;


    private bool dragging = false;
    private Vector3 offset;


    public void SetEnemy()
    {
        body.rectTransform.localScale = new Vector3(-1, 1, 1);
    }


    public void SetInCombat(bool value)
    {
        inCombat = value;
    }


    public void TakeDamage(Weapon attacker)
    {
        if (!inCombat)
        {
            return;
        }

        bool crit = Random.value < attacker.critChance;
        float damage = attacker.attackPower + (crit ? attacker.attackPower : 0);
        health -= damage;

        Vector3 pushDirection = (transform.position - attacker.transform.position).normalized;
        rb.velocity = pushDirection * damage;

        if (health <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Debug.Log("Unit " + weaponID + " has died.");

        CombatManager.instance.RemoveUnitFromCombat(this);

        gameObject.SetActive(false);
    }

    internal void InitWeapon(GameObject weapon, string weaponID)
    {
        myWeapon = weapon.GetComponent<Weapon>();
        myWeapon.transform.SetParent(transform);
        myWeapon.transform.localPosition = weapon.GetComponent<Weapon>().getLocalTransform();
        myWeapon.gameObject.SetActive(true);
        health = weapon.GetComponent<Weapon>().holderHealth;
        this.weaponID = weaponID;
    }

    internal void InitBody(string bodyText)
    {
        unitBody = bodyText;
    }

    // Start is called before the first frame update
    void Start()
    {
        GetComponentInChildren<TextMeshPro>().text = unitBody;
    }
    // Update is called once per frame
    void Update()
    {
        if (!inCombat)
        {
            return;
        }
        if (movementTimer > 0.2f)
        {
            movementIntensity = myWeapon.GetMovement();
            movementTimer = 0f;
        }
        else
        {
            movementTimer += Time.deltaTime;
        }

        rb.velocity = Vector3.Lerp(rb.velocity, movementIntensity, 0.5f);

        if (rb.velocity.x < 0f && movementIntensity.x < 0f)
        {
            transform.localScale = new Vector3(-1, 1, 1);
        }
        else if (rb.velocity.x > 0f && movementIntensity.x > 0f)
        {
            transform.localScale = new Vector3(1, 1, 1);
        }

    }

    private void OnMouseDown()
    {
        if (inCombat)
        {
            return;
        }
        offset = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
        dragging = true;
        offset.z = 0;
    }

    private void OnMouseUp()
    {
        if (inCombat)
        {
            return;
        }
        dragging = false;
        Database.UpdateUnitPosition(weaponID, transform.position);
    }

    private void OnMouseDrag()
    {
        if (inCombat)
        {
            return;
        }
        if (dragging)
        {
            Vector3 newPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition) - offset;
            newPosition.x = Mathf.Clamp(newPosition.x, -4f, 4f);
            newPosition.y = Mathf.Clamp(newPosition.y, -4f, 4f);
            newPosition.z = 0;
            transform.position = newPosition;
        }
    }


    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (SceneManager.GetActiveScene().name == "CombatScene")
        {
            return;
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (SceneManager.GetActiveScene().name == "CombatScene")
        {
            return;
        }
        else
        {
            Vector3 newPosition = transform.position;
            Database.UpdateUnitPosition(weaponID, newPosition);
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (SceneManager.GetActiveScene().name == "CombatScene")
        {
            return;
        }
        else
        {
            Vector3 newPosition = transform.position;
            Database.UpdateUnitPosition(weaponID, newPosition);
        }
    }

    internal void Heal(float amount)
    {
        health += amount;
        if (health > myWeapon.holderHealth)
        {
            health = myWeapon.holderHealth;
        }
        Debug.Log("Unit " + weaponID + " healed. Current health: " + health);
    }
}
