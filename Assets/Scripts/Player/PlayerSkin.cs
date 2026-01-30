using System.Collections;
using TMPro;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class PlayerSkin : MonoBehaviour
{
    public GameObject[] AllSkins;
    int totalSkins;
    [SerializeField]int currentSkinIndex;
    float currentPosX, RightPosX, LeftPosX;
    [SerializeField]GameObject Aimer;
    [SerializeField]Joystick joystick;
    [SerializeField] GameObject SkinMenu,MainMenu;
    [SerializeField] Image[] AllSkinsColor;
    [SerializeField] GameObject[] AllSelcteables;
    SoundManager soundManager;
    PowerProgression powerProgression;
    HealthProgression healthProgression;
    public TMP_Text unlockButtonText;
    Vector3 playerStartPos;
    public TMP_SpriteAsset CoinsAsset, GemAsset;
    public BombPrice playerPriceItem;
    private void Awake()
    {
        healthProgression = FindObjectOfType<HealthProgression>();
        powerProgression=FindObjectOfType<PowerProgression>();
        currentSkinIndex = PlayerPrefs.GetInt("PlayerIndex");
        
    }

    // Start is called before the first frame update
    void Start()
    {

        if (!PlayerPrefs.HasKey("SkinUnlocked0"))
        {
            PlayerPrefs.SetInt("SkinUnlocked0", 1);
        }
        SetPrice();

        ShowCharacter(currentSkinIndex);
        totalSkins = AllSkins.Length;
        currentPosX = AllSkins[0].transform.position.x;
        RightPosX = currentPosX + 10f;
        LeftPosX = currentPosX - 10f;
        soundManager = SoundManager.soundManager;
    }


    void SetPrice()
    {
        for(int i = 0; i < AllSkins.Length; i++)
        {
            PlayerSkinsColor playerSkins = AllSkins[i].GetComponent<PlayerSkinsColor>();
            playerSkins.Price = playerPriceItem.currencyItems[i].price;
            playerSkins.type = playerPriceItem.currencyItems[i].curreny;
        }
    }

   public void NextCharacter()
    {
        int lastIndex = currentSkinIndex;
        currentSkinIndex++;
        if (currentSkinIndex > totalSkins-1)
            currentSkinIndex = 0;
        ShowCharacter(currentSkinIndex, lastIndex, true);
        soundManager.PlayButtonSound();

    }

    public void PreviousCharcter()
    {
        int lastIndex = currentSkinIndex;
        currentSkinIndex--;
        if (currentSkinIndex <0)
            currentSkinIndex =totalSkins-1;
        ShowCharacter(currentSkinIndex, lastIndex, false);
        soundManager.PlayButtonSound();

    }


    public void ShowCharacter(int Index)
    {
        for(int i = 0; i < AllSkins.Length; i++)
        {
            AllSkins[i].SetActive(false);
        }

        AllSkins[Index].SetActive(true);
        playerStartPos = AllSkins[Index].transform.position;
        UpdateUnlockButton(Index);
      //  UnlockDifferentSkinsForCharacter();
    }

    public void ShowCharacter(int currentIndex, int lastIndex,bool nextCharacter)
    {
        int skinIndex = PlayerPrefs.GetInt(currentIndex + "SkinIndex");
       // UnlockDifferentSkinsForCharacter();
        joystick = GameManager.instance.joystick;
        joystick.gameObject.SetActive(true);
        Aimer.SetActive(true);

        float endPosX = nextCharacter ? RightPosX : LeftPosX;
        float startPosX = nextCharacter ? LeftPosX : RightPosX;

        AllSkins[lastIndex].transform.DOMoveX(endPosX, 0.25f).SetEase(Ease.Linear).OnComplete(() =>
        {
            AllSkins[lastIndex].SetActive(false);
        });

        Vector3 temp = AllSkins[currentIndex].transform.position;
        AllSkins[currentIndex].transform.position = new Vector3(startPosX, temp.y, temp.z);
        AllSkins[currentIndex].SetActive(true);
        AllSkins[currentIndex].transform.DOMoveX(currentPosX, 0.25f).SetEase(Ease.Linear);
        UpdateUnlockButton(currentIndex);
    }


   /* public void SetCharacterSkinIndex(int _skinNumber)
    {
       
        PlayerPrefs.SetInt((currentSkinIndex + "SkinIndex"), _skinNumber-1);
        UnlockDifferentSkinsForCharacter();
        soundManager.PlayButtonSound();
        UpdateUnlockButton();
    }

   void UnlockDifferentSkinsForCharacter()
    {
        for (int i = 0; i < 5; i++)
        {
            AllSkins[currentSkinIndex].transform.GetChild(i).gameObject.SetActive(false);
        }
       
        AllSkins[currentSkinIndex].transform.GetChild(PlayerPrefs.GetInt(currentSkinIndex + "SkinIndex")).gameObject.SetActive(true);

        PlayerSkinsColor player = AllSkins[currentSkinIndex].GetComponent<PlayerSkinsColor>();

        for (int i = 0; i < AllSkinsColor.Length; i++)
        {
            AllSkinsColor[i].color = player.Skins[i];
        }
        for (int i = 0; i < AllSelcteables.Length; i++)
        {
            AllSelcteables[i].SetActive(false);
        }
        AllSelcteables[PlayerPrefs.GetInt(currentSkinIndex + "SkinIndex")].SetActive(true);

    }
   */
    public void Unlock()
    {
        UnlockCharacter(currentSkinIndex);
       /* SkinMenu.SetActive(false);
        MainMenu.SetActive(true);
        PlayerPrefs.SetInt("PlayerIndex", currentSkinIndex);
        soundManager.PlayButtonSound();
        powerProgression.UpdateUI();
        healthProgression.UpdateUI();*/
       
    }

    void CheckAchievements()
    {
        AchievementsManager achievementsManager = FindObjectOfType<AchievementsManager>();
        PlayerPrefs.SetInt("CharacterCount", PlayerPrefs.GetInt("CharacterCount") + 1);
        int charactersUnlocked = PlayerPrefs.GetInt("CharacterCount");
        // Unlock achievement for unlocking the first character
        if (charactersUnlocked >= 1 && PlayerPrefs.GetInt("Character1Achieved", 0) != 1)
        {
            achievementsManager.UnlockAchievement(AchievementID.UnlockCharacter); // Unlock "Character 1" achievement
            PlayerPrefs.SetInt("Character1Achieved", 1); // Mark achievement as unlocked
        }

        // Unlock achievement for unlocking 5 characters
        if (charactersUnlocked >= 5 && PlayerPrefs.GetInt("Character5Achieved", 0) != 1)
        {
            achievementsManager.UnlockAchievement(AchievementID.Unlock5Chracater); // Unlock "Character 5" achievement
            PlayerPrefs.SetInt("Character5Achieved", 1); // Mark achievement as unlocked
        }

        // Save PlayerPrefs to persist data
        PlayerPrefs.Save();
    }




    public void BackButtonPressed()
    {
        soundManager.PlayButtonSound();
        for(int i = 0; i < AllSkins.Length; i++)
        {
            AllSkins[i].SetActive(false);
        }

        int index = PlayerPrefs.GetInt("PlayerIndex");
        AllSkins[index].SetActive(true);
        currentSkinIndex = index;
        MainMenu.SetActive(true);
        SkinMenu.SetActive(false);
        AllSkins[index].transform.position = playerStartPos;
        //if (PlayerPrefs.GetInt("RemoveAds") != 1)
          //  Mediation_Manager.mediation_Manager.ShowInterstitialAd();

    }

    bool IsSkinUnlocked(int index)
    {
        return PlayerPrefs.GetInt("SkinUnlocked" + index, 0) == 1;
    }

    public void UnlockCharacter(int index)
    {
        if (!IsSkinUnlocked(currentSkinIndex))
        {
            // Assuming you have a method to deduct currency and handle purchase logic
            bool purchaseSuccessful = PurchaseSkin(currentSkinIndex); // Replace 100 with your actual amount or variable
            if (purchaseSuccessful)
            {
                PlayerPrefs.SetInt("SkinUnlocked" + currentSkinIndex, 1);
                CheckAchievements();
                UpdateUnlockButton(index);
                //FirebaseAnalytics.LogEvent("Character Unlocked", new Parameter("Character Index : ", index));

            }
            else
            {
                UIManager.instance.EnableInAppWindow(true);
            }
        }
        else
        {
            //Unlock();
            SkinMenu.SetActive(false);
            MainMenu.SetActive(true);
            PlayerPrefs.SetInt("PlayerIndex", currentSkinIndex);
            soundManager.PlayButtonSound();
            powerProgression.UpdateUI();
            healthProgression.UpdateUI();
        }
    }
    bool PurchaseSkin(int skinIndex)
    {
        // Implement your purchase logic here, return true if successful
        PlayerSkinsColor playerSkinsColor = AllSkins[skinIndex].GetComponent<PlayerSkinsColor>();
        int amount = playerSkinsColor.Price;
        if (playerSkinsColor.type == PriceType.Coins)
        {
            if (PlayerPrefs.GetInt("Coins") >= amount)
            {
                PlayerPrefs.SetInt("Coins", PlayerPrefs.GetInt("Coins") - amount);
                UIManager.instance.SetCoinsText();
                return true;
            }
            else
            {
                return false;
            }
        }
        else
        {
            if (PlayerPrefs.GetInt("Gems") >= amount)
            {
                PlayerPrefs.SetInt("Gems", PlayerPrefs.GetInt("Gems") - amount);
                UIManager.instance.SetCoinsText();
                return true;
            }
            else
            {
                return false;
            }
        }
    }

    // Method to update the unlock button text
    void UpdateUnlockButton(int index)
    {
        if (IsSkinUnlocked(currentSkinIndex))
        {
            if(unlockButtonText!=null)
            unlockButtonText.text = "Use";
        }
        else
        {
            if (unlockButtonText != null)
            {

                PlayerSkinsColor playerSkinsColor = AllSkins[index].GetComponent<PlayerSkinsColor>();
                int price = playerSkinsColor.Price;
                if(playerSkinsColor.type== PriceType.Coins)
                { 
                 
                    unlockButtonText.spriteAsset = CoinsAsset;
                }
                else
                {
                    unlockButtonText.spriteAsset = GemAsset;

                }


                unlockButtonText.text = $"Buy {price}<sprite=0>";
            }
        }
    }
}
