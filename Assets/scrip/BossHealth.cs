using UnityEngine;
using UnityEngine.UI;

public class BossHealth : MonoBehaviour
{
    [Header("Health Settings")]
    public float maxHealth = 100f;
    private float currentHealth;
    public float Health => currentHealth;

    [Header("UI Reference")]
    public Slider healthBarSlider;
    public GameObject healthBarCanvas;

    private SceneGoalManager sceneGoalManager; // Giữ tham chiếu nhưng không nhất thiết phải gán ở Start()
    private DragonBallSpawner dragonBallSpawner;

    private BossAI bossAI;
    private Animator anim;
    private Collider2D col;

    void Awake()
    {
        currentHealth = maxHealth;
    }

    void Start()
    {
        // Khởi tạo các component trên cùng Boss
        bossAI = GetComponent<BossAI>();
        anim = GetComponent<Animator>();
        col = GetComponent<Collider2D>();
        dragonBallSpawner = GetComponent<DragonBallSpawner>();

        // Tùy chọn: Thử tìm Manager ở đây nếu nó CÓ THỂ tồn tại sớm
        sceneGoalManager = FindAnyObjectByType<SceneGoalManager>();

        // ... (Logic khởi tạo thanh máu giữ nguyên)
        if (healthBarSlider != null)
        {
            healthBarSlider.maxValue = maxHealth;
            healthBarSlider.value = currentHealth;
        }

        if (healthBarCanvas != null)
            healthBarCanvas.SetActive(true);
    }

    // 💥 Nhận sát thương
    public void TakeDamage(float damageAmount)
    {
        // 🚨 DEBUG: Nhận sát thương
        Debug.Log($"[BOSS HIT] Nhận sát thương: {damageAmount}. Máu hiện tại (trước): {currentHealth}");

        // KIỂM TRA LỖI SÁT THƯƠNG = 0
        if (damageAmount <= 0f)
        {
            // Debug.LogError("LỖI CỐT LÕI: Sát thương = 0. Kiểm tra lại Player.cs và Animation Event!");
            return; // Chặn code trừ máu nếu sát thương bằng 0
        }

        if (currentHealth <= 0) return; // Đã chết thì bỏ qua

        currentHealth -= damageAmount;

        // 🚨 DEBUG: Máu còn lại
        Debug.Log($"[BOSS HIT] Máu sau khi trừ: {currentHealth}");

        // Cập nhật thanh máu
        if (healthBarSlider != null)
            healthBarSlider.value = currentHealth;

        // Kiểm tra chết
        if (currentHealth <= 0)
            Die();
    }

    // 💀 Khi Boss chết
    void Die()
    {
        currentHealth = 0;
        Debug.Log($"[BOSS DIE] {gameObject.name} đã bị tiêu diệt! Kích hoạt sự kiện kết thúc.");

        // Tắt AI và Collider
        if (bossAI != null) bossAI.enabled = false;
        if (col != null) col.enabled = false;

        // Ẩn thanh máu
        if (healthBarCanvas != null) healthBarCanvas.SetActive(false);

        // ------------------------------------------------------------------
        // === LOGIC MỚI: RƠI NGỌC RỒNG VÀ HIỆN CỔNG ===

        // 1. RƠI NGỌC RỒNG
        if (dragonBallSpawner != null)
        {
            dragonBallSpawner.SpawnDragonBall();
        }

        // 2. HIỆN CỔNG (Mục tiêu hoàn thành)
        // BƯỚC MỚI: Nếu chưa tìm thấy ở Start(), tìm lại ngay lúc này.
        Debug.Log($"[BOSS DIE] {gameObject.name} đã bị tiêu diệt! Kích hoạt sự kiện kết thúc.");

        SceneGoalManager sceneGoalManager = SceneGoalManager.Instance;

        if (sceneGoalManager != null)
        {
            sceneGoalManager.OnBossDefeated();
        }
        else
        {
            Debug.LogError("[CRITICAL ERROR] SceneGoalManager KHÔNG THỂ tìm thấy. Cổng sẽ KHÔNG hiện ra!");
        }

        Destroy(gameObject, 1.5f);
        // ------------------------------------------------------------------

        // Phát animation chết
        if (anim != null)
            anim.SetTrigger("Death");
    }

    // 🎬 Gọi ở cuối animation Death
    public void DestroyAfterDeath()
    {
        Debug.Log($"{gameObject.name} đã chết và sẽ nằm lại trên sân (không bị xóa).");
    }
}
