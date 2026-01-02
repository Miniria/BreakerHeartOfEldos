using UnityEngine;

public class FakePlayerSpawner : MonoBehaviour
{
    public GameObject fakePlayerPrefab;
    public Transform spawnPoint; // จุดกำเนิดทางซ้าย
    public int maxFakePlayers = 10;
    public float spawnInterval = 2f;

    private float timer = 0f;

    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= spawnInterval)
        {
            SpawnFakePlayer();
            timer = 0f;
        }
    }

    void SpawnFakePlayer()
    {
        float yOffset = Random.Range(-0.5f, 0.5f); // ตำแหน่งสุ่มเล็กน้อย
        Vector3 spawnPos = spawnPoint.position + new Vector3(0, yOffset, 0);
        Instantiate(fakePlayerPrefab, spawnPos, Quaternion.identity);
    }
}