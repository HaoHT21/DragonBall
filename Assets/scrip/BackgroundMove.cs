using UnityEngine;

public class BackgroundMove : MonoBehaviour
{
    public float speed = 2f;    // tốc độ chuyển động
    public float amount = 0.2f;   // biên độ lắc

    private Vector3 startPos;

    void Start()
    {
        startPos = transform.position;
    }

    void Update()
    {
        float x = Mathf.Sin(Time.time * speed) * amount;
        float y = Mathf.Cos(Time.time * speed * 0.6f) * amount;
        transform.position = new Vector3(startPos.x + x, startPos.y + y, startPos.z);
    }
}
