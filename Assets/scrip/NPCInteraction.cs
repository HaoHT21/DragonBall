using UnityEngine;

// Đảm bảo kiểu dữ liệu CycleContent đã được khai báo ở đâu đó (thường là trong chat.cs)

public class NPCInteraction : MonoBehaviour
{
    // === Biến Cờ và Tham chiếu Chung ===
    private bool hasInteracted = false;
    private chat dialogueManager;

    [Header("Phân loại & Collider")]
    public bool isEnemy = false;
    public Collider2D interactionCollider;

    [Header("Mảng Kịch Bản Của NPC")]
    public CycleContent[] myCycleScript;

    // THÊM: Tham chiếu đến Handler Xử lý Cái chết của Boss
    private BossHealth bossDeathHandler;

    void Start()
    {
        // ... (Logic tìm Manager giữ nguyên)
        dialogueManager = FindAnyObjectByType<chat>();
        if (dialogueManager == null) { Debug.LogError("Lỗi: Không tìm thấy script chat (DialogueManager) trong Scene."); }

        // ... (Logic kiểm tra Collider tương tác và Rigidbody giữ nguyên)
        if (interactionCollider == null) { Debug.LogWarning("NPC thiếu Collider TƯƠNG TÁC (interactionCollider). Tương tác sẽ không hoạt động."); }
        if (GetComponent<Rigidbody2D>() == null) { Debug.LogWarning("NPC thiếu Rigidbody 2D. Tương tác có thể không hoạt động."); }

        // MỚI: Chỉ tìm BossDeathHandler nếu đây là Kẻ địch
        if (isEnemy)
        {
            bossDeathHandler = GetComponent<BossHealth>();
            if (bossDeathHandler == null)
            {
                Debug.LogError("Kẻ địch (Boss) thiếu script BossHealth!");
            }
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // ... (Logic bắt đầu đối thoại giữ nguyên)
        if (other.CompareTag("Player"))
        {
            if (hasInteracted) { return; }

            if (dialogueManager != null && myCycleScript != null && myCycleScript.Length > 0)
            {
                dialogueManager.StartCycling(myCycleScript, this);
                hasInteracted = true;
            }
        }
    }

    // Hàm được Manager gọi sau khi đối thoại kết thúc
    public void BeginCombat()
    {
        if (interactionCollider != null)
        {
            // TẮT Collider tương tác để ngăn đối thoại lặp lại
            interactionCollider.enabled = false;

            if (isEnemy)
            {
                // KẺ ĐỊCH: CHỈ BẮT ĐẦU CHIẾN ĐẤU. KHÔNG GỌI GoalCompleted() Ở ĐÂY.
                Debug.Log("KẺ ĐỊCH đã hoàn thành đối thoại. Bắt đầu chiến đấu!");
                // (Thêm logic kích hoạt AI Boss ở đây)
            }
            else
            {
                // NPC (Người bạn): Kích hoạt lệnh hiện cổng ngay lập tức
                SceneGoalManager manager = FindAnyObjectByType<SceneGoalManager>();
                if (manager != null)
                {
                    manager.GoalCompleted();
                }
                Debug.Log("NPC (Người bạn) đã hoàn thành đối thoại. Cổng đã được kích hoạt.");
            }
        }
        else
        {
            Debug.LogError("Không tìm thấy Interaction Collider để tắt.");
        }
    }
}
