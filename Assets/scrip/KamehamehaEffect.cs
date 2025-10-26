using UnityEngine;

public class KamehamehaEffect : MonoBehaviour
{
    // === CẤU HÌNH HÀNH VI ===
    [Header("Behavior")]
    public float moveSpeed = 15f;
    public float lifeTime = 4.0f;
    public float scaleUpDuration = 0.3f;
    public float holdScaleDuration = 1.0f;

    [Header("Visual")]
    public float initialScaleFactor = 0.5f;
    public float maxScaleFactor = 2.0f;
    public GameObject hitEffectPrefab;

    // === BIẾN NỘI BỘ ===
    private float damageValue = 0f;
    private float currentLifeTime = 0f;
    private Vector3 initialPrefabScale;
    private float initialDirection;

    void Awake()
    {
        initialPrefabScale = transform.localScale;
    }

    void Start()
    {
        initialDirection = Mathf.Sign(transform.localScale.x);

        // Đặt kích thước ban đầu
        transform.localScale = initialPrefabScale * initialScaleFactor;
        transform.localScale = new Vector3(transform.localScale.x * initialDirection, transform.localScale.y, transform.localScale.z);

        // Tự hủy sau tổng thời gian sống (lifeTime)
        Destroy(gameObject, lifeTime);
    }

    void Update()
    {
        currentLifeTime += Time.deltaTime;

        // 1. Di chuyển liên tục
        transform.position += new Vector3(initialDirection, 0, 0) * moveSpeed * Time.deltaTime;

        // 2. Logic Phóng To & Giữ Kích Thước (Giữ nguyên)
        float targetScale;

        if (currentLifeTime < scaleUpDuration)
        {
            float progress = currentLifeTime / scaleUpDuration;
            targetScale = Mathf.Lerp(initialScaleFactor, maxScaleFactor, progress);
        }
        else if (currentLifeTime < scaleUpDuration + holdScaleDuration)
        {
            targetScale = maxScaleFactor;
        }
        else
        {
            float fadeProgress = (currentLifeTime - (scaleUpDuration + holdScaleDuration)) /
                                 (lifeTime - (scaleUpDuration + holdScaleDuration));
            targetScale = Mathf.Lerp(maxScaleFactor, initialScaleFactor, fadeProgress);
        }

        transform.localScale = initialPrefabScale * targetScale;
        transform.localScale = new Vector3(transform.localScale.x * initialDirection, transform.localScale.y, transform.localScale.z);
    }

    // ----------------------------------------------------------------------
    // LOGIC SÁT THƯƠNG VÀ XUYÊN THẤU
    // ----------------------------------------------------------------------

    public void SetDamage(float damage)
    {
        damageValue = damage;
        // 🚨 XÁC NHẬN SÁT THƯƠNG ĐÃ ĐƯỢC GÁN
        Debug.Log($"[KAMEHA] Sát thương đã được gán: {damageValue}");
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // 🚨 XÁC NHẬN VA CHẠM
        Debug.Log($"[KAMEHA] Va chạm với: {other.gameObject.name} (Tag: {other.tag})");

        // 0. BỎ QUA VA CHẠM VỚI PLAYER (người bắn)
        if (other.CompareTag("Player"))
        {
            return;
        }

        // 1. VA CHẠM VỚI BOSS (Gây sát thương & XUYÊN THẤU)
        if (other.CompareTag("Boss"))
        {
            BossHealth boss = other.GetComponent<BossHealth>();

            if (boss != null)
            {
                // 🚨 XÁC NHẬN GÂY SÁT THƯƠNG
                Debug.Log($"[KAMEHA HIT] Gây sát thương {damageValue} cho Boss!");
                boss.TakeDamage(damageValue);

                // Tạo hiệu ứng va chạm tại vị trí Boss
                if (hitEffectPrefab != null)
                {
                    Instantiate(hitEffectPrefab, other.transform.position, Quaternion.identity);
                }
            }

            // XUYÊN THẤU: KHÔNG GỌI Destroy
            return;
        }

        // 2. VA CHẠM VỚI ĐỊA HÌNH (XUYÊN THẤU)
        if (other.CompareTag("Ground"))
        {
            return;
        }
    }
}