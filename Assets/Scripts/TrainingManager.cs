using DG.Tweening;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerTutorial : MonoBehaviour
{
    public TextMeshProUGUI instructionText;
    public PlayerMovement player;
    public GameObject throwButtonUI;

    [Header("Crate Setup")]
    public GameObject[] bombCratePrefabs;
    public Transform[] crateSpawnPoints;

    private GameObject tutorialCrate;
    private int currentCrateIndex = 0;

    private bool hasCompletedTraining = false;

    [Header("Loading Screen")]
    [SerializeField] private GameObject loadingScreenPanel;
    [SerializeField] private RectTransform logo;
    [SerializeField] private UnityEngine.UI.Slider loadingBar;

    public Health trainingPlayerHealth;

    private void Start()
    {
        trainingPlayerHealth.HealthInc(-30);
        StartCoroutine(ShowLoadingThenTutorial());
    }

    private IEnumerator ShowLoadingThenTutorial()
    {
        yield return StartCoroutine(LoadingScreenRoutine());

        StartCoroutine(TutorialSequence());
    }

    private IEnumerator LoadingScreenRoutine()
    {
        loadingScreenPanel.SetActive(true);
        logo.localScale = Vector3.zero;

        Tween tween = logo.DOScale(Vector3.one, 0.8f)
            .SetEase(Ease.OutBack)
            .OnComplete(() =>
            {
                logo.DOScale(Vector3.one * 1.1f, 2f)
                    .SetEase(Ease.InOutSine)
                    .SetLoops(-1, LoopType.Yoyo);
            });

        loadingBar.value = 0f;
        float targetProgress = 0f;

        // Stage 1: fast to ~50%
        while (loadingBar.value < 0.5f)
        {
            targetProgress += Time.deltaTime * 1f;
            loadingBar.value = Mathf.MoveTowards(loadingBar.value, targetProgress, Time.deltaTime);
            yield return null;
        }

        // Stage 2: slower climb to ~90%
        while (loadingBar.value < 0.9f)
        {
            targetProgress += Time.deltaTime * 1f;
            loadingBar.value = Mathf.MoveTowards(loadingBar.value, targetProgress, Time.deltaTime * 0.5f);
            yield return null;
        }

        // Stage 3: quick finish to 100%
        while (loadingBar.value < 1f)
        {
            targetProgress += Time.deltaTime * 1f;
            loadingBar.value = Mathf.MoveTowards(loadingBar.value, targetProgress, Time.deltaTime * 1f);
            yield return null;
        }

        tween.Kill();
        loadingScreenPanel.SetActive(false);
    }

    private IEnumerator TutorialSequence()
    {
        // STEP 1: Move joystick
        instructionText.text = "Move joystick to move the character!";
        yield return new WaitUntil(() => player.hasMoved);

        // Wait 2–3 seconds after movement
        float moveTimer = 0f;
        while (moveTimer < 3f)
        {
            if (player.hasMoved) moveTimer += Time.deltaTime;
            yield return null;
        }

        // Loop through all bomb crates
        while (currentCrateIndex < bombCratePrefabs.Length)
        {
            // STEP 2: Collect a crate
            instructionText.text = "Collect a crate to get a bomb!";
            SpawnCrate();

            // Wait until player collects a bomb
            yield return new WaitUntil(() => player.bombCollector.bombCount > 0);

            // STEP 3: Throw the bomb
            instructionText.text = "Aim at the enemy and press the throw button!";
            if (throwButtonUI != null) throwButtonUI.SetActive(true);

            // Store the bomb type when collected
            string collectedBombType = player.bombCollector.currentBombType;

            // Wait until bomb is thrown
            yield return new WaitUntil(() => player.bombCollector.bombCount == 0);

            // --- SHOW BLAST INSTRUCTION ONLY IF IT WAS A STICKY BOMB ---
            if (collectedBombType == "CollectableStickyBomb")
            {
                instructionText.text = "Now press the BLAST button to detonate the Sticky Bomb!";
                // blastButtonUI.SetActive(true);

                // Wait longer so player has enough time to see instruction
                float blastInstructionTimer = 3f;
                float timer = 0f;
                while (timer < blastInstructionTimer)
                {
                    timer += Time.deltaTime;
                    yield return null;
                }
                // yield return new WaitUntil(() => blastButtonPressed);
            }

            // Wait until bomb is thrown
            yield return new WaitUntil(() => player.bombCollector.bombCount == 0);

            // Wait until bomb is thrown
            yield return new WaitUntil(() => player.bombCollector.bombCount == 0);

            // Small delay before spawning next crate
            yield return new WaitForSeconds(1f);

            currentCrateIndex++;
        }

        instructionText.text = "Great! Tutorial complete!";

        // Wait 2 seconds before loading next scene
        yield return new WaitForSeconds(3f);

        FinishTutorial();

        if (throwButtonUI != null) throwButtonUI.SetActive(false);
    }

    private void SpawnCrate()
    {
        if (bombCratePrefabs.Length == 0 || crateSpawnPoints.Length == 0) return;

        GameObject prefab = bombCratePrefabs[currentCrateIndex];
        Transform spawnPoint = crateSpawnPoints[Random.Range(0, crateSpawnPoints.Length)];

        tutorialCrate = Instantiate(prefab, spawnPoint.position, spawnPoint.rotation);
    }

    public void FinishTutorial()
    {
        hasCompletedTraining = true; // Mark training as done
        PlayerPrefs.SetInt("IsFirstTime", 0);
        PlayerPrefs.Save();

        Debug.Log("Tutorial Finished. hasCompletedTraining = " + hasCompletedTraining);

        // Directly load the LandingScene without showing the loading screen again
        SceneManager.LoadScene("LandingScene");
    }
}
