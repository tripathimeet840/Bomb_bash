using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ChestReward : MonoBehaviour
{
    [Header("UI")]
    public GameObject rewardCanvasCoins;
    public GameObject rewardCanvasGems;
    public TMP_Text rewardText;
    public Image rewardIcon;
    public Sprite coinSprite;
    public Sprite gemSprite;
    public Sprite xpSprite;

    [Header("Player Totals")]
    public TMP_Text coinsText;
    public TMP_Text gemsText;
    public TMP_Text xpText;

    private int coins;
    private int gems;
    private int xp;

    private void Start()
    {
        // Load saved values
        coins = PlayerPrefs.GetInt("Coins", 0);
        gems = PlayerPrefs.GetInt("Gems", 0);
        xp = PlayerPrefs.GetInt("XP", 0);

        // Update UI
        coinsText.text = coins.ToString();
        gemsText.text = gems.ToString();
        xpText.text = xp.ToString();
    }

    public void GiveCoins(int amount)
    {
        coins += amount;
        PlayerPrefs.SetInt("Coins", coins);
        coinsText.text = $"{coins}";

        ShowRewardUI(amount, "Coins");
        rewardIcon.sprite = coinSprite;
        rewardCanvasCoins.SetActive(true);
    }

    public void GiveGems(int amount)
    {
        gems += amount;
        PlayerPrefs.SetInt("Gems", gems);
        gemsText.text = $"{gems}";

        ShowRewardUI(amount, "Gems");
        rewardIcon.sprite = gemSprite;
        rewardCanvasGems.SetActive(true);
    }

    public void GiveXP(int amount)
    {
        xp += amount;
        PlayerPrefs.SetInt("XP", xp);
        xpText.text = $"{xp}";

        ShowRewardUI(amount, "XP");
        rewardIcon.sprite = xpSprite;
    }

    private void ShowRewardUI(int amount, string type)
    {
        rewardText.text = $"+{amount} {type}";
    }

    public void CloseRewardCanvas()
    {
        rewardCanvasCoins.SetActive(false);
        rewardCanvasGems.SetActive(false);
    }
}
