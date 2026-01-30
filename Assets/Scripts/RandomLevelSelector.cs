using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class RandomLevelSelector : MonoBehaviour
{
    public Image levelImage; // Reference to the UI image where the sprites will flicker
    public List<Sprite> levelSprites; // List of level sprites
    public float flickerInterval = 0.1f; // Time between sprite changes

    private bool isFlickering = true;
    private string currentSceneName, selectedLevel;
    [SerializeField] GameObject panel;

    private void Start()
    {
      //  ShowRandomLevels();
    }

    public void ShowRandomLevels()
    {
        panel.SetActive(true);
        panel.transform.localScale = Vector3.zero;
        panel.transform.DOScale(Vector3.one, 0.2f).SetEase(Ease.Linear);
        currentSceneName = SceneManager.GetActiveScene().name;

        // Start flickering sprites
        StartCoroutine(FlickerSprites());
    }

    public void CloseRandomLevel()
    {
        SoundManager.soundManager.PlayButtonSound();
        panel.transform.DOScale(Vector3.zero, 0.2f).SetEase(Ease.Linear).OnComplete(() =>
         {
             panel.SetActive(false);
         });
    }


    public void PlayRandomLevel()
    {
        SoundManager.soundManager.PlayButtonSound();
        /*  if (!isFlickering)
          {
              Debug.LogWarning("Already stopped!");
              return;
          }

          // Stop flickering and select a level instantly
          isFlickering = false;

          // Select a random level
          Sprite finalSprite = GetRandomSprite();

          // Update the image to the final selected level
          levelImage.sprite = finalSprite;

          FindObjectOfType<Mediation_Manager>().ShowRewardedAd(RewardType.RandomLevel);
          selectedLevel = finalSprite.name;*/
        SceneManager.LoadScene("RandomLevel");
    }

    private IEnumerator FlickerSprites()
    {
        isFlickering = true;

        while (isFlickering)
        {
            // Pick a random sprite, excluding the current scene's sprite
            Sprite randomSprite = GetRandomSprite();

            // Update the image
            levelImage.sprite = randomSprite;

            // Debug the name of the current sprite
            Debug.Log($"Flickering Level: {randomSprite.name}");

            // Wait for the flicker interval
            yield return new WaitForSeconds(flickerInterval);
        }
    }

    private Sprite GetRandomSprite()
    {
        // Create a list excluding the current scene's sprite
        List<Sprite> validSprites = new List<Sprite>();

        foreach (Sprite sprite in levelSprites)
        {
            if (sprite.name != currentSceneName)
            {
                validSprites.Add(sprite);
            }
        }

        // Ensure we have valid sprites
        if (validSprites.Count == 0)
        {
            Debug.LogError("No valid sprites found!");
            return null;
        }

        // Return a random sprite from the valid list
        return validSprites[Random.Range(0, validSprites.Count)];
    }

    public void LoadLevel()
    {
        SceneManager.LoadScene(selectedLevel);
    }
}
