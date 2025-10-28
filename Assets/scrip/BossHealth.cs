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

    // <-- THÊM MỚI (1/4) -->
    [Header("Audio")]
    public AudioClip takeDamageSound;
    public AudioClip deathSound;

    private SceneGoalManager sceneGoalManager;
    private DragonBallSpawner dragonBallSpawner;

    private BossAI bossAI;
    private Animator anim;
    private Collider2D col;
    private AudioSource audioSource; // <-- THÊM MỚI (2/4) -->

    void Awake()
    {
        currentHealth = maxHealth;
    }

    void Start()
    {
        bossAI = GetComponent<BossAI>();
        anim = GetComponent<Animator>();
        col = GetComponent<Collider2D>();
        dragonBallSpawner = GetComponent<DragonBallSpawner>();
        audioSource = GetComponent<AudioSource>(); // <-- THÊM MỚI (3/4) -->

        sceneGoalManager = FindAnyObjectByType<SceneGoalManager>();

        if (healthBarSlider != null)
        {
            healthBarSlider.maxValue = maxHealth;
            healthBarSlider.value = currentHealth;
        }

        if (healthBarCanvas != null)
            healthBarCanvas.SetActive(true);
    }

    public void TakeDamage(float damageAmount)
    {
        Debug.Log($"[BOSS HIT] Nhận sát thương: {damageAmount}. Máu hiện tại (trước): {currentHealth}");

        if (damageAmount <= 0f)
        {
            return;
        }

        if (currentHealth <= 0) return;

        currentHealth -= damageAmount;

        // <-- THÊM MỚI (ÂM THANH BỊ ĐÁNH) (4/4) -->
        if (audioSource != null && takeDamageSound != null)
        {
            audioSource.PlayOneShot(takeDamageSound);
        }

        Debug.Log($"[BOSS HIT] Máu sau khi trừ: {currentHealth}");

        if (healthBarSlider != null)
            healthBarSlider.value = currentHealth;

        if (currentHealth <= 0)
            Die();
    }

    void Die()
    {
        // <-- THÊM MỚI (ÂM THANH CHẾT) -->
        if (audioSource != null && deathSound != null)
        {
            audioSource.PlayOneShot(deathSound);
        }

        currentHealth = 0;
        Debug.Log($"[BOSS DIE] {gameObject.name} đã bị tiêu diệt! Kích hoạt sự kiện kết thúc.");

        if (bossAI != null) bossAI.enabled = false;
        if (col != null) col.enabled = false;

        if (healthBarCanvas != null) healthBarCanvas.SetActive(false);

        if (dragonBallSpawner != null)
        {
            dragonBallSpawner.SpawnDragonBall();
        }

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

        if (anim != null)
            anim.SetTrigger("Death");
    }

    public void DestroyAfterDeath()
    {
        Debug.Log($"{gameObject.name} đã chết và sẽ nằm lại trên sân (không bị xóa).");
    }
}
