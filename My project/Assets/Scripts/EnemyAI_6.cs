using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI_6 : MonoBehaviour
{
    [Header("Move / Ranges")]
    public float chaseSpeed = 2.5f;
    public float fleeSpeed = 4.0f;
    public float stopRange = 6f;
    public float fleeRange = 2.5f;

    [Header("Shoot")]
    public GameObject bulletPrefab;
    public Transform firePoint;
    public float bulletSpeed = 8f;
    public float shootCooldown = 0.8f;
    public int projectileDamage = 1; // ใช้กับสคริปต์กระสุนของคุณ

    [Header("Stats")]
    public int damage = 1;
    public int maxLife = 3;
    public int currentLife;

    [Header("Animator")]
    public Animator animator;
    public string walkState = "EnemyShooterWalk";
    public string shootState = "EnemyShooterShoot";
    public float crossFade = 0.1f;

    Rigidbody2D rb;
    Transform player;

    // --- Animator cache ---
    int walkHash, shootHash, activeAnimHash = -1;

    // --- State machine ---
    enum State { Chase, Shoot, Flee }
    State state = State.Chase;

    // --- Flee once ---
    bool hasFled = false;
    int fleeDirX = 1;

    // --- Shoot cd ---
    float nextShootTime = 0f;
    bool facingRight = true;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        if (!animator) animator = GetComponent<Animator>();
        currentLife = maxLife;

        var p = GameObject.FindGameObjectWithTag("Player");
        if (p) player = p.transform;

        if (rb) rb.constraints = RigidbodyConstraints2D.FreezeRotation;

        walkHash = Animator.StringToHash(walkState);
        shootHash = Animator.StringToHash(shootState);

        SetAnim(walkHash);   // เริ่มเดิน
    }

    void Update()
    {
        if (!player || !rb) return;

        Vector2 toPlayer = player.position - transform.position;
        float dist = toPlayer.magnitude;
        int dirX = (toPlayer.x >= 0f) ? 1 : -1;

        // หันหน้า
        Face(dirX);

        switch (state)
        {
            case State.Chase:
                rb.velocity = new Vector2(dirX * chaseSpeed, rb.velocity.y);
                SetAnim(walkHash);

                if (dist <= fleeRange)
                {
                    EnterFlee(dirX);
                }
                else if (dist <= stopRange)
                {
                    rb.velocity = new Vector2(0f, rb.velocity.y);
                    SetState(State.Shoot);
                }
                break;

            case State.Shoot:
                rb.velocity = new Vector2(0f, rb.velocity.y);
                SetAnim(shootHash);
                TryShoot();

                if (dist <= fleeRange)
                    EnterFlee(dirX);
                else if (!hasFled && dist > stopRange + 0.5f) // กันแกว่งนิดหน่อย
                    SetState(State.Chase);
                break;

            case State.Flee:
                // หนี "ไปทางเดิม" ตลอด ไม่กลับตัวไล่แล้ว
                rb.velocity = new Vector2(fleeDirX * fleeSpeed, rb.velocity.y);
                SetAnim(walkHash);
                Face(fleeDirX);
                break;
        }
    }

    void EnterFlee(int dirXFacingPlayer)
    {
        hasFled = true;
        fleeDirX = -dirXFacingPlayer;   // หนีจากผู้เล่น
        SetState(State.Flee);
    }

    void SetState(State s)
    {
        state = s;
        // เซ็ตอนิเมชันเริ่มต้นของแต่ละ state
        if (s == State.Shoot) SetAnim(shootHash);
        else SetAnim(walkHash);
    }

    // ===== Shooting (ง่าย) =====
    void TryShoot()
    {
        if (Time.time < nextShootTime || bulletPrefab == null) return;

        // ยิงตามทิศที่หันอยู่ (กระสุนของคุณวิ่งตาม -transform.right)
        Vector2 dir = (transform.localScale.x >= 0f) ? Vector2.right : Vector2.left;
        Vector3 pos = firePoint ? firePoint.position : transform.position;

        var go = Instantiate(bulletPrefab, pos, Quaternion.identity);
        go.transform.right = -dir;     // ให้ -right ตรงกับทิศยิง

        // ตั้งค่ากับสคริปต์กระสุนแบบง่ายของคุณ (ต้องมี public float speed,int damage)
        var bullet = go.GetComponent<EnemyProjectile>();
        if (bullet != null)
        {
            bullet.speed = bulletSpeed;
            bullet.damage = projectileDamage;
        }

        // กันชนตัวเอง (แนะนำ)
        var myCol = GetComponent<Collider2D>();
        var bCol = go.GetComponent<Collider2D>();
        if (myCol && bCol) Physics2D.IgnoreCollision(bCol, myCol);

        nextShootTime = Time.time + shootCooldown;
        SetAnim(shootHash); // ย้ำให้เล่นคลิปยิง
    }

    // ===== Anim helper =====
    void SetAnim(int hash)
    {
        if (!animator) return;
        if (activeAnimHash == hash) return;      // ไม่สั่งซ้ำทุกเฟรม
        animator.CrossFadeInFixedTime(hash, crossFade);
        activeAnimHash = hash;
    }

    // ===== Face (flip) =====
    void Face(int dirX)
    {
        bool shouldFaceRight = dirX > 0;
        if (shouldFaceRight == facingRight) return;
        facingRight = shouldFaceRight;

        var sc = transform.localScale;
        sc.x = Mathf.Abs(sc.x) * (facingRight ? 1f : -1f);
        transform.localScale = sc;
    }

    // ===== Damage from player =====
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

    // ===== Gizmos =====
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow; Gizmos.DrawWireSphere(transform.position, stopRange);
        Gizmos.color = Color.red; Gizmos.DrawWireSphere(transform.position, fleeRange);
    }
}
