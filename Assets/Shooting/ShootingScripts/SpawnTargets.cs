using System.Collections;
using UnityEngine;

public class SpawnTargets : MonoBehaviour
{
    [SerializeField] private GameObject targetPrefab;

    [Header("Spawn Area")]
    [SerializeField] private float minX = -8f;
    [SerializeField] private float maxX = 8f;
    [SerializeField] private float minY = -4f;
    [SerializeField] private float maxY = 4f;

    [Header("Spawn Timing")]
    [SerializeField] private float minSpawnTime = 0.5f;
    [SerializeField] private float maxSpawnTime = 3f;

    void Start()
    {
        StartCoroutine(SpawnerTargets());
    }

    IEnumerator SpawnerTargets()
    {
        while (true)
        {
            float waitTime = Random.Range(minSpawnTime, maxSpawnTime);
            yield return new WaitForSeconds(waitTime);

            SpawnTarget();
        }
    }

    void SpawnTarget()
    {
        float x = Random.Range(minX, maxX);
        float y = Random.Range(minY, maxY);

        Vector2 spawnPosition = new Vector2(x, y);

        Instantiate(targetPrefab, spawnPosition, Quaternion.identity);
    }
}
