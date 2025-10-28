using UnityEngine;
using UnityEngine.SceneManagement; // BẮT BUỘC phải có
using TMPro; // Giữ lại phòng khi cần

public class GameManager : MonoBehaviour
{
    // --- Singleton (Đã có, giữ nguyên) ---
    public static GameManager instance;

    // --- BIẾN ĐÃ XÓA ---
    // public GameObject gameOverScreen; // <-- DÒNG NÀY ĐÃ BỊ XÓA (đây là nguyên nhân lỗi)

    // --- BIẾN QUẢN LÝ MỚI ---
    // Tham chiếu đến PlayerUI, sẽ được TỰ ĐỘNG TÌM trong map mới
    [HideInInspector] // Ẩn đi vì không cần kéo thả thủ công nữa
    public PlayerUI playerUI;

    private int dragonBallCount = 0; // Đã có, giữ nguyên

    // --- HÀM AWAKE (Đã có, giữ nguyên) ---
    // (Logic Singleton và DontDestroyOnLoad)
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // --- HÀM START() CŨ ĐÃ ĐƯỢC THAY THẾ BẰNG 3 HÀM NÀY ---
    // (Đây là logic mới để xử lý việc qua màn)

    void OnEnable()
    {
        // Đăng ký "lắng nghe" sự kiện mỗi khi tải một scene mới
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        // Hủy đăng ký khi GameManager bị tắt
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    // Hàm này sẽ tự động chạy MỖI KHI một scene mới được tải xong
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // 1. Tự động tìm PlayerUI trong scene MỚI
        playerUI = FindObjectOfType<PlayerUI>();

        if (playerUI != null)
        {
            // 2. Yêu cầu PlayerUI cập nhật số ngọc rồng [cite: 22]
            playerUI.UpdateDragonBallUI(dragonBallCount);

            // 3. Yêu cầu PlayerUI ẩn màn hình Game Over (để bắt đầu màn)
            playerUI.HideGameOverUI();
        }
        else
        {
            // Báo lỗi nếu không tìm thấy UI
            Debug.LogError("GameManager không tìm thấy PlayerUI trong scene: " + scene.name);
        }

        // 4. Đảm bảo game luôn chạy khi vào scene mới
        Time.timeScale = 1f;
    }

    // --- HÀM GAME OVER (ĐÃ SỬA) ---
    // Hàm này được Player gọi khi chết
    public void ShowGameOver()
    {
        // Code mới: Ra lệnh cho PlayerUI (của map hiện tại) làm việc
        if (playerUI != null)
        {
            playerUI.ShowGameOverUI();
        }
        else
        {
            Debug.LogError("Không tìm thấy PlayerUI để hiển thị Game Over!");
        }

        // Dừng thời gian (đóng băng game) - Giữ nguyên
        Time.timeScale = 0f;
    }

    // --- HÀM RESTART (Đã có, giữ nguyên) ---
    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    // --- HÀM THU THẬP NGỌC RỒNG (Đã có, giữ nguyên) ---
    public void CollectDragonBall()
    {
        dragonBallCount++;
        Debug.Log("Đã thu thập! Tổng số ngọc rồng: " + dragonBallCount);

        if (playerUI != null)
        {
            playerUI.UpdateDragonBallUI(dragonBallCount);
        }

        if (dragonBallCount >= 7) // [cite: 22]
        {
            Debug.Log("Đã đủ 7 viên! Triệu hồi Thần Rồng!"); // [cite: 23, 40]
            // SceneManager.LoadScene("SummonShenronMap");
        }
    }

    public int GetDragonBallCount()
    {
        return dragonBallCount;
    }
}