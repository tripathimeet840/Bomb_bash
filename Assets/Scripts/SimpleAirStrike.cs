using System.Collections;
using UnityEngine;

public class SimpleAirStrike : MonoBehaviour
{
    public GameObject dangerSignPrefab;  
    public float detectionInterval = 5.0f; 
    public float flickerInterval = 0.5f;   
    public float flickerDuration = 1.5f;   
    public float groundY = 0f;          
    public float spawnHeight = 30f;       
    public float minX, maxX, minZ, maxZ;   
    public bool spawnRandomly = true;

    [Header("Strike Settings")]
    public int strikesPerWave = 4;       
    public float delayBetweenStrikes = 1f;

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

            for (int i = 0; i < strikesPerWave; i++)
            {
                Vector3 strikePos;

                if (spawnRandomly)
                {
                    float randomX = Random.Range(minX, maxX);
                    float randomZ = Random.Range(minZ, maxZ);
                    strikePos = new Vector3(randomX, groundY, randomZ);
                }
                else
                {
                    Transform activePlayer = GetActivePlayer();
                    if (activePlayer == null) continue;
                    strikePos = new Vector3(activePlayer.position.x, groundY, activePlayer.position.z);
                }

                StartCoroutine(FlickerAndSpawn(strikePos));

                if (i < strikesPerWave - 1)
                    yield return new WaitForSeconds(delayBetweenStrikes);
            }
        }
    }

    IEnumerator FlickerAndSpawn(Vector3 strikePos)
    {
        dangerSignPrefab.transform.position = new Vector3(
            strikePos.x,
            dangerSignPrefab.transform.position.y,
            strikePos.z
        );

        float elapsedTime = 0f;
        while (elapsedTime < flickerDuration)
        {
            if (dangerSignPrefab != null)
                dangerSignPrefab.SetActive(!dangerSignPrefab.activeSelf);

            elapsedTime += flickerInterval;
            yield return new WaitForSeconds(flickerInterval);
        }

        dangerSignPrefab.SetActive(false);

        GameObject asteroid = objectPooler.GetObject("Asteroid");
        asteroid.transform.position = strikePos + Vector3.up * spawnHeight;
        asteroid.transform.rotation = Quaternion.identity;
    }
}
