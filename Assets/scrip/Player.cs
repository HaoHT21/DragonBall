using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour
{
    // --- Stats & Configuration ---
    [Header("Kamehameha")]
    public GameObject kamehamehaPrefab;
    public Transform kamehamehaFirePoint;
    public float kamehamehaOffset = 0.5f;
    public float baseKamehamehaDamage = 50f;
    public float ssjKamehamehaDamage = 70f;

    [Header("Stats")]
    public float moveSpeed = 5f;
    public float jumpForce = 8f;
    public int maxHealth = 100;

    // Quản lý Máu bằng Property
    private int _currentHealth;
    public int currentHealth
    {
        get { return _currentHealth; }
        set
        {
            _currentHealth = Mathf.Clamp(value, 0, maxHealth);
            // ...
        }
    }

    [Header("Ki System - USP")]
    public float maxKi = 100f;
    public float kiPerMeleeHit = 10f;
    public float kiCostKamehameha = 50f;
    public float kiDrainSSJ = 5f;

    // Quản lý Ki bằng Property
    private float _currentKi;
    public float currentKi
    {
        get { return _currentKi; }
        set
        {
            _currentKi = Mathf.Clamp(value, 0, maxKi);
            // ...
        }
    }

    [Header("Melee Combat")]
    public float meleeDamage = 15f;
    public float meleeRange = 0.5f;
    public Transform meleeAttackPoint;

    [Header("Effects")]
    public GameObject hitEffectPrefab;

    [Header("Audio")]
    public AudioClip meleeSound;
    public AudioClip transformSound;
    public AudioClip kamehamehaAimSound;  // Thay thế cho kamehamehaSound
    public AudioClip kamehamehaFireSound; // Âm thanh "HA"
    public AudioClip deathSound;
    public AudioClip takeDamageSound; // (Bạn đã yêu cầu thêm cái này)

    [Header("Components & State")]
    private Rigidbody2D rb;
    private Animator anim;
    private Collider2D playerCollider;
    private AudioSource audioSource;
    private GameManager gameManager; // <-- Code GameManager
    private bool isGrounded = true;
    private bool isSSJ = false;
    private bool isCastingSkill = false;
    private bool isDead = false;
    private float horizontalInput;

    void Awake()
    {
        baseKamehamehaDamage = 50f;
        ssjKamehamehaDamage = 70f;
        Debug.Log($"[PLAYER AWAKE] Sát thương base đã được gán cứng: {baseKamehamehaDamage}");
    }

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        playerCollider = GetComponent<Collider2D>();
        audioSource = GetComponent<AudioSource>();
        gameManager = FindObjectOfType<GameManager>(); // <-- Code GameManager

        currentHealth = maxHealth;
        currentKi = 50f;
    }

    void Update()
    {
        if (isDead) return;
        HandleMovement();
        HandleInput();

        if (isSSJ)
        {
            currentKi -= kiDrainSSJ * Time.deltaTime;
            if (currentKi <= 0)
            {
                TransformToggle();
            }
        }
    }

    void HandleMovement()
    {
        if (isCastingSkill)
        {
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
            anim.SetBool("IsRunning", false);
            return;
        }

        horizontalInput = Input.GetAxisRaw("Horizontal");
        rb.linearVelocity = new Vector2(horizontalInput * moveSpeed, rb.linearVelocity.y);

        anim.SetBool("IsRunning", horizontalInput != 0 && isGrounded);

        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            isGrounded = false;
            anim.SetBool("IsGrounded", false);
        }

        if (horizontalInput != 0)
        {
            transform.localScale = new Vector3(Mathf.Sign(horizontalInput), transform.localScale.y, transform.localScale.z);
        }
    }

    void HandleInput()
    {
        if (isCastingSkill) return;

        if (Input.GetKeyDown(KeyCode.J))
        {
            anim.SetTrigger("Atack");
        }

        if (Input.GetKeyDown(KeyCode.I))
        {
            anim.SetTrigger("Đá");
        }

        if (Input.GetKeyDown(KeyCode.L) && currentKi >= kiCostKamehameha)
        {
            UseKamehameha();
        }

        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            TransformToggle();
        }
    }


    // ✅ HÀM GỌI TỪ ANIMATION EVENT: Gây sát thương Melee
    public void DealMeleeDamage()
    {
        if (audioSource != null && meleeSound != null)
        {
            audioSource.PlayOneShot(meleeSound);
        }

        if (meleeAttackPoint == null) return;

        float finalDamage = isSSJ ? meleeDamage * 1.5f : meleeDamage;
        if (!isSSJ) currentKi += kiPerMeleeHit;

        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(meleeAttackPoint.position, meleeRange);

        foreach (Collider2D enemy in hitEnemies)
        {
            if (enemy.CompareTag("Boss"))
            {
                BossHealth boss = enemy.GetComponent<BossHealth>();
                if (boss != null)
                {
                    boss.TakeDamage(finalDamage);
                    if (hitEffectPrefab != null)
                    {
                        Instantiate(hitEffectPrefab, enemy.transform.position, Quaternion.identity);
                    }
                    return;
                }
            }
        }
    }

    // --- SKILL COMBAT ---

    void UseKamehameha()
    {
        if (isCastingSkill || currentKi < kiCostKamehameha) return;

        if (audioSource != null && kamehamehaAimSound != null)
        {
            audioSource.PlayOneShot(kamehamehaAimSound);
        }

        isCastingSkill = true;
        currentKi -= kiCostKamehameha;

        if (isSSJ)
        {
            anim.SetTrigger("sjChưởng");
        }
        else
        {
            anim.SetTrigger("Chưởng");
        }
    }

    // ✅ HÀM GỌI TỪ ANIMATION EVENT
    public void SpawnKamehameha()
    {
        if (audioSource != null && kamehamehaFireSound != null)
        {
            audioSource.PlayOneShot(kamehamehaFireSound);
        }

        if (kamehamehaPrefab == null || kamehamehaFirePoint == null)
        {
            Debug.LogError("Lỗi: Không gán Prefab Kamehameha hoặc FirePoint!");
            return;
        }

        float finalDamage = isSSJ ? ssjKamehamehaDamage : baseKamehamehaDamage;
        Debug.Log($"[PLAYER FIRE] Sát thương Kamehameha tính được: {finalDamage}");
        float gokuDirectionX = Mathf.Sign(transform.localScale.x);
        Vector3 spawnPosition = kamehamehaFirePoint.position;
        spawnPosition.x += gokuDirectionX * kamehamehaOffset;

        GameObject kamehameha = Instantiate(kamehamehaPrefab, spawnPosition, kamehamehaFirePoint.rotation);
        KamehamehaEffect effectScript = kamehameha.GetComponent<KamehamehaEffect>();
        if (effectScript != null)
        {
            effectScript.SetDamage(finalDamage);
        }
        else
        {
            Debug.LogError("Lỗi: Không tìm thấy script KamehamehaEffect trên Prefab Kamehameha! Đã bị xóa hoặc đổi tên.");
        }

        Vector3 currentScale = kamehameha.transform.localScale;
        currentScale.x = Mathf.Abs(currentScale.x) * gokuDirectionX;
        kamehameha.transform.localScale = currentScale;
    }

    public void EndSkillCast()
    {
        isCastingSkill = false;
    }

    public void TakeDamage(int damage)
    {
        if (currentHealth <= 0) return;

        if (audioSource != null && takeDamageSound != null)
        {
            audioSource.PlayOneShot(takeDamageSound);
        }

        currentHealth -= damage;
        anim.SetTrigger("BịHạ");

        if (hitEffectPrefab != null)
        {
            Instantiate(hitEffectPrefab, transform.position, Quaternion.identity);
        }

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        if (audioSource != null && deathSound != null)
        {
            audioSource.PlayOneShot(deathSound);
        }

        isDead = true;
        anim.SetTrigger("Death");

        if (rb != null) rb.linearVelocity = Vector2.zero;
        if (rb != null) rb.simulated = false;
        if (playerCollider != null) playerCollider.enabled = false;

        enabled = false;

        // <-- ĐÃ SỬA (3/3) -->
        if (gameManager != null)
        {
            gameManager.ShowGameOver();
        }
    }

    // (Hàm GameOverDelay() đã được xóa)

    void TransformToggle()
    {
        if (!isSSJ && currentKi > 0)
        {
            if (audioSource != null && transformSound != null)
            {
                audioSource.PlayOneShot(transformSound);
            }

            isSSJ = true;
            anim.SetLayerWeight(1, 1f);
        }
        else
        {
            isSSJ = false;
            anim.SetLayerWeight(1, 0f);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
            anim.SetBool("IsGrounded", true);
        }

        // (Code hội thoại của bạn nằm ở đây sẽ an toàn)
    }

    private void OnDrawGizmosSelected()
    {
        if (meleeAttackPoint == null) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(meleeAttackPoint.position, meleeRange);
    }
}