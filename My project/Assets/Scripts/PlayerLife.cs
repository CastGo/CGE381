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
    private float nextDamageTime = 0f;
    // Start is called before the first frame update
    void Start()
    {
        ResetHealth();

        spriteRenderer = GetComponent<SpriteRenderer>();
        //GameController.OnReset += ResetHealth;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy") && Time.time >= nextDamageTime)
        {
            EnemyAI_1 enemyAI_1 = collision.GetComponent<EnemyAI_1>();
            EnemyAI_2 enemyAI_2 = collision.GetComponent<EnemyAI_2>();
            EnemyAI_3 enemyAI_3 = collision.GetComponent<EnemyAI_3>();

            if (enemyAI_1)
            {
                TakeDamage(enemyAI_1.damage);
            }
            else if (enemyAI_2)
            {
                TakeDamage(enemyAI_2.damage);
            }
            else if (enemyAI_3)
            {
                TakeDamage(enemyAI_3.damage);
            }

            nextDamageTime = Time.time + damageInterval; // เริ่มคูลดาวน์
        }
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            if (collision.CompareTag("Enemy") && Time.time >= nextDamageTime)
            {
                EnemyAI_1 enemyAI_1 = collision.GetComponent<EnemyAI_1>();
                EnemyAI_2 enemyAI_2 = collision.GetComponent<EnemyAI_2>();
                EnemyAI_3 enemyAI_3 = collision.GetComponent<EnemyAI_3>();

                if (enemyAI_1)
                {
                    TakeDamage(enemyAI_1.damage);
                }
                else if (enemyAI_2)
                {
                    TakeDamage(enemyAI_2.damage);
                }
                else if (enemyAI_3)
                {
                    TakeDamage(enemyAI_3.damage);
                }

                nextDamageTime = Time.time + damageInterval; // ต่อคูลดาวน์อีก 1 วิ
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
            OnPlayedDied?.Invoke();
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
