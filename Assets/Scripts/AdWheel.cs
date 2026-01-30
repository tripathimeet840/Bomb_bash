using UnityEngine.UI;
using UnityEngine;
using System.Collections;
using DG.Tweening;

public class AdWheel : MonoBehaviour
{
    public RectTransform arrowTransform; // Reference to the arrow RectTransform
    public float angleRange = 75f; // Maximum angle in degrees (from -75 to 75)
    public float frequency = 1f; // Frequency of the oscillation
    public Transform[] multipliers; // Array of multipliers (2x, 4x, 8x)

    private float elapsedTime = 0f;
    private bool isSpinning = true;
    public float speed;
    int Coin;
    [SerializeField] Text coinsText;
    Vector3 startScale;
  

    private void Start()
    {
        startScale = transform.localScale;
    }

    public void SetCoinsAmt(int _amt)
    {
        Coin = _amt;
        coinsText.text = Coin.ToString();
        PlayerPrefs.SetInt("Coins", PlayerPrefs.GetInt("Coins") + Coin);
      
    }

    void Update()
    {
        if (isSpinning)
        {
            // Calculate the oscillation angle using a sine wave
            elapsedTime += Time.deltaTime*speed;
            float angle = Mathf.Sin(elapsedTime * frequency) * angleRange;

            // Apply the rotation to the arrow
            arrowTransform.localRotation = Quaternion.Euler(0, 0, angle);
            ChangePlayerCoinsText();
        }
    }

    public void StopRotation()
    {
        SoundManager.soundManager.PlayButtonSound();
        isSpinning = false;
        transform.DOScale(startScale + new Vector3(0.3f, 0.3f, 0.3f), 0.2f).SetEase(Ease.Linear).OnComplete(() =>
        {
            transform.DOScale(startScale, 0.2f).SetEase(Ease.Linear);
        });

        // Automatically show ad if not removed
        if (PlayerPrefs.GetInt("RemoveAds") != 1)
        {
            StartCoroutine(ShowAdAfterDelay());
        }
        else
        {
            // No ads, just give normal coins
            NoThanks();
        }
    }

    IEnumerator ShowAdAfterDelay()
    {
        yield return new WaitForSeconds(0.5f); // small delay for animation
        Mediation_Manager.mediation_Manager.ShowRewardedAd(RewardType.AdWheel);
    }



    IEnumerator ShowAd()
    {
        yield return new WaitForSeconds(0.5f);
        if (PlayerPrefs.GetInt("RemoveAds") != 1) 
        {
            Mediation_Manager.mediation_Manager.ShowRewardedAd(RewardType.AdWheel);
        }

        //  FindObjectOfType<RewardedAds>().ShowAd(RewardType.AdWheel);

    }


    public void GiveReward() // Called after ad watched
    {
        float zRotation = arrowTransform.localEulerAngles.z;
        if (zRotation > 180) zRotation -= 360;

        int multiplier = 1;

        if (zRotation > -15 && zRotation < 15)
            multiplier = 8;
        else if ((zRotation > -90 && zRotation < -55) || (zRotation > 55 && zRotation < 90))
            multiplier = 2;
        else if ((zRotation > -55 && zRotation < -15) || (zRotation > 15 && zRotation < 55))
            multiplier = 4;

        WinRewardManager winManager = FindObjectOfType<WinRewardManager>();
        if (winManager != null)
            winManager.ApplyMultiplier(multiplier); // Apply after ad watched
    }

    public void NoThanks() // Called if player skips ad
    {
        WinRewardManager winManager = FindObjectOfType<WinRewardManager>();
        if (winManager != null)
            winManager.ConfirmRewardWithoutAd(); // Just give base reward       
    }


    void ChangePlayerCoinsText()
    {
        
        float zRotation = arrowTransform.localEulerAngles.z;

        if (zRotation > 180)
        {
            zRotation -= 360; // Convert angles > 180 to their negative equivalent
        }

        if (zRotation > -15 && zRotation < 15)
        {
            coinsText.text = (Coin * 8).ToString();

        }
        else if ((zRotation > -90 && zRotation < -55) || (zRotation > 55 && zRotation < 90))
        {
            coinsText.text = (Coin * 2).ToString();

        }
        else if ((zRotation > -55 && zRotation < -15) || (zRotation > 15 && zRotation < 55))
        {
            coinsText.text = (Coin * 4).ToString();

        }
    }

    public void StartRotatingAgain()
    {
        isSpinning = true;
    }

}
