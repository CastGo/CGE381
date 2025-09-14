using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlphaBetaGround : MonoBehaviour
{
    
    [Header("Sprite Settings")]
    public Sprite spriteA;   // รูปที่ 1
    public Sprite spriteB;   // รูปที่ 2
    private SpriteRenderer spriteRenderer;
    private bool usingSpriteA = true; // เริ่มต้นด้วยรูป A

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (spriteRenderer != null)
        {
            if (Random.value < 0.5f && spriteA != null)
            {
                spriteRenderer.sprite = spriteA;
                usingSpriteA = true;
            }
            else if (spriteB != null)
            {
                spriteRenderer.sprite = spriteB;
                usingSpriteA = false;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("PlayerDamage"))
        {
            SwapSprite();
        }

        if (collision.CompareTag("Player"))
        {
            Destroy(gameObject);
        }
    }
    void SwapSprite()
    {
        if (spriteRenderer == null) return;

        if (usingSpriteA && spriteB != null)
        {
            spriteRenderer.sprite = spriteB;
            usingSpriteA = false;
        }
        else if (!usingSpriteA && spriteA != null)
        {
            spriteRenderer.sprite = spriteA;
            usingSpriteA = true;
        }
    }
}
