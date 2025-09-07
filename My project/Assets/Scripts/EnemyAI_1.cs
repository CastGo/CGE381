using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI_1 : MonoBehaviour
{
    public float speed;
    public float circleRadius;
    private Rigidbody2D EnemyRB;
    public GameObject groundCheck;
    public LayerMask groundLayer;

    public int damage = 1;

    public int maxLife = 3;
    public int currentLife;
    private SpriteRenderer spriteRenderer;
    private Color ogColor;

    public bool facingRight;
    public bool isGrounded;
    // Start is called before the first frame update
    void Start()
    {
        EnemyRB = GetComponent<Rigidbody2D>();
        currentLife = maxLife;
        //ogColor = spriteRenderer.color;
    }

    // Update is called once per frame
    void Update()
    {
        EnemyRB.velocity = Vector2.right * speed * Time.deltaTime;
        isGrounded = Physics2D.OverlapCircle(groundCheck.transform.position, circleRadius, groundLayer);
        if(!isGrounded && facingRight)
        {
            Flip();
        }
        else if(!isGrounded && !facingRight)
        {
            Flip();
        }

        void Flip()
        {
            facingRight = !facingRight;
            Vector3 Scaler = transform.localScale;
            Scaler.x *= -1;
            transform.localScale = Scaler;
            speed *= -1;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(groundCheck.transform.position, circleRadius);
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
        //foreach (LootItem lootItem in lootTable)
        //{
        //    if (Random.Range(0f, 100f) <= lootItem.dropChance)
        //    {
        //        InstantiateLoot(lootItem.itemPrefab);
        //    }
        //    break;
        //}

        Destroy(gameObject);
    }

    //void InstantiateLoot(GameObject loot)
    //{
    //    if (loot)
    //    {
    //        GameObject droppedLoot = Instantiate(loot, transform.position, Quaternion.identity);

    //        droppedLoot.GetComponent<SpriteRenderer>().color = Color.red;
    //    }
    //}
}
