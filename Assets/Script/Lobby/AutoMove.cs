using UnityEngine;

public class AutoMove : MonoBehaviour
{
    public float moveSpeed = 3f;

    void Update()
    {
        transform.position += Vector3.right * moveSpeed * Time.deltaTime;
    }
}