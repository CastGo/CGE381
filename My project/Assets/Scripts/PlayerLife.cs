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

    [SerializeField] float damageInterval = 1f; // เวลาดีเลย์ต่อการโดนดาเมจแต่ละครั้ง
    private float damageTimer = 0f;
    // Start is called before the first frame update
    void Start()
    {
        ResetHealth();

        spriteRenderer = GetComponent<SpriteRenderer>();
        //GameController.OnReset += ResetHealth;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            EnemyAI_1 enemyAI_1 = collision.GetComponent<EnemyAI_1>();
            if (enemyAI_1)
            {
                TakeDamage(enemyAI_1.damage);
            }
            // รีเซ็ตเวลาใหม่ทันทีหลังโดน
            damageTimer = 0f;
        }
    }
    private void OnTriggerStay2D(Collider2D collision)
    {        
        if (collision.CompareTag("Enemy"))
        {
            damageTimer += Time.deltaTime;
            if (damageTimer >= damageInterval)
            {
                EnemyAI_1 enemyAI_1 = collision.GetComponent<EnemyAI_1>();
                if (enemyAI_1)
                {
                    TakeDamage(enemyAI_1.damage);
                }
                // รีเซ็ตเวลาเมื่อได้รับดาเมจ
                damageTimer = 0f;
            }
        }
    }
    
    //private void OnTriggerEnter2D(Collider2D collision)
    //{
    //    if (collision.CompareTag("Enemy"))
    //    {
    //        EnemyAI_1 enemyAI_1 = collision.GetComponent<EnemyAI_1>();
    //        if (enemyAI_1)
    //        {
    //            TakeDamage(enemyAI_1.damage);
    //        }
    //    }
    //    //Enemy enemy = collision.GetComponent<Enemy>();
    //    //if (enemy)
    //    //{
    //    //    TakeDamage(enemy.damage);
    //    //}
    //    //EnemyAI_1 enemyAI_1 = collision.GetComponent<EnemyAI_1>();
    //    //if (enemyAI_1)
    //    //{
    //    //    TakeDamage(enemyAI_1.damage);
    //    //}
    //}

    void ResetHealth()
    {
        currentLife = maxLife;
        LifeUI.SetMaxLifes(maxLife);
    }
    public void Heal(int amount)
    {
        int before = currentLife;
        currentLife = Mathf.Min(maxLife, currentLife + amount);

        if (currentLife != before)
        {
            LifeUI.UpdateLifes(currentLife);
            //StartCoroutine(FlashHeal());
        }
    }
    public void HealFull()
    {
        Heal(maxLife);
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
