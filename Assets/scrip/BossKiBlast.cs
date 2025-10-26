using UnityEngine;

public class BossKiBlast : MonoBehaviour
{
    // Cấu hình sát thương
    public int damage = 30; // Cần gán giá trị này trong Inspector của Prefab
    public GameObject hitEffectPrefab;

    // Hàm gọi khi va chạm
    private void OnTriggerEnter2D(Collider2D other)
    {
        // 1. Kiểm tra có phải là Player không
        if (other.CompareTag("Player"))
        {
            // 2. Lấy script Player và gây sát thương
            Player playerScript = other.GetComponent<Player>();
            if (playerScript != null)
            {
                playerScript.TakeDamage(damage);

                // 3. Tạo hiệu ứng
                if (hitEffectPrefab != null)
                {
                    Instantiate(hitEffectPrefab, transform.position, Quaternion.identity);
                }
            }

            // 4. Hủy viên đạn (vì đã chạm)
            Destroy(gameObject);
        }

        // *Thêm logic hủy đạn khi chạm tường/đất nếu cần*
        if (other.CompareTag("Ground"))
        {
            Destroy(gameObject);
        }
    }
}