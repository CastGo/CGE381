using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI_3 : MonoBehaviour
{
    [Header("Movement (Up/Down)")]
    public float speed = 2f;                 // หน่วย/วินาที

    [Header("Ground Check")]
    public GameObject topPoint;              // จุดบน (GameObject)
    public GameObject bottomPoint;           // จุดล่าง (GameObject)
    public float circleRadius = 0.12f;       // รัศมีตรวจ/วาดกิซโม่
    public LayerMask groundLayer;            // กำหนด Ground layer ที่จะตรวจ

    private Rigidbody2D rb;
    private int dir = 1;                     // 1 = ขึ้น, -1 = ลง

    [Header("Combat/HP")]
    public int damage = 1;
    public int maxLife = 3;
    public int currentLife;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        currentLife = maxLife;

        if (rb)
        {
            rb.gravityScale = 0f;                                 // ไม่โดนแรงโน้มถ่วง
            rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        }
    }

    void FixedUpdate()
    {
        if (!rb) return;

        // เคลื่อนที่แนวตั้ง (ห้ามคูณ deltaTime กับ velocity)
        rb.velocity = new Vector2(0f, dir * speed);

        // อ่านตำแหน่งกรอบบน/ล่างจาก GameObject (กันสลับโดยใช้ Min/Max)
        float y = transform.position.y;
        float a = topPoint ? topPoint.transform.position.y : y;
        float b = bottomPoint ? bottomPoint.transform.position.y : y;
        float topY = Mathf.Max(a, b);
        float botY = Mathf.Min(a, b);

        // ตรวจชน Ground ที่จุดบน/ล่าง (เหมือน EnemyAI_2)
        bool hitTop = topPoint
            ? Physics2D.OverlapCircle(topPoint.transform.position, circleRadius, groundLayer)
            : false;
        bool hitBot = bottomPoint
            ? Physics2D.OverlapCircle(bottomPoint.transform.position, circleRadius, groundLayer)
            : false;

        // เงื่อนไขกลับทิศ:
        // - ถ้ากำลัง "ขึ้น" แล้วชนบน หรือเกิน topY -> ลง
        if (dir > 0 && (hitTop || y >= topY)) dir = -1;

        // - ถ้ากำลัง "ลง" แล้วชนล่าง หรือเลย botY -> ขึ้น
        if (dir < 0 && (hitBot || y <= botY)) dir = 1;
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
        if (currentLife <= 0) Destroy(gameObject);
    }

    private void OnDrawGizmos()
    {
        // วาดวงกลมและเส้นเชื่อมเหมือน EnemyAI_2
        Gizmos.color = Color.cyan;
        if (topPoint)
            Gizmos.DrawWireSphere(topPoint.transform.position, circleRadius);
        if (bottomPoint)
            Gizmos.DrawWireSphere(bottomPoint.transform.position, circleRadius);
        if (topPoint && bottomPoint)
            Gizmos.DrawLine(bottomPoint.transform.position, topPoint.transform.position);
    }
}
