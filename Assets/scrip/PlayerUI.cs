using UnityEngine;
using UnityEngine.UI; // Cần thiết để làm việc với các thành phần UI

public class PlayerUI : MonoBehaviour
{
    [Header("UI References")]
    public Slider healthSlider; // Kéo thả Slider Thanh Máu vào đây
    public Slider kiSlider;     // Kéo thả Slider Thanh Ki vào đây

    private Player playerScript; // Tham chiếu đến script Player

    void Start()
    {
        // Lấy tham chiếu đến script Player trên cùng đối tượng
        playerScript = GetComponent<Player>();

        if (playerScript == null)
        {
            Debug.LogError("Player script not found on the same GameObject!");
            enabled = false; // Tắt script nếu không tìm thấy Player
            return;
        }

        // Khởi tạo giá trị Max cho Slider
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

    void Update()
    {
        // Cập nhật giá trị thanh máu và thanh Ki liên tục trong mỗi frame
        UpdateHealthUI(playerScript.currentHealth);
        UpdateKiUI(playerScript.currentKi);
    }

    // Hàm công khai để cập nhật MÁU (có thể gọi từ TakeDamage)
    public void UpdateHealthUI(int newHealth)
    {
        if (healthSlider != null)
        {
            healthSlider.value = newHealth;
        }
    }

    // Hàm công khai để cập nhật KI
    public void UpdateKiUI(float newKi)
    {
        if (kiSlider != null)
        {
            kiSlider.value = newKi;
        }
    }
}