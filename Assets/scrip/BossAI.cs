using UnityEngine;
using System.Collections;

public class BossAI : MonoBehaviour
{
    // === CẤU HÌNH ===
    [Header("Cấu hình")]
    public Transform player;
    public float detectionRange = 10f;
    public float attackRange = 2f;
    public float moveSpeed = 3f;
    public float patrolDistance = 5f;
    public float waitAfterAttack = 1.5f;
    public float attackDamage = 15f;
    public float kiBlastSpeed = 15f;
    public float kiBlastTime = 0.5f; // THÊM: Thời gian chạy Animation Ki Blast (Đồng bộ với Animation)

    [Header("Combat Points")]
    public float bossMeleeRange = 0.7f;
    public Transform bossAttackPoint;

    [Header("Ki Blast")]
    public GameObject kiBlastPrefab;
    public Transform kiFirePoint;

    [Header("Hiệu ứng")]
    public GameObject hitEffectPrefab;

    // === THÀNH PHẦN & TRẠNG THÁI ===
    private Rigidbody2D rb;
    private Animator anim;
    private Vector3 startPos;
    private bool movingRight = true;
    private bool isBusy = false;
    private BossHealth bossHealth;
    private bool isDead = false;

    public bool IsBusy => isBusy;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        bossHealth = GetComponent<BossHealth>();
        startPos = transform.position;

        if (player == null)
        {
            Player playerComponent = FindAnyObjectByType<Player>();
            if (playerComponent != null)
                player = playerComponent.transform;
        }

