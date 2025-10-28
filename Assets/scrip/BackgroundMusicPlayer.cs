using UnityEngine;

public class BackgroundMusicPlayer : MonoBehaviour
{
    // Biến static để lưu trữ instance duy nhất
    private static BackgroundMusicPlayer instance = null;

    // Hàm Awake được gọi trước Start
    void Awake()
    {
        // Kiểm tra xem đã có instance nào tồn tại chưa
        if (instance == null)
        {
            // Nếu chưa có, gán instance này là instance duy nhất
            instance = this;

            // Quan trọng: Bảo Unity không phá hủy object này khi chuyển scene
            DontDestroyOnLoad(gameObject);
        }
        else if (instance != this)
        {
            // Nếu đã có instance khác rồi, tự hủy object này đi (tránh bị trùng lặp nhạc)
            Destroy(gameObject);
        }
    }
}