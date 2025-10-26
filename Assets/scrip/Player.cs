using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour
{
    // --- Stats & Configuration ---
    [Header("Kamehameha")]
    public GameObject kamehamehaPrefab;
    public Transform kamehamehaFirePoint;
    public float kamehamehaOffset = 0.5f;

    // Khai báo mặc định là 50f/70f
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

    [Header("Components & State")]
    private Rigidbody2D rb;
    private Animator anim;
    private Collider2D playerCollider;
    private bool isGrounded = true;
    private bool isSSJ = false;
    private bool isCastingSkill = false;
    private bool isDead = false;
    private float horizontalInput;

    void Awake()
    {
        // ⚡ Gán cứng giá trị mặc định để khắc phục lỗi tham chiếu/serialization.
        baseKamehamehaDamage = 50f;
        ssjKamehamehaDamage = 70f;

        Debug.Log($"[PLAYER AWAKE] Sát thương base đã được gán cứng: {baseKamehamehaDamage}");
    }

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        playerCollider = GetComponent<Collider2D>();

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

        isCastingSkill = true;
        currentKi -= kiCostKamehameha;

        // BẮT BUỘC: Đặt Animation Event SpawnKamehameha() trong cả 2 Animation "Chưởng" và "sjChưởng"
        if (isSSJ)
        {
            anim.SetTrigger("sjChưởng");
        }
        else
        {
            anim.SetTrigger("Chưởng");
        }
    }

    // ✅ HÀM GỌI TỪ ANIMATION EVENT (Đã loại bỏ tham số gây lỗi)
    public void SpawnKamehameha() // <-- Đã xóa (float damageOverride = -1f)
    {
        if (kamehamehaPrefab == null || kamehamehaFirePoint == null)
        {
            Debug.LogError("Lỗi: Không gán Prefab Kamehameha hoặc FirePoint!");
            return;
        }

        // Tính toán sát thương CHỈ dựa trên biến đã gán cứng
        float finalDamage = isSSJ ? ssjKamehamehaDamage : baseKamehamehaDamage;

        // 🚨 DEBUG: XÁC NHẬN SÁT THƯƠNG
        Debug.Log($"[PLAYER FIRE] Sát thương Kamehameha tính được: {finalDamage}");

        // BƯỚC 1: Tính toán hướng và vị trí BÙ TRỪ (Offset)
        float gokuDirectionX = Mathf.Sign(transform.localScale.x);

        // Tạo vị trí khởi tạo mới = FirePoint + Offset theo hướng Goku
        Vector3 spawnPosition = kamehamehaFirePoint.position;
        // Dịch chuyển Kamehameha ra khỏi cơ thể Player
        spawnPosition.x += gokuDirectionX * kamehamehaOffset;

        // BƯỚC 2: Instantiate Kamehameha
        GameObject kamehameha = Instantiate(kamehamehaPrefab, spawnPosition, kamehamehaFirePoint.rotation);

        // BƯỚC 3: Gán Sát thương
        KamehamehaEffect effectScript = kamehameha.GetComponent<KamehamehaEffect>();
        if (effectScript != null)
        {
            effectScript.SetDamage(finalDamage); // Truyền float
        }
        else
        {
            Debug.LogError("Lỗi: Không tìm thấy script KamehamehaEffect trên Prefab Kamehameha! Đã bị xóa hoặc đổi tên.");
        }

        // BƯỚC 4: Lật hướng Kamehameha
        Vector3 currentScale = kamehameha.transform.localScale;
        currentScale.x = Mathf.Abs(currentScale.x) * gokuDirectionX;
        kamehameha.transform.localScale = currentScale;
    }

    // ✅ HÀM GỌI TỪ ANIMATION EVENT (Kết thúc Skill)
    public void EndSkillCast()
    {
        isCastingSkill = false;
    }

    public void TakeDamage(int damage)
    {
        if (currentHealth <= 0) return;

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
        isDead = true;
        anim.SetTrigger("Death");

        if (rb != null) rb.linearVelocity = Vector2.zero;
        if (rb != null) rb.simulated = false;
        if (playerCollider != null) playerCollider.enabled = false;

        enabled = false;
        StartCoroutine(GameOverDelay());
    }

    IEnumerator GameOverDelay()
    {
        yield return new WaitForSeconds(3f);
        Destroy(gameObject);
    }

    void TransformToggle()
    {
        if (!isSSJ && currentKi > 0)
        {
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
    }

    private void OnDrawGizmosSelected()
    {
        if (meleeAttackPoint == null) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(meleeAttackPoint.position, meleeRange);
    }
}