        // KIỂM TRA THAM CHIẾU QUAN TRỌNG (Giúp bạn biết vì sao Boss Crash/Không đánh)
        if (hitEffectPrefab == null)
        {
            Debug.LogError("HIT EFFECT PREFAB BỊ THIẾU! Boss sẽ crash khi tấn công trúng Player. Vui lòng gán trong Inspector.");
        }
    }

    void Update()
    {
        if (isDead) return;
        if (player == null || bossHealth == null) return;

        if (bossHealth.Health <= 0 && !isDead)
        {
            StartCoroutine(HandleDeath());
            return;
        }

        float distance = Vector3.Distance(transform.position, player.position);

        if (isBusy)
        {
            if (rb != null) rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
            return;
        }

        if (distance <= detectionRange)
            HandleCombat(distance);
        else
            Patrol();
    }

    // --- LOGIC AI ---

    void Patrol()
    {
        anim.SetBool("isRunning", true);
        anim.SetBool("isAttacking", false);

        float leftLimit = startPos.x - patrolDistance;
        float rightLimit = startPos.x + patrolDistance;

        if (movingRight)
        {
            if (transform.position.x >= rightLimit)
            {
                Flip();
            }
        }
        else // movingLeft
        {
            if (transform.position.x <= leftLimit)
            {
                Flip();
            }
        }

        float moveDir = movingRight ? 1f : -1f;
        if (rb != null)
            rb.linearVelocity = new Vector2(moveDir * moveSpeed, rb.linearVelocity.y);
    }

    void HandleCombat(float distance)
    {
        FlipTowards(player.position.x);

        if (distance > attackRange)
        {
            anim.SetBool("isRunning", true);
            float dirX = Mathf.Sign(player.position.x - transform.position.x);
            if (rb != null)
                rb.linearVelocity = new Vector2(dirX * moveSpeed, rb.linearVelocity.y);
        }
        else
        {
            anim.SetBool("isRunning", false);
            if (rb != null) rb.linearVelocity = Vector2.zero;
            if (!isBusy)
                StartCoroutine(AttackSequence());
        }
    }

    IEnumerator AttackSequence()
    {
        isBusy = true;

        // Tấn công vật lý (Melee)
        anim.SetBool("isAttacking", true);
        yield return new WaitForSeconds(1.0f);
        anim.SetBool("isAttacking", false);

        yield return new WaitForSeconds(waitAfterAttack);

        // Tấn công Ki Blast
        anim.SetBool("isKi", true);
        // THAY ĐỔI: Sử dụng kiBlastTime để đồng bộ tốt hơn
        yield return new WaitForSeconds(kiBlastTime);
        anim.SetBool("isKi", false);

        // THAY ĐỔI: Thời gian cooldown sau khi hoàn thành Animation Ki
        yield return new WaitForSeconds(1.0f);
        isBusy = false;
    }

    IEnumerator HandleDeath()
    {
        isDead = true;
        anim.SetTrigger("Death");
        if (rb != null) rb.linearVelocity = Vector2.zero;
        if (rb != null) rb.simulated = false;

        yield return null;
    }

    // ✅ HÀM GỌI TỪ ANIMATION EVENT: Tấn công vật lý
    public void DealMeleeDamage()
    {
        if (bossAttackPoint == null) return;

        Collider2D[] hitTargets = Physics2D.OverlapCircleAll(bossAttackPoint.position, bossMeleeRange);
        foreach (Collider2D target in hitTargets)
        {
            if (target.CompareTag("Player"))
            {
                Player playerScript = target.GetComponent<Player>();
                if (playerScript != null)
                {
                    playerScript.TakeDamage((int)attackDamage);
                    // Tạo hiệu ứng (Gây lỗi Null Reference nếu Prefab trống!)
                    if (hitEffectPrefab != null)
                        Instantiate(hitEffectPrefab, target.transform.position, Quaternion.identity);
                }
            }
        }
    }

    // ✅ HÀM GỌI TỪ ANIMATION EVENT: Phóng Ki Blast
    public void SpawnKiBlast()
    {
        // 1. Kiểm tra tham chiếu
        if (kiBlastPrefab == null || kiFirePoint == null)
        {
            Debug.LogError("Ki Blast Prefab hoặc Fire Point bị thiếu!");
            return;
        }

        // 2. Tạo viên đạn
        GameObject kiBlast = Instantiate(kiBlastPrefab, kiFirePoint.position, kiFirePoint.rotation);

        // 3. Lấy Rigidbody2D của viên đạn để di chuyển
        Rigidbody2D kiRb = kiBlast.GetComponent<Rigidbody2D>();
        float bossDirectionX = transform.localScale.x;

        if (kiRb != null)
        {
            // Tính toán hướng bắn dựa trên hướng nhìn của Boss
            Vector2 shootDirection = (bossDirectionX > 0) ? Vector2.right : Vector2.left;
            kiRb.linearVelocity = shootDirection * kiBlastSpeed;
        }

        // 4. Lật viên đạn để nó quay đúng hướng
        Vector3 kiScale = kiBlast.transform.localScale;
        kiScale.x = Mathf.Abs(kiScale.x) * Mathf.Sign(bossDirectionX);
        kiBlast.transform.localScale = kiScale;

        // 5. Thiết lập thuộc tính của viên đạn
        BossKiBlast kiScript = kiBlast.GetComponent<BossKiBlast>();
        if (kiScript != null)
        {
            // Vui lòng kiểm tra BossKiBlast.cs có biến public 'hitEffectPrefab' không
            kiScript.hitEffectPrefab = hitEffectPrefab;
            // Nếu BossKiBlast có biến public damage: kiScript.damage = (int)attackDamage * 2;
        }
    }

    // --- HÀM HỖ TRỢ ---
    void Flip()
    {
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
        movingRight = !movingRight;
    }

    void FlipTowards(float targetX)
    {
        Vector3 scale = transform.localScale;
        if ((targetX > transform.position.x && scale.x < 0) ||
            (targetX < transform.position.x && scale.x > 0))
        {
            scale.x *= -1;
        }
        transform.localScale = scale;
    }

    private void OnDrawGizmosSelected()
    {
        // ... (Không thay đổi)
    }
}