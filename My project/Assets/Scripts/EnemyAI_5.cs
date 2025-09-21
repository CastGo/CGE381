using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI_5 : MonoBehaviour
{
    [Header("Speed")]
    public float descendSpeed = 3f;   // ความเร็วลอยปรับระดับ Y
    public float moveSpeed = 4f;   // ความเร็ววิ่งแนวนอนหลังจัดระดับ

    [Header("Align")]
    public float yAlignTolerance = 0.05f; // ระยะถือว่าอยู่ระดับเดียวกันแล้ว

    [Header("HP / Damage From Player")]
    public int maxLife = 3;
    public int currentLife = 3;
    public int damage = 1;

    Rigidbody2D rb;
    Transform player;

    enum State { DescendToPlayerY, MoveFixedDirection }
    State state = State.DescendToPlayerY;

    float targetY;      // ระดับ Y ของผู้เล่นตอนเริ่ม
    int moveDir = 1;    // 1 = ขวา, -1 = ซ้าย (ยึดตามตำแหน่งผู้เล่นตอนเริ่ม)
    bool facingRight;   // สถานะหันหน้าปัจจุบัน

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        if (rb)
        {
            rb.gravityScale = 0f;
            rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        }

        // กำหนดค่าเริ่มจากสเกลปัจจุบัน (x>0 = หันขวา)
        facingRight = transform.localScale.x >= 0f;
    }

    void Start()
    {
        // หา Player แล้วจับข้อมูล “ตอนเริ่ม”
        var pObj = GameObject.FindGameObjectWithTag("Player");
        if (pObj) player = pObj.transform;

        targetY = player ? player.position.y : transform.position.y;

        // ทิศทางวิ่งถาวร = ตำแหน่งผู้เล่นเทียบกับตำแหน่งโดรน ตอนเริ่ม
        float dx = (player ? player.position.x : transform.position.x) - transform.position.x;
        moveDir = dx >= 0f ? 1 : -1;

        // พลิกหน้าให้ตรงกับทิศที่จะวิ่งตั้งแต่ตอนนี้
        FaceByDir(moveDir);

        currentLife = Mathf.Clamp(currentLife, 1, maxLife);
    }

    void FixedUpdate()
    {
        if (!rb) return;

        switch (state)
        {
            case State.DescendToPlayerY:
                {
                    float dy = targetY - transform.position.y;

                    if (Mathf.Abs(dy) <= yAlignTolerance)
                    {
                        // เข้าสู่โหมดวิ่งแนวนอน (คงทิศเดิม)
                        rb.velocity = new Vector2(moveDir * moveSpeed, 0f);
                        // เผื่อมีใครแก้สเกลภายนอก ให้ย้ำทิศอีกครั้ง
                        FaceByDir(moveDir);
                        state = State.MoveFixedDirection;
                    }
                    else
                    {
                        // ลอยขึ้น/ลงเข้าหา targetY
                        float vy = Mathf.Sign(dy) * descendSpeed;
                        rb.velocity = new Vector2(0f, vy);
                    }
                    break;
                }

            case State.MoveFixedDirection:
                {
                    // วิ่งแนวนอนคงทิศ (ไม่ไล่ผู้เล่นอีก)
                    rb.velocity = new Vector2(moveDir * moveSpeed, 0f);
                    // (ไม่ต้องพลิกซ้ำทุกเฟรม แต่ทำไม่เสียหาย)
                    break;
                }
        }
    }

    // === Flip helper ===
    void FaceByDir(int dir)
    {
        if (dir == 0) return;
        bool shouldFaceRight = dir > 0;
        if (shouldFaceRight == facingRight) return;

        facingRight = shouldFaceRight;
        var sc = transform.localScale;
        sc.x = Mathf.Abs(sc.x) * (facingRight ? 1f : -1f);
        transform.localScale = sc;
    }

    // รับดาเมจจากการโจมตีของผู้เล่น
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("PlayerDamage"))
        {
            var area = collision.GetComponent<AttackArea>();
            if (area) TakeDamage(area.damage);
        }
    }

    public void TakeDamage(int dmg)
    {
        currentLife -= dmg;
        if (currentLife <= 0) Destroy(gameObject);
    }

    // กิซโม่ช่วยมอง targetY
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        float y = Application.isPlaying ? targetY : transform.position.y;
        Gizmos.DrawLine(new Vector3(transform.position.x - 2f, y, 0),
                        new Vector3(transform.position.x + 2f, y, 0));
    }
}
