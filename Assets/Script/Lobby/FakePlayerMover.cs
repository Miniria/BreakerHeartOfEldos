using UnityEngine;

public class FakePlayerMover : MonoBehaviour
{
    public float moveSpeed = 2.5f;
    private float lifetime = 3f;

    void Update()
    {
        transform.position += Vector3.left * moveSpeed * Time.deltaTime; // ⬅ เดินสวน
        lifetime -= Time.deltaTime;

        if (lifetime <= 0)
        {
            Destroy(gameObject);
        }
    }
}