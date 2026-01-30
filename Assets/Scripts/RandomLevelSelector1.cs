using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening; // Make sure DOTween is installed
using System.Collections;
using UnityEngine.SceneManagement;

public class RandomLevelSelector1 : MonoBehaviour
{
    public ScrollRect scrollRect; // Assign your ScrollRect UI
    public RectTransform contentPanel; // The panel holding level icons
    public int totalLevels = 15;
    public float spinDuration = 3f; // Time for fast spinning
    public float decelerationTime = 2f; // Time for smooth stopping
    public AnimationCurve spinCurve; // Optional curve for smoothing deceleration
    [SerializeField] TMP_Text currentLevelName; // TMP text to display the current level name
    public float scrollSpeed = 2f; // Speed of scrolling (higher = faster)

    private bool isSpinning = false;
    [Header("Fadein/out")]
    [SerializeField]Image targetImage;
    [SerializeField] float fadeintime, fadeouttime;
    

    private void OnEnable()
    {
        Time.timeScale = 1;
        FadeOut(fadeouttime);
        StartLevelSelection();
    }

    private void Update()
    {
        // Continuously update the current level name as you scroll
        if(isSpinning)
        UpdateCurrentLevelName();
    }

    public void StartLevelSelection()
    {
        if (isSpinning) return;
        StartCoroutine(SpinAndSelectLevel());
    }

    private IEnumerator SpinAndSelectLevel()
    {
        isSpinning = true;
        float elapsedTime = 0f;
        float startPos = scrollRect.horizontalNormalizedPosition;

        // Select a random level
        int randomLevelIndex = Random.Range(0, totalLevels);
        Transform selectedLevel = contentPanel.GetChild(randomLevelIndex);

        // Calculate target position (center the selected level)
        float targetPos = GetCenteredPosition(selectedLevel);

        while (elapsedTime < spinDuration)
        {
            elapsedTime += Time.deltaTime;
            // Keep the speed constant (adjustable scroll speed)
            scrollRect.horizontalNormalizedPosition = Mathf.Repeat(startPos + elapsedTime * scrollSpeed, 1f); // Wrap it around
            yield return null;
        }

        // Smoothly transition from full speed to deceleration phase
        float transitionStartTime = Time.time;
        float transitionDuration = 0.2f; // Small time for the transition
        float transitionStartPos = scrollRect.horizontalNormalizedPosition;

        while (Time.time - transitionStartTime < transitionDuration)
        {
            float transitionT = (Time.time - transitionStartTime) / transitionDuration;
            // Easing curve from fast speed to deceleration smoothly
            scrollRect.horizontalNormalizedPosition = Mathf.Lerp(transitionStartPos, targetPos, transitionT);
            yield return null;
        }

        elapsedTime = 0f;
        startPos = scrollRect.horizontalNormalizedPosition;

        // Apply deceleration with easing
        while (elapsedTime < decelerationTime)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / decelerationTime;
            float easedT = spinCurve != null ? spinCurve.Evaluate(t) : t;

            scrollRect.horizontalNormalizedPosition = Mathf.Lerp(startPos, targetPos, easedT);
            yield return null;
        }

        // Ensure final stop at the target position
        scrollRect.horizontalNormalizedPosition = targetPos;

        // Animate selected GameObject to scale to 1.5f (smoothly)
        selectedLevel.DOScale(1.5f, 0.5f).SetEase(Ease.OutBack);
        selectedLevel.parent = scrollRect.transform;
        isSpinning = false;

        // Update the current level name with the selected level after the spin stops
        Debug.Log("Selected Level: " + selectedLevel.name);
        currentLevelName.text = selectedLevel.name;
        yield return new WaitForSeconds(0.65f);
        FadeIn(fadeintime);
        yield return new WaitForSeconds(fadeintime);
        SceneManager.LoadScene(selectedLevel.name);

    }

    private float GetCenteredPosition(Transform levelTransform)
    {
        // Convert level's anchored position to normalized position
        float contentWidth = contentPanel.rect.width;
        float viewportWidth = scrollRect.viewport.rect.width;
        float levelPosX = levelTransform.localPosition.x;

        // Calculate target normalized position considering the center offset
        float centerOffset = viewportWidth / 2f;
        float targetPos = Mathf.Clamp01((levelPosX - centerOffset) / (contentWidth - viewportWidth));

        return targetPos;
    }

    private void UpdateCurrentLevelName()
    {
        // Find the level closest to the center of the viewport in the X-axis
        float viewportWidth = scrollRect.viewport.rect.width;
        float contentWidth = contentPanel.rect.width;

        // Calculate the center of the viewport in local position
        float viewportCenterX = scrollRect.horizontalNormalizedPosition * (contentWidth - viewportWidth) + (viewportWidth / 2f);

        float closestLevelDistance = Mathf.Infinity;
        Transform closestLevel = null;

        // Loop through each child of the content panel and find the one closest to the center
        foreach (Transform level in contentPanel)
        {
            float levelPosX = level.localPosition.x;

            // Calculate the distance of the level from the center
            float distance = Mathf.Abs(levelPosX - viewportCenterX);

            // Keep track of the closest level
            if (distance < closestLevelDistance)
            {
                closestLevelDistance = distance;
                closestLevel = level;
            }
        }

        // Update the current level name text with the closest level's name
        if (closestLevel != null)
        {
            currentLevelName.text = closestLevel.name;
        }
    }

    public void FadeIn(float fadeInTime)
    {
        // Fade the image in (alpha from 0 to 1)
        targetImage.DOFade(1f, fadeInTime).SetEase(Ease.Linear); // Adjust ease if needed
    }

    public void FadeOut(float fadeOutTime)
    {
        // Fade the image out (alpha from 1 to 0)
        targetImage.DOFade(0f, fadeOutTime).SetEase(Ease.Linear); // Adjust ease if needed
    }
}
