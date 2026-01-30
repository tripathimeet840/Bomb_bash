using System.Collections;
using UnityEngine;

public class SpawnObjectsInAir : MonoBehaviour
{
    public GameObject dangerSignPrefab; // Prefab of the danger sign
    public float detectionInterval = 5.0f; // Time interval for detecting player position
    public float flickerInterval = 0.5f; // Interval at which the danger sign flickers
    public float flickerDuration = 3.0f; // Total duration of the danger sign flickering
    public int maxBarrels = 5; // Maximum number of barrels allowed in the scene
    public float YSpawnPos; // Y position offset for spawning objects
    public float minX, maxX, minZ, maxZ; // Boundaries for random spawning
    public bool spawnRandomly;

    private ObjectPooler objectPooler;

    void Start()
    {
        objectPooler = ObjectPooler.instance;
        StartCoroutine(DetectAndSpawnRoutine());
    }

    private Transform GetActivePlayer()
    {
        PlayerMovement[] players = FindObjectsOfType<PlayerMovement>();
        foreach (var p in players)
        {
            if (p.gameObject.activeInHierarchy)
                return p.transform;
        }
        return null;
    }

    IEnumerator DetectAndSpawnRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(detectionInterval);

            Vector3 spawnPosition;

            if (spawnRandomly)
            {
                // Generate a random position within the specified boundaries
                float randomX = Random.Range(minX, maxX);
                float randomZ = Random.Range(minZ, maxZ);
                spawnPosition = new Vector3(randomX, YSpawnPos, randomZ);

                // Set the danger sign position
                dangerSignPrefab.transform.position = new Vector3(randomX, dangerSignPrefab.transform.position.y, randomZ);

                // Flicker the danger sign
                StartCoroutine(FlickerDangerSign(flickerDuration, flickerInterval));

                yield return new WaitForSeconds(flickerDuration);

                // Spawn the object (asteroid) at the random position
                GameObject asteroid = objectPooler.GetObject("Asteroid");
                asteroid.transform.position = spawnPosition + Vector3.up * YSpawnPos;
                asteroid.transform.rotation = Quaternion.identity;
            }
            else
            {
                // Get current active player
                Transform activePlayer = GetActivePlayer();
                if (activePlayer == null) continue; // skip if no active player

                spawnPosition = activePlayer.position;

                int barrelCount = GameObject.FindGameObjectsWithTag("Barel").Length;

                // Set the danger sign position
                dangerSignPrefab.transform.position = new Vector3(spawnPosition.x, dangerSignPrefab.transform.position.y, spawnPosition.z);

                if (barrelCount < maxBarrels)
                    StartCoroutine(FlickerDangerSign(flickerDuration, flickerInterval));

                yield return new WaitForSeconds(flickerDuration);

                if (barrelCount < maxBarrels)
                {
                    // Spawn the barrel at the active player's position
                    GameObject barrel = objectPooler.GetObject("Barrel");
                    barrel.transform.position = spawnPosition + Vector3.up * YSpawnPos;
                    barrel.transform.rotation = Quaternion.Euler(-90, 0, -90);
                }
            }
        }
    }

    IEnumerator FlickerDangerSign(float duration, float interval)
    {
        float elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            if (dangerSignPrefab != null)
                dangerSignPrefab.SetActive(!dangerSignPrefab.activeSelf);

            elapsedTime += interval;
            yield return new WaitForSeconds(interval);
        }
        dangerSignPrefab.SetActive(false);
    }
}
