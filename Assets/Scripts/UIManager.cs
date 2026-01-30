using DG.Tweening;
using GoogleMobileAds.Api;
using GoogleMobileAds.Sample;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Services.CloudSave;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public float startTransitiontime;
    public Ease ease;
    public RectTransform[] LeftPanels, RightPanels, TopPanels, BottomPanels;
    public static UIManager instance;
    [Header("InGame")]
    [SerializeField] RectTransform DeathCounter;
    [SerializeField] RectTransform backButton;
    public TMP_Text scoreText;
    Vector3 startScoreScale;
    [SerializeField] GameObject EndPanel;
    [SerializeField] TextMeshProUGUI[] AllCoinsText, AllGemsText;
    [SerializeField] AdWheel adWheel;

    [SerializeField] GameObject SkinsMenu, MainMenu, UpgradesMenu, CharacterUpgrade, CharacterUnlock, inventoryCam, shopCam, startCam;
    public int PlayerWonCoin, PlayerLoseCoin;
    [SerializeField] GameObject Nothanks;
    bool started;
    SoundManager soundManager;

    [Header("Settings")]
    [SerializeField] GameObject SettingsPanel;
    bool settingOpen;
    bool soundOff;
    bool bgMusicOff;
    [SerializeField] Image SoundImage, BgImage;
    [SerializeField] Sprite SoundOnSprite, SoundOffSprite, BgMusicOnSprite, BgMusicOffSprite;


    [Header("NoAds")]
    public GameObject NoAdsPanel;
    public GameObject RemoveAdsPanel;
    public GameObject RemoveAdsButton;

    [Header("InApp")]
    public GameObject InAppWindow;

    [Header("Levels")]
    public GameObject levelWindow;
    AchievementsManager achievementsManager;

    [Header("Pause")]
    [SerializeField] GameObject PauseCanvas;

    [SerializeField] GameObject WatchAdsToUpgradeWindow;
    bool isPower;

    [Header("Wave UI")]
    [SerializeField] TMP_Text waveText;
    [SerializeField] TMP_Text waveCounterText;

    [Header("Store")]
    [SerializeField] GameObject StoreMenu;
    [SerializeField] ScrollRect storeScroll;

    [SerializeField] GameObject leaderboardsPanel;
    [SerializeField] public GameObject changeNamePanel;

    [Header("Player Info")]
    [SerializeField] TMP_Text playerNameText;
    [SerializeField] TMP_Text playerNameText2;
    [SerializeField] TMP_Text playerLevelText;
    [SerializeField] TMP_Text playerLevelText2;
    [SerializeField] TMP_InputField nameInputField;

    [Header("Loading Screen")]
    [SerializeField] GameObject loadingScreenPanel;
    [SerializeField] RectTransform logo;
    [SerializeField] private Slider loadingBar;

    public bool homeScreen;

    public HomeStatsUI homeStatsUI;

    [Header("Revive Button")]
    public Button reviveButton;

    [Header("Player Keys")]
    [SerializeField] private TMP_Text keysText;

    private void Awake()
    {
        if (instance == null)
            instance = this;

        StartCoroutine(LoadingThenCountdownRoutine());
    }

    private IEnumerator LoadingThenCountdownRoutine()
    {
        yield return StartCoroutine(LoadingScreenRoutine());

        if (!homeScreen)
        {
            StartGame();
        }
    }

    private void Start()
    {
        achievementsManager = AchievementsManager.instance;
        soundManager = SoundManager.soundManager;
        startScoreScale = DeathCounter.localScale;

        // Load all local data
        LoadPlayerDataFromLocal();

        SaveToCloud("Name", playerNameText.text);
        SaveToCloud("LevelNumber", PlayerPrefs.GetInt("LevelNumber", 1));

        CheckAdsRemoved();
        LoadSettings();
        UpdateSoundIcon();
        UpdateBgMusicIcon();
        Time.timeScale = 1;

        playerNameText2.text = playerNameText.text;
        playerLevelText2.text = playerLevelText.text;
    }

    private void Update()
    {
        playerNameText2.text = playerNameText.text;
        playerLevelText2.text = playerLevelText.text;
    }

    private IEnumerator ShowNamePanelAfterLoading()
    {
        // Wait until loading screen finishes
        yield return StartCoroutine(LoadingScreenRoutine());

        changeNamePanel.SetActive(true);
    }

    private void LoadPlayerDataFromLocal()
    {
        // Load basic info
        string savedName = PlayerPrefs.GetString("PlayerName", "Player");
        int savedLevel = PlayerPrefs.GetInt("LevelNumber", 1);
        int savedCoins = PlayerPrefs.GetInt("Coins", 0);
        int savedGems = PlayerPrefs.GetInt("Gems", 0);
        int savedKeys = PlayerPrefs.GetInt("Keys", 0);

        bool isFirstTime = PlayerPrefs.GetInt("IsFirstTime", 1) == 1;
        bool removeAds = PlayerPrefs.GetInt("RemoveAds", 0) == 1;

        if (playerNameText != null) playerNameText.text = savedName;
        if (playerLevelText != null) playerLevelText.text = "Level " + savedLevel;

        for (int i = 0; i < AllCoinsText.Length; i++)
            AllCoinsText[i].text = savedCoins.ToString();

        for (int i = 0; i < AllGemsText.Length; i++)
            AllGemsText[i].text = savedGems.ToString();

        if (keysText != null)
            keysText.text = savedKeys.ToString();

        CheckAdsRemoved();
    }


    public void ShowLoadingScreen()
    {
        StartCoroutine(LoadingScreenRoutine());
    }

    private IEnumerator LoadingScreenRoutine()
    {
        loadingScreenPanel.SetActive(true);
        logo.localScale = Vector3.zero;

        


        // Animate logo
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

        // Stage 1: quick jump to ~50%
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

        // At this point loading is finished
        loadingScreenPanel.SetActive(false);

        if (PlayerPrefs.GetInt("IsFirstTime", 1) == 1)
        {
            // First time -> go to tutorial
            SceneManager.LoadScene("Training New");
        }
        else
        {
            // Not first time -> stay in same scene, show main menu
            homeScreen = true;
            MainMenu.SetActive(true);
        }
    }


    //public void ShowStartCountdown()
    //{
    //    StartCoroutine(StartCountdownRoutine());
    //}

    //private IEnumerator StartCountdownRoutine()
    //{
    //    int countdown = 3;

    //    while (countdown > 0)
    //    {
    //        waveText.gameObject.SetActive(true);
    //        waveText.text = "Game starting in " + countdown;
    //        waveText.alpha = 0;
    //        waveText.transform.localScale = Vector3.zero;

    //        // Animate: fade in + scale up
    //        waveText.DOFade(1f, 0.4f);
    //        waveText.transform.DOScale(Vector3.one, 0.4f).SetEase(Ease.OutBack);

    //        yield return new WaitForSeconds(1f);

    //        // Fade out & scale down
    //        waveText.DOFade(0f, 0.3f);
    //        waveText.transform.DOScale(Vector3.zero, 0.3f);

    //        countdown--;
    //        yield return new WaitForSeconds(0.2f);
    //    }

    //    waveText.gameObject.SetActive(false);

    //    // After countdown, start the actual game
    //    StartGame();
    //}

    private async void SaveToCloud(string key, object value)
    {
        try
        {
            var data = new Dictionary<string, object> { { key, value } };
            await CloudSaveService.Instance.Data.ForceSaveAsync(data);
            Debug.Log($"[CloudSave] Saved {key}: {value}");
        }
        catch (System.Exception ex)
        {
            Debug.LogError("[CloudSave] Failed to save " + key + " - " + ex.Message);
        }
    }

    public void SavePlayerName()
    {
        string newName = string.IsNullOrEmpty(nameInputField.text) ? "Player" : nameInputField.text;

        PlayerPrefs.SetString("PlayerName", newName);
        PlayerPrefs.SetInt("IsFirstTime", 0);
        PlayerPrefs.Save();

        playerNameText.text = newName;

        SaveToCloud("Name", newName);
        SaveToCloud("IsFirstTime", 0);

        changeNamePanel.SetActive(false);
    }


    public void SavePlayerLevel(int level)
    {
        PlayerPrefs.SetInt("LevelNumber", level);
        PlayerPrefs.Save();
        playerLevelText.text = "Level " + level;
        SaveToCloud("LevelNumber", level);
    }


    public void StartGame()
    {
        if (!started)
        {
            started = true;
            soundManager.PlayButtonSound();
            GameManager.instance.StartGame(startTransitiontime);
            StartCoroutine(ShowInGameUI());
            MainMenu.SetActive(false);

            foreach (RectTransform panel in LeftPanels)
            {
                panel.DOAnchorPos(new Vector2(-500f, panel.anchoredPosition.y), 0.8f, false).SetEase(ease).OnComplete(() => { panel.gameObject.SetActive(false); });
            }

            // Animate right panels
            foreach (RectTransform panel in RightPanels)
            {
                panel.DOAnchorPos(new Vector2(500f, panel.anchoredPosition.y), 0.8f, false).SetEase(ease).OnComplete(() => { panel.gameObject.SetActive(false); });
            }

            // Animate top panels
            foreach (RectTransform panel in TopPanels)
            {
                panel.DOAnchorPos(new Vector2(panel.anchoredPosition.x, 500f), 0.8f, false).SetEase(ease).OnComplete(() => { panel.gameObject.SetActive(false); });
            }

            // Animate bottom panels
            foreach (RectTransform panel in BottomPanels)
            {
                panel.DOAnchorPos(new Vector2(panel.anchoredPosition.x, -500f), 0.8f, false).SetEase(ease).OnComplete(() => { panel.gameObject.SetActive(false); });
            }
        }
    }

    IEnumerator ShowInGameUI()
    {
        yield return new WaitForSeconds(0.8f);
        transform.GetChild(0).gameObject.SetActive(false);
        //DeathCounter.DOAnchorPos(new Vector2(DeathCounter.anchoredPosition.x, -120f), 0.8f, false).SetEase(ease);
        //backButton.DOAnchorPos(new Vector2(backButton.anchoredPosition.x, -120f), 0.8f, false).SetEase(ease);
    }

    public void EnemyDied(int killed, int total)
    {
        scoreText.text = killed + "/" + total;

        DeathCounter.DOScale(startScoreScale + new Vector3(0.2f, 0.2f, 0.2f), 0.25f)
            .SetEase(Ease.OutElastic)
            .OnComplete(() =>
            {
                DeathCounter.DOScale(startScoreScale, 0.25f).SetEase(Ease.OutElastic);
            });
    }

    public void ShowRemoveAdsPanel()
    {
        soundManager.PlayButtonSound();
        NoAdsPanel.SetActive(true);  
    }

    public void CloseRemoveAdsPanel()
    {
        soundManager.PlayButtonSound();
        NoAdsPanel.SetActive(false); 
    }

    public void SetDeathText(int _killedNumber, int _total)
    {
        scoreText.text = _killedNumber + "/" + _total;
    }

    public void PlayerWon()
    {
        // Update win streak
        int currentStreak = PlayerPrefs.GetInt("WinStreak", 0);
        currentStreak++;
        PlayerPrefs.SetInt("WinStreak", currentStreak);
        //PlayerPrefs.SetInt("GamesWon", PlayerPrefs.GetInt("GamesWon", 0) + 1);
        //PlayerPrefs.SetInt("GamesPlayed", PlayerPrefs.GetInt("GamesPlayed", 0) + 1);
        PlayerPrefs.Save();

        homeStatsUI?.UpdateStatsUI();

        StartCoroutine(PlayerWonCanvas());
        EndPanel.SetActive(false);
    }

    public void PlayerLose()
    {
        // Reset win streak
        PlayerPrefs.SetInt("WinStreak", 0);
        //PlayerPrefs.SetInt("GamesLost", PlayerPrefs.GetInt("GamesLost", 0) + 1);
        //PlayerPrefs.SetInt("GamesPlayed", PlayerPrefs.GetInt("GamesPlayed", 0) + 1);
        PlayerPrefs.Save();

        homeStatsUI?.UpdateStatsUI();

        EndPanel.SetActive(true);
        StartCoroutine(PlayerLoseCanvas());
    }

    IEnumerator PlayerWonCanvas()
    {
        if (PlayerPrefs.GetInt("FirstGame") != 1)
        {
            achievementsManager.UnlockAchievement(AchievementID.FirstRun);
            PlayerPrefs.SetInt("FirstGame", 1);
        }
        CheckWonAchievements();
        CheckGamesPlayedAchievements(); // <-- unlock achievements here

        PlayerPrefs.SetInt("Coins", PlayerPrefs.GetInt("Coins") + PlayerWonCoin);
        PlayerPrefs.Save();
        SetCoinsText();
        SaveToCloud("Coins", PlayerPrefs.GetInt("Coins"));

        DeathCounter.gameObject.SetActive(false);
        adWheel.SetCoinsAmt(PlayerWonCoin);
        EndPanel.SetActive(true);
        EndPanel.transform.DOScale(Vector3.one, 0.45f).SetEase(Ease.Linear);
        SoundManager.soundManager.PlayLevelFailed();
        //RateGame.Instance.IncreaseCustomEvents();
        //RateGame.Instance.ShowRatePopup();
        yield return new WaitForSeconds(4f);

        PlayerPrefs.SetInt("AdsWonCount", PlayerPrefs.GetInt("AdsWonCount") + 1);

        if (PlayerPrefs.GetInt("AdsWonCount") >= 2)
        {
            PlayerPrefs.SetInt("AdsWonCount", 0);
            //if (PlayerPrefs.GetInt("RemoveAds") != 1)
                //Mediation_Manager.mediation_Manager.ShowInterstitialAd();
        }

        PlayerPrefs.SetInt("RandomLevel", PlayerPrefs.GetInt("RandomLevel") + 1);

        if (PlayerPrefs.GetInt("RandomLevel") >= 1)
        {
            PlayerPrefs.SetInt("RandomLevel", 0);
            FindObjectOfType<RandomLevelSelector>().ShowRandomLevels();
        }
    }

    IEnumerator PlayerLoseCanvas()
    {
        if (PlayerPrefs.GetInt("FirstGame") != 1)
        {
            achievementsManager.UnlockAchievement(AchievementID.FirstRun);
            PlayerPrefs.SetInt("FirstGame", 1);
        }
        CheckLoseAchievements();
        yield return new WaitForSeconds(4f);
        if (PlayerPrefs.GetInt("RemoveAds") != 1)
        {
            //Mediation_Manager.mediation_Manager.ShowInterstitialAd();
        }

        PlayerPrefs.SetInt("RandomLevel", PlayerPrefs.GetInt("RandomLevel") + 1);

        if (PlayerPrefs.GetInt("RandomLevel") >= 1)
        {
            PlayerPrefs.SetInt("RandomLevel", 0);
            FindObjectOfType<RandomLevelSelector>().ShowRandomLevels();
        }

        PlayerPrefs.SetInt("Coins", PlayerPrefs.GetInt("Coins") + PlayerLoseCoin);
        PlayerPrefs.Save();
        SetCoinsText();
        SaveToCloud("Coins", PlayerPrefs.GetInt("Coins")); // CLOUD SYNC

        DeathCounter.gameObject.SetActive(false);
        adWheel.SetCoinsAmt(PlayerLoseCoin);
        EndPanel.SetActive(true);
        EndPanel.transform.DOScale(Vector3.one, 0.45f).SetEase(Ease.Linear);
        //RateGame.Instance.IncreaseCustomEvents();
        //RateGame.Instance.ShowRatePopup();
        yield return new WaitForSeconds(4f);
        Nothanks.SetActive(true);
        CheckGamesPlayedAchievements(); // unlock achievements here
    }


    void CheckLoseAchievements()
    {
        // Increment LoseCount by 1
        PlayerPrefs.SetInt("LoseCount", PlayerPrefs.GetInt("LoseCount") + 1);
        int loses = PlayerPrefs.GetInt("LoseCount");

        // Unlock "Lose 5" achievement
        if (loses >= 5 && PlayerPrefs.GetInt("Lose5Achieved", 0) != 1)
        {
            achievementsManager.UnlockAchievement(AchievementID.Lose5);
            PlayerPrefs.SetInt("Lose5Achieved", 1); // Mark the achievement as unlocked
        }

        // Unlock "Lose 10" achievement
        if (loses >= 10 && PlayerPrefs.GetInt("Lose10Achieved", 0) != 1)
        {
            achievementsManager.UnlockAchievement(AchievementID.Lose10);
            PlayerPrefs.SetInt("Lose10Achieved", 1); // Mark the achievement as unlocked
        }

        // Unlock "Lose 20" achievement
        if (loses >= 20 && PlayerPrefs.GetInt("Lose20Achieved", 0) != 1)
        {
            achievementsManager.UnlockAchievement(AchievementID.Lose20);
            PlayerPrefs.SetInt("Lose20Achieved", 1); // Mark the achievement as unlocked
        }

        // Save PlayerPrefs to persist data
        PlayerPrefs.Save();
    }

    void CheckWonAchievements()
    {
        PlayerPrefs.SetInt("WonCount", PlayerPrefs.GetInt("WonCount") + 1);

        int wins = PlayerPrefs.GetInt("WonCount");

        if (wins >= 5 && PlayerPrefs.GetInt("Won5Achieved", 0) != 1)
        {
            achievementsManager.UnlockAchievement(AchievementID.Won5); // Unlock "Won 5" achievement
            PlayerPrefs.SetInt("Won5Achieved", 1); // Mark the achievement as unlocked
        }

        if (wins >= 10 && PlayerPrefs.GetInt("Won10Achieved", 0) != 1)
        {
            achievementsManager.UnlockAchievement(AchievementID.Won10); // Unlock "Won 10" achievement
            PlayerPrefs.SetInt("Won10Achieved", 1); // Mark the achievement as unlocked
        }

        if (wins >= 20 && PlayerPrefs.GetInt("Won20Achieved", 0) != 1)
        {
            achievementsManager.UnlockAchievement(AchievementID.Won20); // Unlock "Won 20" achievement
            PlayerPrefs.SetInt("Won20Achieved", 1); // Mark the achievement as unlocked
        }

        if (wins >= 50 && PlayerPrefs.GetInt("Won50Achieved", 0) != 1)
        {
            achievementsManager.UnlockAchievement(AchievementID.Won50); // Unlock "Won 50" achievement
            PlayerPrefs.SetInt("Won50Achieved", 1); // Mark the achievement as unlocked
        }

        if (wins >= 100 && PlayerPrefs.GetInt("Won100Achieved", 0) != 1)
        {
            achievementsManager.UnlockAchievement(AchievementID.Won100); // Unlock "Won 100" achievement
            PlayerPrefs.SetInt("Won100Achieved", 1); // Mark the achievement as unlocked
        }
        PlayerPrefs.Save();
    }

    void CheckGamesPlayedAchievements()
    {
        int gamesPlayed = PlayerPrefs.GetInt("GamesPlayed", 0);

        if (gamesPlayed >= 5 && PlayerPrefs.GetInt("Played5Achieved", 0) != 1)
        {
            achievementsManager.UnlockAchievement(AchievementID.Played5);
            PlayerPrefs.SetInt("Played5Achieved", 1);
        }

        if (gamesPlayed >= 10 && PlayerPrefs.GetInt("Played10Achieved", 0) != 1)
        {
            achievementsManager.UnlockAchievement(AchievementID.Played10);
            PlayerPrefs.SetInt("Played10Achieved", 1);
        }

        if (gamesPlayed >= 20 && PlayerPrefs.GetInt("Played20Achieved", 0) != 1)
        {
            achievementsManager.UnlockAchievement(AchievementID.Played20);
            PlayerPrefs.SetInt("Played20Achieved", 1);
        }

        if (gamesPlayed >= 50 && PlayerPrefs.GetInt("Played50Achieved", 0) != 1)
        {
            achievementsManager.UnlockAchievement(AchievementID.Played50);
            PlayerPrefs.SetInt("Played50Achieved", 1);
        }

        if (gamesPlayed >= 100 && PlayerPrefs.GetInt("Played100Achieved", 0) != 1)
        {
            achievementsManager.UnlockAchievement(AchievementID.Played100);
            PlayerPrefs.SetInt("Played100Achieved", 1);
        }

        PlayerPrefs.Save();
    }


    public void RestartLevel()
    {
        Time.timeScale = 1;
        //if (PlayerPrefs.GetInt("RemoveAds") != 1)
        //  Mediation_Manager.mediation_Manager.ShowInterstitialAd();
        soundManager.PlayButtonSound();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void SetCoinsText()
    {
        for (int i = 0; i < AllCoinsText.Length; i++)
        {
            AllCoinsText[i].text = PlayerPrefs.GetInt("Coins").ToString();
        }

        for (int i = 0; i < AllGemsText.Length; i++)
        {
            AllGemsText[i].text = PlayerPrefs.GetInt("Gems").ToString();
        }

        // CLOUD SYNC for both
        SaveToCloud("Coins", PlayerPrefs.GetInt("Coins"));
        SaveToCloud("Gems", PlayerPrefs.GetInt("Gems"));
    }

    public void OpenSkinsShop()
    {
        soundManager.PlayButtonSound();
        SkinsMenu.SetActive(true);
        MainMenu.SetActive(false);
    }

    public void OpenBombShop()
    {
        soundManager.PlayButtonSound();
        UpgradesMenu.SetActive(true);
        MainMenu.SetActive(false);
        FindObjectOfType<BombManager>().ShowBomb();

    }

    public void CloseBomb()
    {
        soundManager.PlayButtonSound();
        UpgradesMenu.SetActive(false);
        MainMenu.SetActive(true);
        FindObjectOfType<BombManager>().HideBombs();
        //  if (PlayerPrefs.GetInt("RemoveAds") != 1)
        //    Mediation_Manager.mediation_Manager.ShowInterstitialAd();
    }

    public void OpenStore()
    {
        soundManager.PlayButtonSound();
        StoreMenu.SetActive(true);
        MainMenu.SetActive(false);
    }

    public void CloseStore()
    {
        soundManager.PlayButtonSound();
        StoreMenu.SetActive(false);
        MainMenu.SetActive(true);

        UpgradesMenu.SetActive(false);
        CharacterUnlock.SetActive(false);
        CharacterUpgrade.SetActive(false);

        shopCam.SetActive(false);
        inventoryCam.SetActive(false);
        startCam.SetActive(true);

        storeScroll.verticalNormalizedPosition = 1f;
    }


    public void SettingButtonPressed()
    {
        soundManager.PlayButtonSound();
        if (settingOpen)
        {
            settingOpen = false;
            SettingsPanel.SetActive(false);
        }
        else
        {
            settingOpen = true;
            SettingsPanel.SetActive(true);
        }
    }

    private void UpdateSoundIcon()
    {
        if (SoundImage != null)
        {
            SoundImage.sprite = soundOff ? SoundOffSprite : SoundOnSprite;
        }
    }

    private void UpdateBgMusicIcon()
    {
        if (BgImage != null)
        {
            BgImage.sprite = bgMusicOff ? BgMusicOffSprite : BgMusicOnSprite;
        }
    }

    public void ToggleSound()
    {
        soundOff = !soundOff;
        PlayerPrefs.SetInt("SoundOff", soundOff ? 1 : 0);
        PlayerPrefs.Save();
        UpdateSoundIcon();
        soundManager.CheckMusic();
        soundManager.PlayButtonSound();

    }

    public void ToggleBgMusic()
    {
        bgMusicOff = !bgMusicOff;
        PlayerPrefs.SetInt("BgMusicOff", bgMusicOff ? 1 : 0);
        PlayerPrefs.Save();
        UpdateBgMusicIcon();
        BGMusic.instance.CheckMusic();
        soundManager.PlayButtonSound();
    }

    private void LoadSettings()
    {
        soundOff = PlayerPrefs.GetInt("SoundOff", 0) == 1;
        bgMusicOff = PlayerPrefs.GetInt("BgMusicOff", 0) == 1;
    }

    public void NoAds()
    {
        soundManager.PlayButtonSound();
        NoAdsPanel.SetActive(true);
    }

    public void CloseNoAdsButton()
    {
        soundManager.PlayButtonSound();
        NoAdsPanel.SetActive(false);
        adWheel.StartRotatingAgain();
    }

    public void OpenLeaderboards()
    {
        leaderboardsPanel.SetActive(true);
    }

    public void CloseLeaderboards()
    {
        leaderboardsPanel.SetActive(false);
    }

    public void OpenChangeNamePanel()
    {
        changeNamePanel.SetActive(true);
    }

    public void CloseChangeNamePanel()
    {
        changeNamePanel.SetActive(false);
    }

    public void EnableInAppWindow(bool _enable)
    {
        soundManager.PlayButtonSound();
        InAppWindow.SetActive(_enable);
        if (_enable)
        {
            //Mediation_Manager.mediation_Manager.HideBannerAd();
        }
        else
        {
            if (PlayerPrefs.GetInt("RemoveAds") != 1 && !levelWindow.activeInHierarchy)
                Mediation_Manager.mediation_Manager.ShowBannerAd();
        }
    }
    public void EnableLevelsWindow(bool _enable)
    {
        soundManager.PlayButtonSound();
        levelWindow.SetActive(_enable);
        if (_enable)
        {
            Mediation_Manager.mediation_Manager.HideBannerAd();
        }
        else
        {
            if (PlayerPrefs.GetInt("OK") != 1)
                Mediation_Manager.mediation_Manager.ShowBannerAd();
        }
    }

    public void ShowRewardedAds()
    {
        soundManager.PlayButtonSound();
        if (PlayerPrefs.GetInt("RemoveAds") != 1)
        {
            Mediation_Manager.mediation_Manager.ShowRewardedAd(RewardType.Coins);
        }
        // FindObjectOfType<RewardedAds>().ShowAd(RewardType.Coins);
    }

    public void RemoveAds()
    {
        PlayerPrefs.SetInt("RemoveAds", 1);
        CheckAdsRemoved();
    }

    void CheckAdsRemoved()
    {
        if (PlayerPrefs.GetInt("RemoveAds") == 1)
        {
            RemoveAdsButton.SetActive(false);
        }
    }

    public void PurchaseComplete(Product product)
    {
        if (product.definition.id == "com.gamexstudio.bombbashroyale.removeads")
        {
            RemoveAds();
            CheckAdsRemoved();
            AchievementsManager achievementsManager = FindObjectOfType<AchievementsManager>();
            PlayerPrefs.SetInt("PurchaseCount", PlayerPrefs.GetInt("PurchaseCount") + 1);
            int purchaseCount = PlayerPrefs.GetInt("PurchaseCount");
            if (purchaseCount >= 1 && PlayerPrefs.GetInt("PurchaseAchieved", 0) != 1)
            {
                achievementsManager.UnlockAchievement(AchievementID.PurchaseSomething);
                PlayerPrefs.SetInt("PurchaseAchieved", 1); 
            }

        }
    }

    public void OnPurchaseFailed(Product product, PurchaseFailureDescription p)
    {
        Debug.Log(product.definition.id + " failed because" + p);
    }

    public void BackButtonPressed()
    {
        soundManager.PlayButtonSound();
        PauseGame();
    }


    void PauseGame()
    {
        GameManager.instance.joystick.gameObject.SetActive(false);

        Time.timeScale = 0;
        PauseCanvas.SetActive(true);
    }

    public void ReloadGame()
    {
        soundManager.PlayButtonSound();
        Time.timeScale = 1;

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void Resume()
    {
        GameManager.instance.joystick.gameObject.SetActive(true);
        soundManager.PlayButtonSound();
        Time.timeScale = 1;
        PauseCanvas.SetActive(false);
    }

    public void QuitGame()
    {
        soundManager.PlayButtonSound();
        Application.Quit();
    }

    public void ShowWatchAdsToUpgradeWindow(bool _isPower)
    {
        isPower = _isPower;
        WatchAdsToUpgradeWindow.SetActive(true);
    }

    public void WatchAdsToUpgrade()
    {
        soundManager.PlayButtonSound();
        if (isPower)
            FindObjectOfType<Mediation_Manager>().ShowRewardedAd(RewardType.PowerUpgrade);
        else
            FindObjectOfType<Mediation_Manager>().ShowRewardedAd(RewardType.HealthUpgrade);
    }

    public void UpgradeAfterWatchAds()
    {
        WatchAdsToUpgradeWindow.SetActive(false);

        if (isPower)
        {
            FindObjectOfType<PowerProgression>().IncreaseByForce();
        }
        else
        {
            FindObjectOfType<HealthProgression>().IncreaseByForce();
        }
    }

    public void CloseUpgradeWindow()
    {
        soundManager.PlayButtonSound();
        WatchAdsToUpgradeWindow.SetActive(false);
    }

    public void LoadGame(string _name)
    {
        soundManager.PlayButtonSound();
        SceneManager.LoadScene(_name);
    }

    public void ShowWaveText(int waveNumber)
    {
        waveText.gameObject.SetActive(true);
        waveText.text = "Wave " + waveNumber;
        waveText.transform.localScale = Vector3.zero;

        waveText.DOFade(1f, 0.5f);
        waveText.transform.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBack);

        DOVirtual.DelayedCall(2f, () =>
        {
            waveText.DOFade(0f, 0.5f);
            waveText.transform.DOScale(Vector3.zero, 0.5f).OnComplete(() =>
            {
                waveText.gameObject.SetActive(false);
            });
        });
    }

    public void OnCloseStoreButton()
    {
        CloseStore();
    }

    public void OpenTutorialScene()
    {
        SceneManager.LoadScene("Training New");
    }

    public void DisableReviveButton()
    {
        if (reviveButton != null)
        {
            reviveButton.interactable = false;
        }
    }
    public void EnableReviveButton()
    {
        if (reviveButton != null)
        {
            reviveButton.interactable = true;
            reviveButton.gameObject.SetActive(true);
        }
    }

    public void AddKeys(int amount)
    {
        int currentKeys = PlayerPrefs.GetInt("Keys", 0);
        currentKeys += amount;
        PlayerPrefs.SetInt("Keys", currentKeys);
        PlayerPrefs.Save();

        if (keysText != null)
            keysText.text = currentKeys.ToString();

        SaveToCloud("Keys", currentKeys);
    }

    public void UpdateKeysText()
    {
        int keys = PlayerPrefs.GetInt("Keys", 0);
        if (keysText != null)
            keysText.text = keys.ToString();
    }
}
