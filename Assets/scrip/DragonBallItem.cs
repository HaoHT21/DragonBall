using UnityEngine;

public class DragonBallItem : MonoBehaviour
{
    // Kéo prefab hiệu ứng (particle) vào đây trong Inspector
    public GameObject collectionEffect;

    // (Tùy chọn) Kéo file âm thanh vào đây
    // public AudioClip collectSound; 

    private void OnTriggerEnter2D(Collider2D other)
    {
        // 1. Kiểm tra xem có phải là Player chạm vào không
        // (Hãy chắc chắn Player của bạn có Tag là "Player")
        if (other.CompareTag("Player"))
        {
            // 2. Gọi hàm Collect() bên dưới
            Collect();
        }
    }

    void Collect()
    {
        // 3. Báo cho GameManager biết là đã nhặt
        // Dòng này sẽ hoạt động vì bạn đã sửa GameManager.cs
        if (GameManager.instance != null)
        {
            GameManager.instance.CollectDragonBall();
        }
        else
        {
            Debug.LogError("Không tìm thấy GameManager.instance!");
        }

        // 4. "Làm cho đẹp": Tạo hiệu ứng
        if (collectionEffect != null)
        {
            Instantiate(collectionEffect, transform.position, Quaternion.identity);
        }

        // 5. (Tùy chọn) Phát âm thanh
        // if (collectSound != null)
        // {
        //     AudioSource.PlayClipAtPoint(collectSound, transform.position);
        // }

        // 6. Tự hủy (biến mất) viên ngọc rồng này
        Destroy(gameObject);
    }
}