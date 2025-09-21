using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI_4 : MonoBehaviour
{
    [Header("Animator / State names (ต้องตรงกับใน Animator)")]
    public Animator animator;                   // ไม่ใส่จะ GetComponent<Animator>() อัตโนมัติ
    public string idleState = "EnemyCellingIdle";
    public string openState = "EnemyCellingOpen";
    public string closeState = "EnemyCellingClose";
    [Tooltip("เวลาครอสเฟดตอนเปลี่ยน State")]
    public float crossFade = 0.1f;

    [Header("Timing")]
    [Tooltip("เริ่มเกมแล้วรอกี่วิ ก่อนเปิดฝา (Anim2) ครั้งแรก")]
    public float delayBeforeFirstOpen = 3f;
    [Tooltip("หลังปิดฝา (Anim3) และกลับไป Idle แล้วรอกี่วิ ก่อนเปิดรอบถัดไป")]
    public float delayBetweenLoops = 7f;
    public float holdOpenBeforeClose = 1f;

    [Header("Spawn")]
    [Tooltip("Prefab ที่จะเกิด (เลือกมา 1 อัน)")]
    public GameObject spawnPrefab;
    [Tooltip("จุดเกิด (ไม่ใส่จะใช้ตำแหน่ง Enemy)")]
    public Transform spawnPoint;

    int idleHash, openHash, closeHash;

    [Header("Combat/HP")]
    public int damage = 1;
    public int maxLife = 3;
    public int currentLife;

    void Awake()
    {
        if (!animator) animator = GetComponent<Animator>();
        idleHash = Animator.StringToHash(idleState);
        openHash = Animator.StringToHash(openState);
        closeHash = Animator.StringToHash(closeState);
    }

    void OnEnable()
    {
        StartCoroutine(MainLoop());
    }

    IEnumerator MainLoop()
    {
        // เริ่มที่ Idle แน่นอน
        animator.Play(idleHash, 0, 0f);

        while (true)
        {
            // 1) รอ 3 วินาที
            yield return new WaitForSeconds(delayBeforeFirstOpen);

            // 2) เล่น Open แล้วรอจนจบ
            yield return PlayAndWait(openHash);

            // 3) Spawn 1 อัน
            SpawnOnce();

            // 4) ค้างไว้ก่อน 1 วิ (หรือค่าที่ตั้งไว้)
            yield return new WaitForSeconds(holdOpenBeforeClose);

            // 5) เล่น Close แล้วรอจนจบ
            yield return PlayAndWait(closeHash);

            // 6) กลับไป Idle แล้วรอ 7 วิ
            yield return CrossFadeAndWaitEnter(idleHash);
            yield return new WaitForSeconds(delayBetweenLoops);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("PlayerDamage"))
        {
            var attackArea = collision.GetComponent<AttackArea>();
            if (attackArea) TakeDamage(attackArea.damage);
        }
    }

    public void TakeDamage(int dmg)
    {
        currentLife -= dmg;
        if (currentLife <= 0) Destroy(gameObject);
    }
    IEnumerator PlayAndWait(int stateHash)
    {
        animator.CrossFadeInFixedTime(stateHash, crossFade);
        yield return WaitEnter(stateHash);
        yield return WaitUntilFinished(stateHash);
    }

    IEnumerator CrossFadeAndWaitEnter(int stateHash)
    {
        animator.CrossFadeInFixedTime(stateHash, crossFade);
        yield return WaitEnter(stateHash);
    }

    IEnumerator WaitEnter(int stateHash)
    {
        // รอจน "เข้า" state นั้นจริง ๆ
        yield return null;
        while (true)
        {
            var st = animator.GetCurrentAnimatorStateInfo(0);
            if (st.shortNameHash == stateHash || st.fullPathHash == stateHash) break;
            yield return null;
        }
    }

    IEnumerator WaitUntilFinished(int stateHash)
    {
        // รอจนเล่น state นั้นครบ (ไม่อยู่ช่วงทรานซิชัน และ normalizedTime >= 1)
        while (true)
        {
            var st = animator.GetCurrentAnimatorStateInfo(0);
            bool inState = (st.shortNameHash == stateHash || st.fullPathHash == stateHash);
            if (inState && !animator.IsInTransition(0) && st.normalizedTime >= 0.999f)
                break;
            yield return null;
        }
    }

    void SpawnOnce()
    {
        if (!spawnPrefab) return;
        Vector3 pos = spawnPoint ? spawnPoint.position : transform.position;
        Instantiate(spawnPrefab, pos, Quaternion.identity);
    }

    // ถ้าอยากสั่งเกิดจาก Animation Event ในคลิป Open ก็เรียกเมธอดนี้แทนได้
    public void SpawnFromAnimEvent() => SpawnOnce();
}
