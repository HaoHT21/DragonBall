using UnityEngine;
using UnityEngine.SceneManagement; // Cần thiết cho việc quản lý Scene

public class MenuManager : MonoBehaviour
{
    // Kéo và thả Panel Cài đặt UI vào slot này trong Inspector
    // (Lưu ý: Panel này nên chứa một CanvasGroup để dễ dàng ẩn/hiện)
    [SerializeField]
    private GameObject settingsPanel;

    // Đặt tên Scene Game chính của bạn vào đây
    [SerializeField]
    private string gameSceneName = "GameLevel";

    // --- CHỨC NĂNG 1: CHƠI GAME (Tải Scene) ---

    /// <summary>
    /// Bắt đầu game bằng cách tải Scene game chính.
    /// </summary>
    public void PlayGame()
    {
        // Bạn có thể dùng LoadSceneAsync để có màn hình tải mượt mà hơn
        SceneManager.LoadScene(gameSceneName);
        Debug.Log("Bắt đầu Game: Tải Scene " + gameSceneName);
    }

    // --- CHỨC NĂNG 2: MỞ/ĐÓNG PANEL CÀI ĐẶT ---

    /// <summary>
    /// Chuyển đổi trạng thái hiển thị của Panel Cài đặt.
    /// </summary>
    public void ToggleSettings()
    {
        // Kiểm tra Panel có tồn tại không
        if (settingsPanel != null)
        {
            // Đảo ngược trạng thái hiện tại: true thành false, false thành true
            bool isActive = settingsPanel.activeSelf;
            settingsPanel.SetActive(!isActive);

            Debug.Log("Panel Cài đặt: " + (!isActive ? "Mở" : "Đóng"));
        }
        else
        {
            Debug.LogError("Lỗi: Chưa gán Settings Panel vào script MenuManager!");
        }
    }

    /// <summary>
    /// Đóng Panel Cài đặt (Hữu ích khi gán vào nút "Back" hoặc "Apply").
    /// </summary>
    public void CloseSettings()
    {
        if (settingsPanel != null && settingsPanel.activeSelf)
        {
            settingsPanel.SetActive(false);
            Debug.Log("Đóng Panel Cài đặt.");
        }
    }

    // --- CHỨC NĂNG 3: THOÁT GAME ---

    /// <summary>
    /// Thoát khỏi ứng dụng game.
    /// </summary>
    public void QuitGame()
    {
        Debug.Log("Thoát Game...");

        // Thoát ứng dụng khi chạy trên bản build (.exe, .app)
        Application.Quit();

        // Thoát khỏi chế độ Play trong Unity Editor (chỉ dùng cho mục đích debug)
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}