using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("Enemy Prefabs")]
    public GameObject[] enemies;

    [Header("Waypoints")]
    public Transform[] waypoints;

    public GameObject[] SpawnEnemies(int count)
    {
        GameObject[] spawned = new GameObject[count];

        for (int i = 0; i < count; i++)
        {
            Transform spawnPos = waypoints[Random.Range(0, waypoints.Length)];
            GameObject enemyPrefab = enemies[Random.Range(0, enemies.Length)];

            spawned[i] = Instantiate(enemyPrefab, spawnPos.position, spawnPos.rotation);
        }

        return spawned;
    }
}
