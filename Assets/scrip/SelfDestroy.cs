using UnityEngine;
using System.Collections;

public class SelfDestroy : MonoBehaviour
{
    // Thời gian hiệu ứng tồn tại (ví dụ: 0.5 giây)
    public float lifeTime = 0.5f;

    void Start()
    {
        // Hủy GameObject này sau thời gian lifeTime
        Destroy(gameObject, lifeTime);
    }
}