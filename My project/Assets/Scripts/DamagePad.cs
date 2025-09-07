using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamagePad : MonoBehaviour
{
    [Header("Buff Settings")]
    public int buffAmount = 1;         // เพิ่มดาเมจเท่าไร
    public bool singleUse = true;      // ใช้ครั้งเดียวไหม

    [Header("Visuals")]
    public Sprite idleSprite;          // ปกติ
    public Sprite usedSprite;          // หลังใช้แล้ว

    private SpriteRenderer sr;
    private bool playerInside = false;
    private bool used = false;
    private PlayerMovement pm;         // อ้างถึง PlayerMovement เพื่อแก้ดาเมจ

    void Awake()
    {
        var col = GetComponent<Collider2D>();
        col.isTrigger = true;

        sr = GetComponent<SpriteRenderer>();
        if (sr != null && idleSprite != null) sr.sprite = idleSprite;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        pm = other.GetComponent<PlayerMovement>();
        if (pm == null && other.attachedRigidbody != null)
            pm = other.attachedRigidbody.GetComponent<PlayerMovement>();

        playerInside = (pm != null);
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        playerInside = false;
        pm = null;
    }

    void Update()
    {
        if (used || !playerInside || pm == null) return;

        // ผู้เล่นกดย่อ
        if (Input.GetKeyDown(KeyCode.S))
        {
            pm.attackDamage += buffAmount;   // เพิ่มดาเมจถาวร

            if (sr != null && usedSprite != null) sr.sprite = usedSprite;

            if (singleUse)
            {
                GetComponent<Collider2D>().enabled = false; // กันใช้ซ้ำ
                used = true;
            }
        }
    }
}
