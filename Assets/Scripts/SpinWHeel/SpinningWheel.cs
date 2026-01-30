using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using DG.Tweening;
using System;

public class SpinningWheel : MonoBehaviour
{
    public float spinDuration = 4.0f;    // Total time for the spin
    public float maxSpinSpeed = 500.0f;  // Maximum spin speed
    private bool isSpinning = false;     // To prevent multiple spins at once

    // Define angle ranges for each item in the editor
    public Vector2[] angleRanges;
    public bool spinAvailable;
    int RewardIndex;

    [Header("Won")]
    [SerializeField] GameObject WonCanvas;
    [SerializeField] TMP_Text  RewardText;
    [SerializeField] GameObject SpinningWheelObj;
    int rewadValue;
    public Sprite coinSprite, GemSprite;
    public Image wonImageIcon;

    public enum Rewards {Coins,Gem }
    public Rewards rewardGot;
    SoundManager soundManager;
    private float lastTickAngle;         // To track the last angle for the tick sound
    public float tickInterval = 15.0f;

    private void OnEnable()
    {
        soundManager = SoundManager.soundManager;
        spinAvailable = true;
 
    }

    public void StartRegularSpin()
    {
        spinDuration = UnityEngine.Random.Range(4, 7f);
        if (!isSpinning && spinAvailable)
        {
            //SoundManager.instance.PlayAudio(AudioType.Wheel);
            soundManager.PlayButtonSound();

            StartCoroutine(SpinWheel());

            spinAvailable = false;
   
        }
    }

    private IEnumerator SpinWheel()
    {
        isSpinning = true;
        float currentSpinSpeed = maxSpinSpeed;
        float elapsedTime = 0.0f;
        lastTickAngle = SpinningWheelObj.transform.eulerAngles.z; // Initialize the last tick angle

        while (elapsedTime < spinDuration)
        {
            float newAngle = SpinningWheelObj.transform.eulerAngles.z + (currentSpinSpeed * Time.deltaTime);
            // Spin the wheel
            SpinningWheelObj.transform.Rotate(0, 0, currentSpinSpeed * Time.deltaTime);

            // Gradually slow down the spin speed
            currentSpinSpeed = Mathf.Lerp(maxSpinSpeed, 0, elapsedTime / spinDuration);

            if (Mathf.Abs(newAngle - lastTickAngle) >= tickInterval)
            {
                soundManager.PlayTickSound();
                lastTickAngle = newAngle; // Update the last tick angle
            }

            // Increase the elapsed time
            elapsedTime += Time.deltaTime;

            yield return null;
        }

        // After spinning, find out where it stopped
        float finalAngle = SpinningWheelObj.transform.eulerAngles.z;

        // Ensure the angle is positive and within 0-360 degrees
        if (finalAngle < 0)
        {
            finalAngle += 360.0f;
        }

        // Check which item the final angle falls into
        int stoppedItemIndex = -1;
        for (int i = 0; i < angleRanges.Length; i++)
        {
            if (finalAngle >= angleRanges[i].x && finalAngle <= angleRanges[i].y)
            {
                stoppedItemIndex = i;
                break;
            }
        }

        if (stoppedItemIndex != -1)
        {
            // Debug the result (adding 1 to match item numbers starting from 1)
            int Stoppedindex = stoppedItemIndex + 1;

            if (Stoppedindex == 1 || Stoppedindex == 5 )
                RewardIndex = 1;
            else if (Stoppedindex == 2 || Stoppedindex == 6)
                RewardIndex = 2;
            else if (Stoppedindex == 3 || Stoppedindex == 7)
                RewardIndex = 3;
            else if (Stoppedindex == 4 || Stoppedindex == 8)
                RewardIndex = 4;


        }
        else
        {
            Debug.LogError("No valid item found for final angle: " + finalAngle);
        }


        switch (RewardIndex)
        {
            case 1:
                StartCoroutine(ShowWonPanel(200,coinSprite,Rewards.Coins));
                break;
            case 2:
                StartCoroutine(ShowWonPanel(20,coinSprite, Rewards.Coins));
                break;
            case 3:
                StartCoroutine(ShowWonPanel(20,GemSprite, Rewards.Gem));
                break;
            default:
                StartCoroutine(ShowWonPanel(50,coinSprite, Rewards.Coins));
                break;
        }
        isSpinning = false;
    }


    IEnumerator ShowWonPanel(int amt,Sprite winSprite,Rewards wonThing)
    {
        yield return new WaitForSeconds(0.5f);
        rewadValue = amt;
        gameObject.SetActive(false);
        WonCanvas.SetActive(true);
        RewardText.text = $"+{amt}";
        wonImageIcon.sprite = winSprite;
        rewardGot = wonThing;
        soundManager.PlayRewardSound();
    }

    public void SetRewardValue(int amt)
    {
        rewadValue = amt;

    }

    public void ClaimReward()
    {
        SoundManager.soundManager.PlayButtonSound();
        if (rewardGot == Rewards.Coins)
        {
            PlayerPrefs.SetInt("Coins", PlayerPrefs.GetInt("Coins") + rewadValue);
        }
        if (rewardGot == Rewards.Gem)
        {
            PlayerPrefs.SetInt("Gems", PlayerPrefs.GetInt("Gems") + rewadValue);

        }
        UIManager.instance.SetCoinsText();
        WonCanvas.SetActive(false);
        PlayerPrefs.SetString("SpinWheelClaimed", DateTime.Now.ToString());
    }



}
