using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomb : MonoBehaviour
{
    [Header("Explosion Settings")]
    [SerializeField] int damage = 1;             // ดาเมจที่จะใส่ให้ Player
    [SerializeField] float radius = 1.2f;        // รัศมีระเบิด
    [SerializeField] float hitActiveTime = 0.05f;// เปิด Trigger แค่ชั่วพริบตา (โดนครั้งเดียว)
    [SerializeField] Transform center;           // จุดกึ่งกลางระเบิด (ว่างไว้จะใช้ transform เอง)

    [Header("Auto destroy")]
    [SerializeField] float destroyDelayAfterExplode = 0.01f; // หน่วงนิดเดียวให้ทันสร้าง Trigger

    // เรียกจาก Animation Event ที่เฟรม "ระเบิด"
    public void Explode()
    {
        Vector3 pos = center ? center.position : transform.position;

        // สร้าง GameObject ชั่วคราวทำหน้าที่เป็น Damage Area
        GameObject damageGO = new GameObject("ExplosionDamage");
        damageGO.layer = gameObject.layer; // ไม่จำเป็น แต่เผื่ออยากจัด Layer เดียวกัน
        damageGO.tag = "Enemy";            // สำคัญ: ให้ PlayerLife รู้ว่าเป็นศัตรู (จะโดนดาเมจ)

        damageGO.transform.position = pos;

        // วงระเบิดเป็น Trigger
        var circle = damageGO.AddComponent<CircleCollider2D>();
        circle.isTrigger = true;
        circle.radius = radius;

        // ให้ PlayerLife หา EnemyAI_1 แล้วอ่านค่า damage ได้
        var enemy = damageGO.AddComponent<EnemyAI_1>();
        enemy.damage = damage;

        // ทำลาย Trigger นี้ในไม่กี่เฟรม เพื่อให้เกิด OnTriggerEnter2D แค่ครั้งเดียว
        Object.Destroy(damageGO, hitActiveTime);

        // ทำลายระเบิดตัวเองตามต้องการ
        StartCoroutine(DestroySelfSoon());
    }

    IEnumerator DestroySelfSoon()
    {
        yield return new WaitForSeconds(destroyDelayAfterExplode);
        Destroy(gameObject);
    }

    // เผื่อดูรัศมีใน Scene
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Vector3 pos = center ? center.position : transform.position;
        Gizmos.DrawWireSphere(pos, radius);
    }
}
