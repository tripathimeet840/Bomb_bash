using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;

public class EpicChestReward : MonoBehaviour
{
    [Header("Reward Ranges")]
    public int minCoins = 100;
    public int maxCoins = 500;
    public int minGems = 10;
    public int maxGems = 50;
    public int minXP = 50;
    public int maxXP = 200;

    [Header("UI References")]
    public GameObject rewardCanvas;
    public TMP_Text rewardValueText;
    public Image rewardIcon;
    public Sprite coinSprite;
    public Sprite gemSprite;
    public Sprite xpSprite;
    public GameObject xpIcon;

    [Header("Character Rewards")]
    public Sprite[] characterSprites;
    public int[] characterIndices;

    [Header("Saved Totals Display (Optional)")]
    public TMP_Text totalCoinsText;
    public TMP_Text totalGemsText;
    public TMP_Text totalKeysText;

    [Header("References")]
    public PlayerLevelSystem playerLevelSystem;
    public ShopManager shopManager;

    private int coins;
    private int gems;
    private int keys;
    private int chestOpenCount;

    [Header("Messages")]
    public string notEnoughKeysMessage = "Not enough keys!";
    public TMP_Text messageText;

    private void Start()
    {
        coins = PlayerPrefs.GetInt("Coins", 0);
        gems = PlayerPrefs.GetInt("Gems", 0);
        keys = PlayerPrefs.GetInt("Keys", 0);
        chestOpenCount = PlayerPrefs.GetInt("EpicChestOpenCount", 0);

        if (playerLevelSystem == null)
            playerLevelSystem = FindObjectOfType<PlayerLevelSystem>();

        if (shopManager == null)
            shopManager = FindObjectOfType<ShopManager>();

        UpdateTotalUI();
    }

    private void Update()
    {
        coins = PlayerPrefs.GetInt("Coins", 0);
        gems = PlayerPrefs.GetInt("Gems", 0);
        keys = PlayerPrefs.GetInt("Keys", 0);
        chestOpenCount = PlayerPrefs.GetInt("EpicChestOpenCount", 0);
        UpdateTotalUI();
    }

    public void OpenSpecialChest()
    {
        xpIcon.SetActive(false);

        if (keys < 1)
        {
            ShowMessageTemporary(notEnoughKeysMessage);
            return;
        }

        keys -= 1;
        PlayerPrefs.SetInt("Keys", keys);
        UpdateTotalUI();

        chestOpenCount++;
        PlayerPrefs.SetInt("EpicChestOpenCount", chestOpenCount);

        int rewardAmount = 0;
        bool gaveCharacter = false;

        if (chestOpenCount % 10 == 0 && characterSprites.Length > 0)
        {
            gaveCharacter = true;
            xpIcon.SetActive(false);
            rewardIcon.enabled = true;

            int charIndex = Random.Range(0, characterSprites.Length);
            rewardIcon.sprite = characterSprites[charIndex];

            if (charIndex < characterIndices.Length)
            {
                int indexToUnlock = characterIndices[charIndex];
                UnlockCharacterByIndex(indexToUnlock);
                rewardValueText.text = "Unlocked Character!";
            }
            else
            {
                rewardValueText.text = "Character Reward Error!";
            }
        }
        else
        {
            int rewardType = Random.Range(0, 3);

            if (rewardType == 0)
            {
                rewardAmount = Random.Range(minCoins, maxCoins + 1);
                coins += rewardAmount;
                PlayerPrefs.SetInt("Coins", coins);
                rewardIcon.enabled = true;
                rewardIcon.sprite = coinSprite;
                rewardValueText.text = "+" + rewardAmount;
            }
            else if (rewardType == 1)
            {
                rewardAmount = Random.Range(minGems, maxGems + 1);
                gems += rewardAmount;
                PlayerPrefs.SetInt("Gems", gems);
                rewardIcon.enabled = true;
                rewardIcon.sprite = gemSprite;
                rewardValueText.text = "+" + rewardAmount;
            }
            else
            {
                rewardAmount = Random.Range(minXP, maxXP + 1);
                rewardIcon.enabled = false;
                xpIcon.SetActive(true);
                rewardValueText.text = "+" + rewardAmount + " XP";

                if (playerLevelSystem != null)
                    playerLevelSystem.AddXP(rewardAmount);
            }
        }

        PlayerPrefs.Save();
        rewardCanvas.SetActive(true);
        UpdateTotalUI();

        Debug.Log(gaveCharacter
            ? $"Special reward! Character unlocked on chest #{chestOpenCount}"
            : $"Opened chest #{chestOpenCount}");
    }

