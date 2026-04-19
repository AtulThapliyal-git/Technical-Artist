using UnityEngine;

public class TrafficSpawner : MonoBehaviour
{
    [Header("Spawning Settings")]
    public GameObject[] trafficPrefabs;
    public float spawnRate = 2f;
    public float spawnDistanceZ = 150f;
    public float spawnHeight = 0f;

    [Header("Exact Lane Positions")]
    [Tooltip("Type the exact X positions of your lanes here.")]
    public float[] lanePositions = { -4f, -2f, 0f, 2f, 4f }; // Change these to match your road!

    [Header("Required Reference")]
    public Transform playerCar;

    private float timer = 0f;

    void Update()
    {
        if (playerCar == null) return;

        timer += Time.deltaTime;
        if (timer >= spawnRate)
        {
            SpawnCar();
            timer = 0f;
        }
    }

    private void SpawnCar()
    {
        if (trafficPrefabs.Length == 0 || lanePositions.Length == 0) return;

        float exactXLane = lanePositions[Random.Range(0, lanePositions.Length)];

        Vector3 spawnPosition = new Vector3(exactXLane, spawnHeight, playerCar.position.z + spawnDistanceZ);

        GameObject selectedPrefab = trafficPrefabs[Random.Range(0, trafficPrefabs.Length)];
        Instantiate(selectedPrefab, spawnPosition, Quaternion.identity);
    }
}