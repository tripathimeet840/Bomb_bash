using UnityEngine;
using System.Collections;

public class TrainingGameManager : MonoBehaviour
{
    public static TrainingGameManager instance;

    [Header("References")]
    [SerializeField] private BombSpawner bombSpawner;
    [SerializeField] private PlayerTraining player;
    [SerializeField] private GameObject trainingCompleteCanvas;

    [Header("Bomb Spawning")]
    [SerializeField] private int bombsToSpawn = 3;
    [SerializeField] private float bombSpawnInterval = 5f;

    private bool trainingComplete = false;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        if (bombSpawner != null)
            StartCoroutine(SpawnBombsRoutine());
    }

    private IEnumerator SpawnBombsRoutine()
    {
        while (!trainingComplete)
        {
            bombSpawner.SpawnBombs(bombsToSpawn);
            yield return new WaitForSeconds(bombSpawnInterval);
        }
    }

    // Call this when player destroys enough targets
    public void TrainingCompleted()
    {
        if (trainingComplete) return;

        trainingComplete = true;
        StopAllCoroutines();

        if (trainingCompleteCanvas != null)
        {
            trainingCompleteCanvas.SetActive(true);
            trainingCompleteCanvas.transform.localScale = Vector3.zero;
        }

        PlayerPrefs.SetInt("TrainingComplete", 1);
    }
}