    private void UnlockCharacterByIndex(int index)
    {
        if (shopManager == null)
        {
            Debug.LogWarning("ShopManager missing — cannot unlock character.");
            return;
        }

        if (PlayerPrefs.GetInt("CharacterUnlocked_" + index, index == 0 ? 1 : 0) == 1)
        {
            Debug.Log($"Character #{index} already unlocked — giving fallback coins instead.");
            coins += 300;
            PlayerPrefs.SetInt("Coins", coins);
            rewardIcon.sprite = coinSprite;
            rewardValueText.text = "+300 Coins";
            return;
        }

        shopManager.UnlockCharacter(index);
        Debug.Log($"Unlocked character index {index} ({shopManager.characters[index].name})!");
    }

    private void UpdateTotalUI()
    {
        if (totalCoinsText != null) totalCoinsText.text = coins.ToString();
        if (totalGemsText != null) totalGemsText.text = gems.ToString();
        if (totalKeysText != null) totalKeysText.text = keys.ToString();
    }

    public void CloseRewardCanvas()
    {
        rewardCanvas.SetActive(false);
    }

    public void Open10Chests()
    {
        int keysNeeded = 10;

        if (keys < keysNeeded)
        {
            ShowMessageTemporary(notEnoughKeysMessage);
            return;
        }

        keys -= keysNeeded;
        PlayerPrefs.SetInt("Keys", keys);
        UpdateTotalUI();

        int totalCoins = 0;
        int totalGems = 0;
        int totalXP = 0;
        bool unlockedCharacter = false;

        for (int i = 0; i < 10; i++)
        {
            chestOpenCount++;
            PlayerPrefs.SetInt("EpicChestOpenCount", chestOpenCount);

            if (chestOpenCount % 10 == 0 && characterSprites.Length > 0)
            {
                unlockedCharacter = true;
                int charIndex = Random.Range(0, characterSprites.Length);
                if (charIndex < characterIndices.Length)
                    UnlockCharacterByIndex(characterIndices[charIndex]);
            }
            else
            {
                int rewardType = Random.Range(0, 3);
                int rewardAmount = 0;

                if (rewardType == 0)
                {
                    rewardAmount = Random.Range(minCoins, maxCoins + 1);
                    totalCoins += rewardAmount;
                }
                else if (rewardType == 1)
                {
                    rewardAmount = Random.Range(minGems, maxGems + 1);
                    totalGems += rewardAmount;
                }
                else
                {
                    rewardAmount = Random.Range(minXP, maxXP + 1);
                    totalXP += rewardAmount;
                    if (playerLevelSystem != null)
                        playerLevelSystem.AddXP(rewardAmount);
                }
            }
        }

        coins += totalCoins;
        gems += totalGems;
        PlayerPrefs.SetInt("Coins", coins);
        PlayerPrefs.SetInt("Gems", gems);
        PlayerPrefs.Save();

        rewardValueText.text = (unlockedCharacter ? "Character Unlocked!\n" : "") +
                               $"+{totalCoins} Coins\n+{totalGems} Gems\n+{totalXP} XP";

        rewardIcon.sprite = coinSprite;
        rewardCanvas.SetActive(true);
        UpdateTotalUI();

        Debug.Log($"Opened 10 Epic Chests. Total coins: {totalCoins}, gems: {totalGems}, XP: {totalXP}");
    }

    private void ShowMessageTemporary(string msg)
    {
        if (messageText != null)
        {
            messageText.text = msg;
            messageText.gameObject.SetActive(true);
            StopAllCoroutines();
            StartCoroutine(HideMessageAfterSeconds(2f));
        }
        rewardCanvas.SetActive(false);
    }

    private IEnumerator HideMessageAfterSeconds(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        if (messageText != null)
            messageText.gameObject.SetActive(false);
    }

    public void OpenChestWithAd()
    {
        if (Mediation_Manager.mediation_Manager != null)
        {
            Mediation_Manager.mediation_Manager.ShowRewardedAd(RewardType.RewardedEpicChest);
        }
        else
        {
            Debug.LogWarning("Mediation_Manager not found!");
        }
    }

}
