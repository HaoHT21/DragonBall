using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerUI : MonoBehaviour
{
    [Header("UI References")]
    public Slider healthSlider;
    public Slider kiSlider;
    public TextMeshProUGUI dragonBallCountText;
    public Animator dragonBallIconAnimator;
    public GameObject gameOverScreen;

    private Player playerScript; // Giữ nguyên
    private bool slidersInitialized = false; // Giữ nguyên

    void Start()
    {
        // Chỉ cần ẩn Game Over UI khi bắt đầu
        HideGameOverUI();
    }

    void Update()
    {
        // --- LOGIC TÌM PLAYER (Giữ nguyên) ---
        if (playerScript == null)
        {
            playerScript = FindObjectOfType<Player>();
            if (playerScript == null)
            {
                return;
            }
            slidersInitialized = false;
        }

        // --- KHỞI TẠO SLIDERS (Giữ nguyên) ---
        if (!slidersInitialized)
        {
            InitializeSliders();
            slidersInitialized = true;
        }

        // --- CẬP NHẬT UI (Giữ nguyên) ---
        UpdateHealthUI(playerScript.currentHealth);
        UpdateKiUI(playerScript.currentKi);
    }

    // --- HÀM KHỞI TẠO (Giữ nguyên) ---
    void InitializeSliders()
    {
        Debug.Log("Đã tìm thấy Player! Khởi tạo thanh Máu/Ki.");
        if (healthSlider != null)
        {
            healthSlider.maxValue = playerScript.maxHealth;
            healthSlider.value = playerScript.currentHealth;
        }
        if (kiSlider != null)
        {
            kiSlider.maxValue = playerScript.maxKi;
            kiSlider.value = playerScript.currentKi;
        }
    }

    // --- CÁC HÀM CẬP NHẬT (Giữ nguyên) ---
    public void UpdateHealthUI(int newHealth)
    {
        if (healthSlider != null)
        {
            healthSlider.value = newHealth;
        }
    }

    public void UpdateKiUI(float newKi)
    {
        if (kiSlider != null)
        {
            kiSlider.value = newKi;
        }
    }

    public void UpdateDragonBallUI(int count)
    {
        if (dragonBallCountText != null)
        {
            dragonBallCountText.text = "x " + count.ToString();
            if (dragonBallIconAnimator != null)
            {
                dragonBallIconAnimator.SetTrigger("OnCollect");
            }
        }
    }

    // --- HÀM GAME OVER (Giữ nguyên) ---
    public void ShowGameOverUI()
    {
        if (gameOverScreen != null)
        {
            gameOverScreen.SetActive(true);
        }
    }

    public void HideGameOverUI()
    {
        if (gameOverScreen != null)
        {
            gameOverScreen.SetActive(false);
        }
    }

    // --- HÀM MỚI (ĐÃ THÊM VÀO) ---
    // Hàm này dùng để Nút "Chơi Lại" gọi
    public void OnClick_RestartButton()
    {
        // Kiểm tra xem GameManager (luôn sống) có tồn tại không
        if (GameManager.instance != null)
        {
            // Ra lệnh cho GameManager khởi động lại
            GameManager.instance.RestartGame();
        }
        else
        {
            Debug.LogError("Không tìm thấy GameManager.instance để Restart!");
        }
    }
}