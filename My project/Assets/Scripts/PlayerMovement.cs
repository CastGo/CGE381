using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public Transform areaPoint;
    public GameObject attackArea;

    float horizontalInput;
    float moveSpeed = 5f;
    bool isFacingRight = false;
    float jumpPower = 7f;
    bool isGrounded = false;
    bool isCrouching = false;
    bool isAttacking = false;

    public int attackDamage = 1;
    Rigidbody2D rb;
    Animator animator;

    public CoinManager cm;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        horizontalInput = Input.GetAxis("Horizontal");

        FlipSprite();

        if (Input.GetButtonDown("Jump") && isGrounded && !isCrouching && !isAttacking)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpPower);
            isGrounded = false;
            animator.SetBool("isJumping", !isGrounded);
        }

        if(Input.GetKeyDown(KeyCode.S) && !isAttacking)
        {
            isCrouching = true;
            animator.SetBool("isCrouching", true);
        }
        if (Input.GetKeyUp(KeyCode.S))
        {
            isCrouching = false;
            animator.SetBool("isCrouching", false);
        }

        if (Input.GetKeyDown(KeyCode.J) && !isAttacking)
        {

            if(!isGrounded)
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

    private void FixedUpdate()
    {
        if (isCrouching || isAttacking)
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
    }

    public void FinishAttack()
    {
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
