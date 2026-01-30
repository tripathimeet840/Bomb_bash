using System.Collections;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [System.Serializable]
    public class WaveConfig
    {
        public bool spawnEnemies = true;
        public bool spawnBarrels = false;
        public bool spawnAirStrike = false;
    }
    [SerializeField] private WaveConfig[] waves; 

    public GameObject StartCam;
    [SerializeField] public GameObject[] allEnemies;
    public static GameManager instance;

    int totalEnemies;
    int currentKilled;
    bool Started;
    bool gameOver;

    [SerializeField] UIManager uIManager;
    public GameObject levelScript;

    [SerializeField] public Joystick joystick;
    [SerializeField] public Transform Aimer;

    [SerializeField] BombSpawner bombSpawner;
    [SerializeField] int bombsToSpawn = 3;
    [SerializeField] int bombSpawningInterval = 5;

    [SerializeField] EnemySpawner enemySpawner;
    private int round = 0;
    private int[] enemiesPerRound = { 6, 12, 18};

    public GameObject wonPanel;
    public TextMeshProUGUI text;
    public TextMeshProUGUI countdownText;

    public GameObject joystickVisual;
    public GameObject bgImage;

    [Header("Music & SFX")]
    [SerializeField] private AudioSource musicSource;   
    [SerializeField] private AudioClip[] waveMusic;    
    [SerializeField] private AudioSource sfxSource;    
    [SerializeField] private AudioClip countdownSFX;  

    [SerializeField] private SpawnObjectsInAir barrelSpawner;
    [SerializeField] private SimpleAirStrike airStrikeSpawner;

    private const string GamesPlayedKey = "GamesPlayed";
    private const string GamesWonKey = "GamesWon";
    private const string GamesLostKey = "GamesLost";

    private bool countedThisGame = false; 

    private void Awake()
    {
        instance = this;
        joystick = GameObject.FindGameObjectWithTag("Joystick").transform.GetChild(0).GetComponent<Joystick>();
        Aimer = GameObject.FindGameObjectWithTag("Aimer").transform;

        wonPanel.SetActive(false);

        countedThisGame = false;
    }

    void Start()
    {
        //Application.targetFrameRate = 60;
        uIManager = UIManager.instance;
    }

    private void Update()
    {
        text.text = "Wave: " + round + "/" + enemiesPerRound.Length;
    }

    private void IncrementGamesPlayed()
    {
        int games = PlayerPrefs.GetInt(GamesPlayedKey, 0);
        PlayerPrefs.SetInt(GamesPlayedKey, games + 1);
    }

    private void IncrementGamesWon()
    {
        int won = PlayerPrefs.GetInt(GamesWonKey, 0);
        PlayerPrefs.SetInt(GamesWonKey, won + 1);
    }

    private void IncrementGamesLost()
    {
        int lost = PlayerPrefs.GetInt(GamesLostKey, 0);
        PlayerPrefs.SetInt(GamesLostKey, lost + 1);
    }

    private void DecrementGamesLost()
    {
        int lost = PlayerPrefs.GetInt(GamesLostKey, 0);
        if (lost > 0)
            PlayerPrefs.SetInt(GamesLostKey, lost - 1);
    }

    private PlayerMovement GetActivePlayer()
    {
        PlayerMovement[] players = FindObjectsOfType<PlayerMovement>();
        foreach (var p in players)
        {
            if (p.gameObject.activeInHierarchy)
                return p;
        }
        return null;
    }

    public void StartGame(float _time)
    {
        if (!Started)
        {
            Started = true;
            if (levelScript != null)
                levelScript.SetActive(true);

            StartCoroutine(CountdownAndStartFirstRound(_time));
        }
    }

    private IEnumerator CountdownAndStartFirstRound(float _time)
    {
        yield return StartCoroutine(RunCountdown(round + 1));

        SpawnNextRound();

        if (bombSpawner != null)
            StartCoroutine(SpawnBombsRoutine());

        uIManager.SetDeathText(currentKilled, totalEnemies);
        FindObjectOfType<HealthProgression>().UpdateUI();

        StartCoroutine(StartGameCor(_time));
    }

    private IEnumerator RunCountdown(int upcomingWave)
    {
        int countdown = 3;
        bgImage.SetActive(true);

        if (countdownSFX != null && sfxSource != null)
            sfxSource.PlayOneShot(countdownSFX);

        while (countdown > 0)
        {
            countdownText.gameObject.SetActive(true);
            countdownText.text = $"Wave {upcomingWave} starting in {countdown}...";
            yield return new WaitForSeconds(1f);
            countdown--;
        }

        yield return new WaitForSeconds(0.5f);
        countdownText.gameObject.SetActive(false);
        bgImage.SetActive(false);

        PlayWaveMusic(round);
    }

    public void SpawnNextRound()
    {
        if (round >= waves.Length)
        {
            Debug.Log("All rounds completed!");
            return;
        }

        currentKilled = 0;

        if (waves[round].spawnEnemies)
        {
            allEnemies = enemySpawner.SpawnEnemies(enemiesPerRound[round]);
            totalEnemies = allEnemies.Length;
        }
        else
        {
            allEnemies = new GameObject[0];
            totalEnemies = 0;
        }

        uIManager.ShowWaveText(round + 1);

        if (barrelSpawner != null) barrelSpawner.enabled = waves[round].spawnBarrels;
        if (airStrikeSpawner != null) airStrikeSpawner.enabled = waves[round].spawnAirStrike;

        round++;
    }

    private void PlayWaveMusic(int waveIndex)
    {
        if (waveMusic.Length == 0 || musicSource == null) return;

        int musicIndex = waveIndex % waveMusic.Length;

        if (musicSource.clip != waveMusic[musicIndex])
        {
            musicSource.clip = waveMusic[musicIndex];
            musicSource.Play();
        }
    }

    IEnumerator StartGameCor(float _time)
    {
        StartCam.SetActive(false);
        yield return new WaitForSeconds(_time + 0.2f);

        GetActivePlayer()?.StartGame();

        for (int i = 0; i < allEnemies.Length; i++)
        {
            allEnemies[i].GetComponent<AIMovement>().enabled = true;
            allEnemies[i].GetComponent<Target>().enabled = true;
        }
    }

    private IEnumerator SpawnBombsRoutine()
    {
        while (!gameOver)
        {
            if (bombSpawner != null)
                bombSpawner.SpawnBombs(bombsToSpawn);

            yield return new WaitForSeconds(bombSpawningInterval);
        }
    }

    public void EnemyDied()
    {
        currentKilled++;
        uIManager.EnemyDied(currentKilled, totalEnemies);

        if (currentKilled >= totalEnemies && !gameOver)
        {
            //if (levelScript != null)
            //    levelScript.SetActive(false);

            if (round < enemiesPerRound.Length)
            {
                Debug.Log("Player cleared round " + round + ", preparing next round...");
                StartCoroutine(CountdownAndSpawnNextRound());
            }
            else
            {
                gameOver = true;
                GetActivePlayer()?.Won();
                joystick.gameObject.SetActive(false);
                StartCoroutine(WonDelay());
            }
        }
    }

    private IEnumerator CountdownAndSpawnNextRound()
    {
        yield return StartCoroutine(RunCountdown(round + 1));

        SpawnNextRound();
        uIManager.SetDeathText(currentKilled, totalEnemies);

        for (int i = 0; i < allEnemies.Length; i++)
        {
            allEnemies[i].GetComponent<AIMovement>().enabled = true;
            allEnemies[i].GetComponent<Target>().enabled = true;
        }

        FindObjectOfType<HealthProgression>().UpdateUI();
    }

    private IEnumerator WonDelay()
    {
        if (levelScript != null)
            levelScript.SetActive(false);

        yield return new WaitForSeconds(5f);
        

        if (!countedThisGame)
        {
            IncrementGamesWon();
            IncrementGamesPlayed();
            countedThisGame = true;
        }

        uIManager.PlayerWon();
        wonPanel.SetActive(true);
    }

    public void PlayerLose()
    {
        if (!gameOver)
        {
            gameOver = true;

            if (!countedThisGame)
            {
                IncrementGamesLost();
                IncrementGamesPlayed();
                countedThisGame = true;
            }

            joystick.gameObject.SetActive(false);
            uIManager.PlayerLose();
        }
    }

    public void RevivePlayer()
    {
        DecrementGamesLost();
        countedThisGame = false;

        gameOver = false;
        PlayerMovement activePlayer = GetActivePlayer();

        if (activePlayer != null)
            activePlayer.RevivePlayer();
    }

    public int GetGamesPlayed() => PlayerPrefs.GetInt(GamesPlayedKey, 0);
    public int GetGamesWon() => PlayerPrefs.GetInt(GamesWonKey, 0);
    public int GetGamesLost() => PlayerPrefs.GetInt(GamesLostKey, 0);
}
