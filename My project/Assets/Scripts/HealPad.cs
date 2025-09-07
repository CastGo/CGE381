using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealPad : MonoBehaviour
{
    [Header("Sprites")]
    public Sprite idleSprite;    // ก่อนใช้
    public Sprite usedSprite;    // หลังใช้ (เช่น เทา/จาง)

    private SpriteRenderer sr;
    private bool playerInside = false;
    private bool used = false;
    private PlayerLife playerLife;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        var col = GetComponent<Collider2D>();
        col.isTrigger = true;
        if (idleSprite) sr.sprite = idleSprite;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        playerInside = true;

        // รองรับกรณีชนคอลลิเดอร์ "ลูก" ของ Player
        var rb = other.attachedRigidbody;
        var root = rb ? rb.gameObject : other.gameObject;
        playerLife = root.GetComponent<PlayerLife>();
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        playerInside = false;
        playerLife = null;
    }

    void Update()
    {
        if (used || !playerInside || playerLife == null) return;

        // กด "ย่อ" เพื่อฮีล (คุณใช้ S สำหรับย่อ)
        if (Input.GetKeyDown(KeyCode.S))
        {
            playerLife.HealFull();     // ฮีลเต็มหลอด
            used = true;               // ใช้ไปแล้ว: ล็อกไม่ให้เรียกอีก

            // เปลี่ยนสไปรต์และปิดการทำงานถาวร
            if (usedSprite) sr.sprite = usedSprite;
            GetComponent<Collider2D>().enabled = false;  // ไม่ให้ชนอีก
            enabled = false;                              // ปิดสคริปต์ (เลือกได้ว่าจะ Destroy)
            // Destroy(gameObject);  // ถ้าต้องการให้แผ่นหายไปเลย
        }
    }
}
