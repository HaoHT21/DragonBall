using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    // Biến static để giữ tham chiếu duy nhất đến instance của SceneLoader
    public static SceneLoader Instance;

    // Tên Scene mặc định bạn muốn chuyển đến (có thể được gán lại trước khi gọi LoadTargetScene)
    [HideInInspector] // Ẩn khỏi Inspector vì nó chỉ là biến tạm
    public string targetSceneName;

    void Awake()
    {
        // 1. Kiểm tra xem đã có Instance nào tồn tại chưa
        if (Instance == null)
        {
            // Nếu chưa, thiết lập Instance này là Instance duy nhất
            Instance = this;
            // Quan trọng: Giữ đối tượng này tồn tại khi chuyển Scene
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            // Nếu đã có Instance khác, hủy đối tượng mới này
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Hàm public được gọi để thực hiện chuyển cảnh tới Scene đã chỉ định.
    /// </summary>
    /// <param name="sceneName">Tên Scene cần tải.</param>
    public void LoadScene(string sceneName)
    {
        // Kiểm tra xem Scene có tồn tại trong cài đặt Build không
        if (SceneUtility.GetBuildIndexByScenePath(sceneName) >= 0)
        {
            Debug.Log("Đang tải Scene: " + sceneName);
            SceneManager.LoadScene(sceneName);
        }
        else
        {
            Debug.LogError("Lỗi: Scene '" + sceneName + "' không có trong Build Settings! Vui lòng thêm nó.");
        }
    }
}