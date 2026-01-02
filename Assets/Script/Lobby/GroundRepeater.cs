using UnityEngine;

public class GroundRepeater : MonoBehaviour
{
    public Transform player;
    public float repeatLength = 20f;

    void Update()
    {
        // ถ้า Player เดินเลยพื้นนี้ไปแล้ว
        if (player.position.x - transform.position.x > repeatLength)
        {
            // ย้ายพื้นไปไว้ข้างหน้า
            transform.position += Vector3.right * (repeatLength * 2);
        }
    }
}