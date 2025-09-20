using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomb : MonoBehaviour
{
    [Header("Explosion Settings")]
    [SerializeField] int damage = 1;
    [SerializeField] float radius = 1.2f;
    [SerializeField] float hitActiveTime = 0.1f;   // เปิดฮิตบ็อกซ์ชั่วคราว
    [SerializeField] Transform center;             // ถ้าเว้นไว้จะใช้ transform ตัวเอง

    [Header("Auto destroy")]
    [SerializeField] float destroyDelayAfterExplode = 0.01f;

    // เรียกจาก Animation Event ตอน “ตูม!”
    public void Explode()
    {
        Vector3 pos = center ? center.position : transform.position;

        // สร้าง GO ชั่วคราวเป็นวงดาเมจ
        GameObject damageGO = new GameObject("ExplosionDamage");
        damageGO.tag = "Enemy";                 // สำคัญ: ให้ PlayerLife จัดเป็นศัตรู
        damageGO.layer = gameObject.layer;
        damageGO.transform.position = pos;

        // ต้องมี Rigidbody2D (Kinematic) เพื่อให้ OnTrigger ทำงานชัวร์และไม่ error
        var rb2d = damageGO.AddComponent<Rigidbody2D>();
        rb2d.bodyType = RigidbodyType2D.Kinematic;
        rb2d.gravityScale = 0f;

        // วงกลม Trigger (เปิดใช้ช้ากว่า 1 fixed frame เพื่อ force OnTriggerEnter2D)
        var circle = damageGO.AddComponent<CircleCollider2D>();
        circle.isTrigger = true;
        circle.radius = radius;
        circle.enabled = false;

        // ใส่ EnemyAI_1 แค่เพื่อให้ PlayerLife อ่าน damage ได้ (ปิดสคริปต์กันพฤติกรรมอื่น ๆ)
        var enemy = damageGO.AddComponent<EnemyAI_1>();
        enemy.damage = damage;
        enemy.enabled = false; // ปิด Update/พฤติกรรมอื่น แต่ GetComponent ยังเจออยู่

        StartCoroutine(ActivateAndAutoDestroy(damageGO, circle));

        // ทำลาย Prefab ระเบิด
        StartCoroutine(DestroySelfSoon());
    }

    IEnumerator ActivateAndAutoDestroy(GameObject go, Collider2D col)
    {
        // รอให้ผ่าน 1 FixedUpdate เพื่อให้เกิด OnTriggerEnter2D แน่ ๆ
        yield return new WaitForFixedUpdate();
        if (col) col.enabled = true;

        yield return new WaitForSeconds(hitActiveTime);
        if (go) Destroy(go);
    }

    IEnumerator DestroySelfSoon()
    {
        yield return new WaitForSeconds(destroyDelayAfterExplode);
        if (this) Destroy(gameObject);
    }

    // ดูรัศมีใน Scene
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Vector3 pos = center ? center.position : transform.position;
        Gizmos.DrawWireSphere(pos, radius);
    }
}
