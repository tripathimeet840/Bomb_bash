using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
//using Firebase.Analytics;


public enum PriceType { Coins, Gems }
[System.Serializable]
public class BombInfo
{
    public GameObject BombVisual; 
    public bool Unlocked; 
    public int Price;
    
    public PriceType priceType;
}

public class BombManager : MonoBehaviour
{
    SoundManager soundManager;
    private BombInfo[] bombInfos; 
    private int currentBombIndex = 0; 
    public TMP_Text bombInfoText; 

    private PlayerBombsHandler playerBombsHandler;
    Animator playerAnim;
    public TMP_SpriteAsset coinSpriteAsset; 
    public TMP_SpriteAsset gemSpriteAsset;

    private void Start()
    {
        soundManager = SoundManager.soundManager;
    }

    public void ShowBomb()
    {
        if (playerBombsHandler == null)
        {
            playerBombsHandler = FindObjectOfType<PlayerBombsHandler>();
            playerAnim = playerBombsHandler.GetComponent<Animator>();
           // playerAnim.SetLayerWeight(1, 1);
        }

        if (bombInfos == null || bombInfos.Length == 0)
        {
            bombInfos = playerBombsHandler.allBombs;

            for (int i = 0; i < bombInfos.Length; i++)
            {
                bombInfos[i].Unlocked = PlayerPrefs.GetInt($"BombUnlocked_{i}", (i == 0) ? 1 : 0) == 1;
            }

            currentBombIndex = PlayerPrefs.GetInt("BombSkin", 0);
        }

        if (bombInfos == null || bombInfos.Length == 0) return;

        for (int i = 0; i < bombInfos.Length; i++)
        {
            bombInfos[i].BombVisual.SetActive(false);
        }

        bombInfos[currentBombIndex].BombVisual.SetActive(true);


        if (bombInfos[currentBombIndex].Unlocked)
        {
            bombInfoText.text = "Use";
        }
        else
        {
            string priceText;

            if (bombInfos[currentBombIndex].priceType == PriceType.Coins)
            {
                bombInfoText.spriteAsset = coinSpriteAsset;
                priceText = $"{bombInfos[currentBombIndex].Price} <sprite=0>"; 
            }
            else
            {
                bombInfoText.spriteAsset = gemSpriteAsset;
                priceText = $"{bombInfos[currentBombIndex].Price} <sprite=0>"; 
            }

            bombInfoText.text = $"{priceText}";
        }
    }


    public void NextBomb()
    {
        soundManager.PlayButtonSound();
        currentBombIndex = (currentBombIndex + 1) % bombInfos.Length;
        ShowBomb();
    }

    public void PreviousBomb()
    {
        soundManager.PlayButtonSound();
        currentBombIndex = (currentBombIndex - 1 + bombInfos.Length) % bombInfos.Length;
        ShowBomb();
    }

    private void InitializeBombs()
    {
        bombInfos = playerBombsHandler.allBombs;

        for (int i = 0; i < bombInfos.Length; i++)
        {
            bombInfos[i].Unlocked = PlayerPrefs.GetInt($"BombUnlocked_{i}", (i == 0) ? 1 : 0) == 1;
        }
    }

    public void UnlockCurrentBomb()
    {
        if (!bombInfos[currentBombIndex].Unlocked)
        {
            if (bombInfos[currentBombIndex].priceType == PriceType.Coins)
            {
                if (PlayerPrefs.GetInt("Coins") >= bombInfos[currentBombIndex].Price)
                {
                    bombInfos[currentBombIndex].Unlocked = true;
                    PlayerPrefs.SetInt("Coins", PlayerPrefs.GetInt("Coins") - bombInfos[currentBombIndex].Price);
                    PlayerPrefs.SetInt($"BombUnlocked_{currentBombIndex}", 1);
                    PlayerPrefs.SetInt("BombSkin", currentBombIndex);

                    //FirebaseAnalytics.LogEvent("Bomb Skin Unlocked", new Parameter("Bomb SKin Index: ", currentBombIndex));

                }
                else
                {
                    UIManager.instance.EnableInAppWindow(true);
                }
            }
            else
            {
                if (PlayerPrefs.GetInt("Gems") >= bombInfos[currentBombIndex].Price)
                {
                    bombInfos[currentBombIndex].Unlocked = true;
                    PlayerPrefs.SetInt("Gems", PlayerPrefs.GetInt("Gems") - bombInfos[currentBombIndex].Price);
                    PlayerPrefs.SetInt($"BombUnlocked_{currentBombIndex}", 1);
                    PlayerPrefs.SetInt("BombSkin", currentBombIndex);
                    //FirebaseAnalytics.LogEvent("Bomb Skin Unlocked", new Parameter("Bomb SKin Index: ", currentBombIndex));

                }
                else
                {
                    UIManager.instance.EnableInAppWindow(true);
                }
            }

            ShowBomb();
        }
        else
        {
            PlayerPrefs.SetInt("BombSkin", currentBombIndex);
            UIManager.instance.CloseBomb();
        }

        UIManager.instance.SetCoinsText();
    }

    public void HideBombs()
    {
        for (int i = 0; i < bombInfos.Length; i++)
        {
            bombInfos[i].BombVisual.SetActive(false);
        }

        playerAnim.SetLayerWeight(1, 0);
    }
}

