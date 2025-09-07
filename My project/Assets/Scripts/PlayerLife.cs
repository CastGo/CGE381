using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLife : MonoBehaviour
{
    public int maxLife = 8;
    private int currentLife;

    public LifeUI LifeUI;

    private SpriteRenderer spriteRenderer;

    public static event Action OnPlayedDied;

    // Start is called before the first frame update
    void Start()
    {
        ResetHealth();

        //spriteRenderer = GetComponent<SpriteRenderer>();
        //GameController.OnReset += ResetHealth;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            Enemy enemy = collision.GetComponent<Enemy>();
            if (enemy != null)
            {
                TakeDamage(enemy.damage);
            }
        }
    }

    void ResetHealth()
    {
        currentLife = maxLife;
        LifeUI.SetMaxLifes(maxLife);
    }

    private void TakeDamage(int damage)
    {
        currentLife -= damage;
        LifeUI.UpdateLifes(currentLife);

        StartCoroutine(FlashBlink());

        if (currentLife <= 0)
        {
            //player dead! -- call game over, animation, etc
            OnPlayedDied.Invoke();
        }
    }

    private IEnumerator FlashBlink()
    {
        // ดึง SpriteRenderer ถ้ายังไม่ได้อ้างอิง
        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();

        // จำนวนครั้งที่จะกระพริบ
        int blinkCount = 5;

        for (int i = 0; i < blinkCount; i++)
        {
            spriteRenderer.enabled = false;  // ซ่อน sprite
            yield return new WaitForSeconds(0.1f);

            spriteRenderer.enabled = true;   // แสดง sprite
            yield return new WaitForSeconds(0.1f);
        }
    }
}
