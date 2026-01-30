using UnityEngine.UI;
using UnityEngine;
using UnityEngine.Purchasing;
using TMPro;
using UnityEngine.Purchasing.Extension;

public class InAppManager : MonoBehaviour
{
    public Button previouButton, nextButton;
    int CurrentIndex;
    public int totalElements;
    SoundManager soundManager;
    public Image buyImage;
    public TMP_Text buyText;
    public Detail[] buyOptions;
    //[SerializeField] Image[] PagesDot;
    [SerializeField] Sprite BlueDot, WhiteDot;
    [SerializeField] CodelessIAPButton iapButton;
    [SerializeField] TMP_SpriteAsset coinSprite, GemSprite;

    [System.Serializable]
    public struct Detail
    {
        public string _description;
        public Sprite _image;
        public string _purchaseId;
        public enum PriceType { Coins,Gem}
        public PriceType type;
        public int _coinsGrant;
    }

    void Start()
    {
        soundManager = SoundManager.soundManager;
        totalElements -= 1;
        previouButton.onClick.AddListener(ShowPreviousBuyOption);
        nextButton.onClick.AddListener(ShowNextBuyOption);
    }

    private void OnEnable()
    {
        CurrentIndex = 0;
        CheckButtonState();
    }

    void ShowNextBuyOption()
    {
        soundManager.PlayButtonSound();
        CurrentIndex++;
        CheckButtonState();
    }

    void ShowPreviousBuyOption()
    {
        soundManager.PlayButtonSound();
        CurrentIndex--;
        CheckButtonState();
    }

    void CheckButtonState()
    {
        if (CurrentIndex == 0)
        {
            nextButton.interactable = true;
            previouButton.interactable = false;
        }
        else
        {
            previouButton.interactable = true;
        }

        if (CurrentIndex == totalElements)
        {
            nextButton.interactable = false;
            previouButton.interactable = true;
        }
        else
        {
            nextButton.interactable = true;
        }
        ShowCurrentOptionDescription();
    }

    void ShowCurrentOptionDescription()
    {
        Detail selectedDetail = buyOptions[CurrentIndex];

        if (selectedDetail.type == Detail.PriceType.Coins)
        {
            buyText.spriteAsset = coinSprite;
        }
        else
        {
            buyText.spriteAsset = GemSprite;
        }

        buyText.text = selectedDetail._description + "<sprite=0>";
        buyImage.sprite = selectedDetail._image;

       /* for (int i = 0; i < PagesDot.Length; i++)
        {
            PagesDot[i].sprite = i == CurrentIndex ? WhiteDot : BlueDot;
        }*/
        UpdateIAPButton(selectedDetail._purchaseId);
    }

    void UpdateIAPButton(string productId)
    {
        // Set the productId of the IAPButton
        iapButton.productId = productId;

        // Retrieve product information
        var product = CodelessIAPStoreListener.Instance.GetProduct(productId);

        if (product != null)
        {
            // Update the button text to show the price
            string price = product.metadata.localizedPriceString;
            iapButton.GetComponentInChildren<Text>().text =  price;
        }
        else
        {
            Debug.LogError($"Product {productId} is not found in the catalog.");
        }
    }

    public void OnPurchseComplete(Product product)
    {
        for(int i = 0; i < buyOptions.Length; i++)
        {
            if (product.definition.id == buyOptions[i]._purchaseId)
            {

                if (buyOptions[i].type == Detail.PriceType.Coins)
                {
                    PlayerPrefs.SetInt("Coins", PlayerPrefs.GetInt("Coins") + buyOptions[i]._coinsGrant);
                }
                else
                {
                    PlayerPrefs.SetInt("Gems", PlayerPrefs.GetInt("Gems") + buyOptions[i]._coinsGrant);
                }
                
                UIManager.instance.SetCoinsText();


                AchievementsManager achievementsManager = FindObjectOfType<AchievementsManager>();
                PlayerPrefs.SetInt("PurchaseCount", PlayerPrefs.GetInt("PurchaseCount") + 1);
                int purchaseCount = PlayerPrefs.GetInt("PurchaseCount");
                if (purchaseCount >= 1 && PlayerPrefs.GetInt("PurchaseAchieved", 0) != 1)
                {
                    achievementsManager.UnlockAchievement(AchievementID.PurchaseSomething); // Unlock "Character 1" achievement
                    PlayerPrefs.SetInt("PurchaseAchieved", 1); // Mark achievement as unlocked
                }


            }
        }
    }


    public void TestIncrease()
    {
        PlayerPrefs.SetInt("Coins", PlayerPrefs.GetInt("Coins") +1000);
        UIManager.instance.SetCoinsText();
    }

    public void OnPurchaseFailed(Product product, PurchaseFailureDescription p)
    {
        Debug.Log(product.definition.id + " failed because" + p);
    }
}
