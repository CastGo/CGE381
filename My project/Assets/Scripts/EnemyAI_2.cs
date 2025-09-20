using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI_2 : MonoBehaviour
{
    [Header("Movement")]
    public float speed = 3f;         // หน่วย/วินาที
    public float jumpForce = 7f;     // แรงกระโดด (Impulse)
    public bool facingRight = true;  // true=มองขวา

    [Header("Wall Checks")]
    public GameObject groundCheck1;  // จุดบน (Top)
    public GameObject groundCheck2;  // จุดล่าง (Bottom)
    public float circleRadius = 0.2f;
    public LayerMask groundLayer;

    [Header("Stats")]
    public int damage = 1;
    public int maxLife = 3;
    public int currentLife;

    [Header("Jump Control")]
    public float jumpCooldown = 0.2f;
    private float nextJumpTime = 0f;

    private Rigidbody2D rb;
    private Transform player;

    // สถานะชนของเฟรมก่อนหน้า (ใช้ตรวจจับ “เริ่มชน”)
    private bool prevTop = false;
    private bool prevBot = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        currentLife = maxLife;

        // หา player และหันไปทางผู้เล่น
        var pObj = GameObject.FindGameObjectWithTag("Player");
        if (pObj) player = pObj.transform;

        if (player != null)
        {
            float dir = Mathf.Sign(player.position.x - transform.position.x);
            if (dir == 0) dir = facingRight ? 1f : -1f;
            bool shouldFaceRight = dir > 0;
            if (shouldFaceRight != facingRight) Flip();
        }
    }

    void Update()
    {
        // เดินคงที่ตามทิศที่หันหน้า
        float dir = facingRight ? 1f : -1f;
        rb.velocity = new Vector2(dir * speed, rb.velocity.y);

        // อ่านสถานะชนปัจจุบันจาก 2 จุด
        bool topHit = false, botHit = false;

        if (groundCheck1 != null)
        {
            topHit = Physics2D.OverlapCircle(groundCheck1.transform.position, circleRadius, groundLayer);
        }
        if (groundCheck2 != null)
        {
            botHit = Physics2D.OverlapCircle(groundCheck2.transform.position, circleRadius, groundLayer);
        }

        // ตรวจ "เริ่มชน" (edge) ของแต่ละจุด
        bool topJustEntered = topHit && !prevTop;
        bool botJustEntered = botHit && !prevBot;

        // เงื่อนไข:
        // 1) ถ้า Top เริ่มชน และ Bottom ยังไม่ชน => Flip (กลับหลัง)
        if (topHit && botHit)
        {
            Flip();
        }
        // 2) ถ้า Bottom เริ่มชน และ Top ยังไม่ชน => กระโดด
        else if (botJustEntered && !topHit)
        {
            TryJump();
        }

        // เก็บสถานะไว้เทียบรอบถัดไป
        prevTop = topHit;
        prevBot = botHit;
    }

    private void TryJump()
    {
        if (Time.time < nextJumpTime) return;
        // reset y เพื่อให้แรงกระโดดสม่ำเสมอ
        rb.velocity = new Vector2(rb.velocity.x, 0f);
        rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        nextJumpTime = Time.time + jumpCooldown;
    }

    private void Flip()
    {
        facingRight = !facingRight;
        Vector3 sc = transform.localScale;
        sc.x *= -1f;
        transform.localScale = sc;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("PlayerDamage"))
        {
            var attackArea = collision.GetComponent<AttackArea>();
            if (attackArea) TakeDamage(attackArea.damage);
        }
    }

    public void TakeDamage(int dmg)
    {
        currentLife -= dmg;
        if (currentLife <= 0) Die();
    }

    private void Die()
    {
        Destroy(gameObject);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        if (groundCheck1 != null)
            Gizmos.DrawWireSphere(groundCheck1.transform.position, circleRadius);
        if (groundCheck2 != null)
            Gizmos.DrawWireSphere(groundCheck2.transform.position, circleRadius);
    }
}
