using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public Transform areaPoint;
    private Vector3 defaultAreaPointPos;
    public GameObject attackArea;

    float horizontalInput;
    float moveSpeed = 5f;
    bool isFacingRight = false;
    float jumpPower = 7f;
    bool isGrounded = false;
    bool isCrouching = false;
    bool isClimbing = false;
    bool isClimbAttacking = false;
    bool isAttacking = false;

    public int attackDamage = 1;
    Rigidbody2D rb;
    Animator animator;

    public CoinManager cm;

    [SerializeField] float climbJumpY = 8f;   // แรงกระโดดแนวตั้งตอนกด Jump ระหว่างเกาะ
    float normalGravity;                      // เก็บค่า gravityScale ปกติ
    bool isInClimbZone = false;               // อยู่ทับผนังที่เกาะได้ (Tag=ClimbingWall) ไหม

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        defaultAreaPointPos = areaPoint.localPosition;
        normalGravity = rb.gravityScale <= 0f ? 1f : rb.gravityScale;
    }

    // Update is called once per frame
    void Update()
    {
        horizontalInput = Input.GetAxis("Horizontal");

        FlipSprite();

        if (!isClimbing && Input.GetKeyDown(KeyCode.W) && isInClimbZone && !isGrounded && !isAttacking)
        {
            StartClimb();
        }

        if (isClimbing && !isClimbAttacking && Input.GetButtonDown("Jump"))
        {
            rb.velocity = new Vector2(0f, climbJumpY);
            StopClimb();

            isGrounded = false;
            animator.SetBool("isJumping", true);
        }

        if (Input.GetButtonDown("Jump") && isGrounded && !isCrouching && !isAttacking)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpPower);
            isGrounded = false;
            animator.SetBool("isJumping", !isGrounded);
        }

        if (Input.GetKeyDown(KeyCode.S) && !isAttacking && !isClimbing)
        {
            isCrouching = true;
            animator.SetBool("isCrouching", true);
            areaPoint.localPosition = defaultAreaPointPos + new Vector3(0, -0.4f, 0);
        }
        if (Input.GetKeyUp(KeyCode.S))
        {
            isCrouching = false;
            animator.SetBool("isCrouching", false);
            areaPoint.localPosition = defaultAreaPointPos;
        }

        if (Input.GetKeyDown(KeyCode.J) && !isAttacking)
        {
            if (isClimbing)
            {
                // --- Climb Attack ---
                isAttacking = true;
                isClimbAttacking = true;

                // ปิดสถานะเกาะระหว่างออกท่า
                isClimbing = false;
                animator.SetBool("isClimbing", false);

                // เปิดอนิเมชันท่าเกาะต่อย
                animator.SetBool("isClimbAttacking", true);

                // ล็อกนิ่งเหมือนตอนเกาะ
                rb.gravityScale = 0f;
                rb.velocity = Vector2.zero;

                // สร้าง hitbox
                var attack = Instantiate(attackArea, areaPoint.position, areaPoint.rotation);
                var area = attack.GetComponent<AttackArea>();
                if (area != null) area.damage = attackDamage;
            }
            else
            {
                if (!isGrounded)
                {
                    animator.SetBool("isJumping", false);
                    animator.SetBool("isJumpAttacking", true);
                }
                else if (isCrouching)
                {
                    isAttacking = true;
                    animator.SetBool("isCrouching", false);
                    animator.SetBool("isCrouchAttacking", true);
                }
                else
                {

                    isAttacking = true;
                    animator.SetBool("isAttacking", true);
                }

                var attack = Instantiate(attackArea, areaPoint.position, areaPoint.rotation);


                var area = attack.GetComponent<AttackArea>();
                if (area != null)
                    area.damage = attackDamage;
            }
        }
    }

    private void FixedUpdate()
    {
        if (isClimbing)
        {
            rb.velocity = Vector2.zero;   // ล็อกนิ่งทั้ง X/Y ตอนเกาะ
        }
        else if (isCrouching || isAttacking)
        {
            rb.velocity = new Vector2(0, rb.velocity.y);
        }
        else
        {
            rb.velocity = new Vector2(horizontalInput * moveSpeed, rb.velocity.y);
        }

        animator.SetFloat("xVelocity", Math.Abs(rb.velocity.x));
        animator.SetFloat("yVelocity", rb.velocity.y);
    }

    void FlipSprite()
    {
        if (isFacingRight && horizontalInput < 0f || !isFacingRight && horizontalInput > 0f)
        {
            isFacingRight = !isFacingRight;
            Vector3 ls = transform.localScale;
            ls.x *= -1f;
            transform.localScale = ls;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            isGrounded = true;  // อยู่บนพื้น
            animator.SetBool("isJumping", !isGrounded); // false -> ไม่เล่น jump
        }
        if (collision.gameObject.CompareTag("ClimbingWall"))
        {
            isInClimbZone = true;
        }
        if (collision.gameObject.CompareTag("Coin50"))
        {
            Destroy(collision.gameObject);
            cm.coinCount += 50;
        }
        if (collision.gameObject.CompareTag("Coin200"))
        {
            Destroy(collision.gameObject);
            cm.coinCount += 200;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            isGrounded = false; // ลอยจากพื้น
            animator.SetBool("isJumping", !isGrounded); // true -> เล่น jump
        }
        if (collision.gameObject.CompareTag("ClimbingWall"))
        {
            isInClimbZone = false;
            if (isClimbing) StopClimb();
        }
    }
    void StartClimb()
    {
        isClimbing = true;
        rb.gravityScale = 0f;        // เกาะแล้วไม่ตก
        rb.velocity = Vector2.zero;  // หยุดทันที

        // ถ้ามีพารามิเตอร์ใน Animator
        animator.SetBool("isClimbing", true);
        animator.SetBool("isJumping", false);
    }

    void StopClimb()
    {
        isClimbing = false;
        rb.gravityScale = normalGravity; // คืนแรงโน้มถ่วง
        animator.SetBool("isClimbing", false);

        // ถ้ายังไม่แตะพื้น ให้กลับไปอนิเมชันกระโดด
        if (!isGrounded) animator.SetBool("isJumping", true);
    }
    public void FinishAttack()
    {
        // --- ถ้าเป็นการโจมตีขณะเกาะ ---
        if (isClimbAttacking)
        {
            isClimbAttacking = false;
            isAttacking = false;
            animator.SetBool("isClimbAttacking", false);

            // กลับไปเกาะแบบตรง ๆ
            isClimbing = true;
            rb.gravityScale = 0f;
            rb.velocity = Vector2.zero;
            animator.SetBool("isClimbing", true);
            animator.SetBool("isJumping", false);

            return;
        }

        isAttacking = false;
        animator.SetBool("isAttacking", false);
        animator.SetBool("isCrouchAttacking", false);
        animator.SetBool("isJumpAttacking", false);

        if (!isGrounded)
        {
            animator.SetBool("isJumping", true);
        }
        if (Input.GetKey(KeyCode.S))
        {
            isCrouching = true;
            animator.SetBool("isCrouching", true);
        }
    }
}
