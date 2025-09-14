using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyObject : MonoBehaviour
{
    private Rigidbody2D EnemyRB;

    public int maxLife = 3;
    public int currentLife;
    private SpriteRenderer spriteRenderer;
    private Color ogColor;

    //LootTable
    [Header("Loot")]
    public List<LootItem> lootTable = new List<LootItem>();


    void Start()
    {
        EnemyRB = GetComponent<Rigidbody2D>();
        currentLife = maxLife;
        //ogColor = spriteRenderer.color;
    }

    void Update()
    {
        
    }

    //private IEnumerator FlashWhite()
    //{
    //    spriteRenderer.color = Color.white;
    //    yield return new WaitForSeconds(0.2f);
    //    spriteRenderer.color = ogColor;
    //}
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("PlayerDamage"))
        {
            AttackArea attackArea = collision.GetComponent<AttackArea>();
            if (attackArea)
            {
                TakeDamage(attackArea.damage);
            }
            //// รีเซ็ตเวลาใหม่ทันทีหลังโดน
            //damageTimer = 0f;
        }
    }
    public void TakeDamage(int damage)
    {
        currentLife -= damage;
        //StartCoroutine(FlashWhite());
        if (currentLife <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        //Go around loottable
        //Spawn item
        foreach (LootItem lootItem in lootTable)
        {
            if (Random.Range(0f, 100f) <= lootItem.dropChance)
            {
                InstantiateLoot(lootItem.itemPrefab);
            }
            break;
        }

        Destroy(gameObject);
    }

    void InstantiateLoot(GameObject loot)
    { 
        if (loot)
        {
            GameObject droppedLoot = Instantiate(loot, transform.position, Quaternion.identity);

            //droppedLoot.GetComponent<SpriteRenderer>().color = Color.red;
            //Rigidbody2D rb = droppedLoot.GetComponent<Rigidbody2D>();
            //if (rb == null)
            //{
            //    rb = droppedLoot.AddComponent<Rigidbody2D>();
            //}

            //rb.freezeRotation = true;
            //Vector2 force = new Vector2(Random.Range(-2f, 2f), Random.Range(3f, 5f));
            //rb.AddForce(force, ForceMode2D.Impulse);
        }
    }
}
