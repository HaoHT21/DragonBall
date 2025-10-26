using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneGoalManager : MonoBehaviour
{
    // === SINGLETON INSTANCE ===
    public static SceneGoalManager Instance { get; private set; }

    public enum GoalType
    {
        None,
        NPCInteraction,
        BossDefeat
    }

    [Header("Loại mục tiêu")]
    public GoalType goalType = GoalType.None;

    [Header("Cấu hình Scene")]
    public GameObject exitGate;
    public string nextSceneName;

    [Header("Trạng thái")]
    public bool isGateActive = false;
    public bool isOneTimeGoal = true;

    private bool goalCompleted = false;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    void Start()
    {
        if (exitGate != null)
        {
            exitGate.SetActive(false);
            Debug.Log("[MANAGER START] ExitGate đã ẩn.");
        }
        else
        {
            Debug.LogError("[MANAGER START] Lỗi: Exit Gate chưa được gán!");
        }
    }

    // === HÀM GỌI KHI MỤC TIÊU HOÀN THÀNH ===
    public void GoalCompleted()
    {
        if (goalCompleted && isOneTimeGoal) return;

        goalCompleted = true;
        isGateActive = true;

        Debug.Log($"[GOAL COMPLETED] {goalType} hoàn thành! Chuẩn bị mở cổng...");

        if (exitGate != null)
        {
            exitGate.SetActive(true);
            Debug.Log($"[GATE STATUS] ExitGate.SetActive(true); Trạng thái thực tế: {exitGate.activeSelf}");
        }
        else
        {
            Debug.LogError("[GOAL ERROR] ExitGate chưa được gán!");
        }
    }


    // === GỌI KHI HOÀN THÀNH NHIỆM VỤ NPC ===
    public void OnNPCInteractionComplete()
    {
        if (goalType == GoalType.NPCInteraction)
        {
            Debug.Log("[NPC GOAL] Giao tiếp với NPC hoàn tất. Mở cổng!");
            GoalCompleted();
        }
    }

    // === GỌI KHI BOSS CHẾT ===
    public void OnBossDefeated()
    {
        if (goalType == GoalType.BossDefeat)
        {
            Debug.Log("[BOSS GOAL] Boss bị hạ gục. Mở cổng!");
            GoalCompleted();
        }
    }

    // === CHUYỂN SCENE ===
    public void LoadNextScene()
    {
        if (!isGateActive) return;

        if (string.IsNullOrEmpty(nextSceneName))
        {
            Debug.LogError("[SCENE ERROR] Tên Scene rỗng!");
            return;
        }

        // Kiểm tra scene tồn tại trong Build Settings
        int index = SceneUtility.GetBuildIndexByScenePath(nextSceneName);
        if (index >= 0)
        {
            Debug.Log($"[SCENE CHANGE] Đang tải scene: {nextSceneName}");
            SceneManager.LoadScene(nextSceneName);
        }
        else
        {
            Debug.LogError($"[SCENE ERROR] Scene '{nextSceneName}' chưa thêm vào Build Settings!");
        }
    }

    // === VA CHẠM VỚI CỔNG ===
    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log($"[TRIGGER DEBUG] Va chạm: {other.gameObject.name}");

        if (!isGateActive)
        {
            Debug.Log("[TRIGGER DEBUG] Cổng chưa Active, bỏ qua va chạm.");
            return;
        }

        if (other.CompareTag("Player"))
        {
            Debug.Log("Player chạm cổng. Đang chuyển Scene...");
            LoadNextScene();
        }
    }
}
