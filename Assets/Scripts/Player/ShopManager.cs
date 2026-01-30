using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopManager : MonoBehaviour
{
    [Header("Characters")]
    public GameObject[] characters;   // List of all character prefabs
    private bool[] unlockedCharacters;
    private int selectedCharacter = 0;

    [Header("UI")]
    public Button[] characterButtons;
    public RectTransform selectionFrame;
    public RectTransform selectionFrameUpgradesMenu;
    public GameObject[] lockOverlays;  // Same length as characters

    [Header("Unlock Costs")]
    public int[] coinCosts;   // Cost in coins for each character
    public int[] gemCosts;    // Cost in gems for each character

    [Header("References")]
    public PlayerDataManager playerDataManager; // Drag your PlayerDataManager here

    [Header("UI Locks")]
    public TextMeshProUGUI[] lockTexts;  // Assign cost texts per character

    [Header("Feedback UI")]
    public TextMeshProUGUI feedbackText;   // Assign a TextMeshProUGUI in the scene
    public float feedbackDuration = 2f;    // How long the message shows


    public int previewedCharacter = 0;

    [Header("UI Buttons")]
    public GameObject unlockButton; // Assign your Unlock button here


    private void Start()
    {
        unlockedCharacters = new bool[characters.Length];
        for (int i = 0; i < characters.Length; i++)
            unlockedCharacters[i] = PlayerPrefs.GetInt("CharacterUnlocked_" + i, i == 0 ? 1 : 0) == 1;

        // Show equipped character
        selectedCharacter = PlayerPrefs.GetInt("SelectedCharacter", 0);
        EnableCharacter(selectedCharacter);

        for (int i = 0; i < characterButtons.Length; i++)
        {
            int index = i;
            characterButtons[i].onClick.AddListener(() => OnCharacterSelected(index));
        }

        UpdateSelectionFrame(selectedCharacter);
        RefreshUI();
    }



    private void Update()
    {
        // Sync selection frames
        selectionFrame.transform.position = selectionFrameUpgradesMenu.transform.position;
        selectionFrameUpgradesMenu.transform.position = selectionFrame.transform.position;

        RefreshUI();
    }

    public void OnCharacterSelected(int index)
    {
        if (!unlockedCharacters[index])
        {
            // Locked preview only
            PreviewCharacter(index);
            ShowFeedback("This character is locked! Tap 'Unlock' to buy.");
            return;
        }

        // Unlocked equip
        EnableCharacter(index);
        UpdateSelectionFrame(index);
        ShowFeedback("Character equipped!");
    }


    private void TryUnlockCharacter(int index)
    {
        int costCoins = (coinCosts.Length > index) ? coinCosts[index] : 0;
        int costGems = (gemCosts.Length > index) ? gemCosts[index] : 0;

        if (playerDataManager.coins >= costCoins && playerDataManager.gems >= costGems)
        {
            // Deduct cost
            playerDataManager.DeductCoins(costCoins);
            playerDataManager.DeductGems(costGems);

            // Unlock character
            UnlockCharacter(index);

            // Save immediately
            playerDataManager.SaveData();

            // Force UI refresh right now
            RefreshUI();

            Debug.Log($"Character {index} unlocked! Coins left: {playerDataManager.coins}");
        }
        else
        {
            string message = "Not enough ";
            if (playerDataManager.coins < costCoins && playerDataManager.gems < costGems)
                message += "coins and gems!";
            else if (playerDataManager.coins < costCoins)
                message += "coins!";
            else if (playerDataManager.gems < costGems)
                message += "gems!";

            ShowFeedback(message);
        }

    }

    public void UnlockCharacter(int index)
    {
        unlockedCharacters[index] = true;
        PlayerPrefs.SetInt("CharacterUnlocked_" + index, 1);
        PlayerPrefs.Save();
        Debug.Log("Unlocked Character: " + index);

        RefreshUI();
    }

    public void RefreshUI()
    {
        // Sync unlockedCharacters with PlayerPrefs first
        for (int i = 0; i < characters.Length; i++)
        {
            unlockedCharacters[i] = PlayerPrefs.GetInt("CharacterUnlocked_" + i, i == 0 ? 1 : 0) == 1;
        }

        for (int i = 0; i < characterButtons.Length; i++)
        {
            if (lockOverlays.Length > i)
            {
                lockOverlays[i].SetActive(!unlockedCharacters[i]);
            }

            if (lockTexts.Length > i)
            {
                int costCoins = (coinCosts.Length > i) ? coinCosts[i] : 0;
                int costGems = (gemCosts.Length > i) ? gemCosts[i] : 0;

                if (!unlockedCharacters[i])
                {
                    if (costCoins > 0) lockTexts[i].text = costCoins + " Coins";
                    else if (costGems > 0) lockTexts[i].text = costGems + " Gems";
                    else lockTexts[i].text = "Free";
                }
                else
                {
                    lockTexts[i].text = "";
                }
            }
        }

        // Update unlock button based on current previewed character
        UpdateUnlockButtonVisibility(previewedCharacter);

        Canvas.ForceUpdateCanvases();
    }




    // Only called when equipping an unlocked character
    private void EnableCharacter(int index)
    {
        for (int i = 0; i < characters.Length; i++)
        {
            characters[i].SetActive(i == index);
        }

        selectedCharacter = index;
        PlayerPrefs.SetInt("SelectedCharacter", index); // persist equipped character
        PlayerPrefs.Save();

        Debug.Log("Equipped Character: " + index);
    }


    private void UpdateSelectionFrame(int index)
    {
        if (selectionFrame != null && characterButtons.Length > index)
        {
            selectionFrame.position = characterButtons[index].transform.position;
            selectionFrameUpgradesMenu.position = characterButtons[index].transform.position;
        }
    }

    private void ShowFeedback(string message)
    {
        if (feedbackText != null)
        {
            feedbackText.text = message;
            feedbackText.gameObject.SetActive(true);
            CancelInvoke(nameof(HideFeedback)); // Cancel any previous hide calls
            Invoke(nameof(HideFeedback), feedbackDuration);
        }
    }

    private void HideFeedback()
    {
        if (feedbackText != null)
            feedbackText.gameObject.SetActive(false);
    }

    // Only shows the character model for inspection
    public void PreviewCharacter(int index)
    {
        previewedCharacter = index;  // Track previewed
        for (int i = 0; i < characters.Length; i++)
        {
            characters[i].SetActive(i == index);
        }

        // Update Unlock button visibility
        UpdateUnlockButtonVisibility(index);

        Debug.Log("Previewing Character: " + index);
    }

    private void UpdateUnlockButtonVisibility(int previewedIndex)
    {
        if (unlockButton == null) return;

        // Show unlock button only if the character is locked
        unlockButton.SetActive(!unlockedCharacters[previewedIndex]);
    }

    private int GetPreviewedCharacterIndex()
    {
        return previewedCharacter;
    }


    public void OnUnlockButtonClicked()
    {
        // Use the currently previewed character
        int index = GetPreviewedCharacterIndex(); // We'll define this next
        TryUnlockCharacter(index);
    }

    public void ResetToEquippedCharacter()
    {
        EnableCharacter(selectedCharacter); // Only activates the equipped one
        UpdateSelectionFrame(selectedCharacter);
    }

}
