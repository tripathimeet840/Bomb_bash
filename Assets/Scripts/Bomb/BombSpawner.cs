using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombSpawner : MonoBehaviour
{
    [Header("Bomb Settings")]
    [Tooltip("Add all different bomb prefabs here")]
    public GameObject[] bombPrefabs;

    public Vector3 areaCenter = Vector3.zero;
    public Vector3 areaSize = new Vector3(20, 0, 20);

    [Header("Spawn Options")]
    public bool randomizeBombType = true;  
    public int fixedBombIndex = 0;         

    [Header("Heal Crate")]
    public GameObject healCratePrefab;
    [Range(0f, 1f)] public float healCrateChance = 0.1f;

    public void SpawnBombs(int count)
    {
        if (bombPrefabs == null || bombPrefabs.Length == 0) return;

        for (int i = 0; i < count; i++)
        {
            GameObject bombPrefab;

            if (healCratePrefab != null && Random.value < healCrateChance)
            {
                bombPrefab = healCratePrefab;
            }
            else
            {
                if (randomizeBombType)
                {
                    bombPrefab = bombPrefabs[Random.Range(0, bombPrefabs.Length)];
                }
                else
                {
                    fixedBombIndex = Mathf.Clamp(fixedBombIndex, 0, bombPrefabs.Length - 1);
                    bombPrefab = bombPrefabs[fixedBombIndex];
                }
            }

            Vector3 randomPos = areaCenter + new Vector3(
                Random.Range(-areaSize.x / 2, areaSize.x / 2),
                1f,
                Random.Range(-areaSize.z / 2, areaSize.z / 2)
            );

            Quaternion spawnRotation = Quaternion.identity;

            if (bombPrefab == healCratePrefab)
            {
                spawnRotation = Quaternion.Euler(-90f, 0f, 0f);
            }
            else
            {
                spawnRotation = bombPrefab.transform.rotation;
            }

            Instantiate(bombPrefab, randomPos, spawnRotation);
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(areaCenter, areaSize);
    }
}
