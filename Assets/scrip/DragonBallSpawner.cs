using UnityEngine;

// Script này được gắn lên GameObject Boss
public class DragonBallSpawner : MonoBehaviour
{
    // Kéo và thả Prefab Ngọc Rồng vào đây (Ví dụ: 7 sao)
    [Header("Vật phẩm rơi ra")]
    public GameObject dragonBallPrefab;

    // Số lượng Ngọc Rồng sẽ rơi
    public int dropCount = 1;

    // Hàm được gọi từ logic xử lý cái chết của Boss
    public void SpawnDragonBall()
    {
        if (dragonBallPrefab == null)
        {
            Debug.LogError("DragonBall Prefab chưa được gắn vào DragonBallSpawner.");
            return;
        }

        Vector3 spawnPosition = transform.position;

        for (int i = 0; i < dropCount; i++)
        {
            // Thêm độ lệch ngẫu nhiên để Ngọc Rồng không rơi tại cùng một điểm
            Vector3 offset = new Vector3(Random.Range(-0.5f, 0.5f), Random.Range(0.5f, 1f), 0f);

            GameObject newBall = Instantiate(dragonBallPrefab, spawnPosition + offset, Quaternion.identity);

            // Tùy chọn: Thêm lực đẩy nhẹ nếu Ngọc Rồng có Rigidbody
            Rigidbody2D rb = newBall.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.AddForce(new Vector2(offset.x * 2f, 5f), ForceMode2D.Impulse);
            }
        }

        Debug.Log($"Đã rơi ra {dropCount} viên Ngọc Rồng.");
    }
}
