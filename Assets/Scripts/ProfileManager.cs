using UnityEngine;

public class ProfileManager : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject mainUI;       
    [SerializeField] private GameObject profileBar;    
    [SerializeField] private GameObject profilePanel;  
    [SerializeField] private GameObject spinPanel;  
    [SerializeField] private GameObject noAds;  
    [SerializeField] private GameObject store;
    [SerializeField] private GameObject dailyPanel;  
    [SerializeField] private GameObject weeklyPanel;  
    [SerializeField] private GameObject achievementPanel;  
    [SerializeField] private GameObject charactersInventory;  
    [SerializeField] private GameObject bombsInventory;  
    [SerializeField] private GameObject characterUpgrade; 
    [SerializeField] private GameObject bombsUpgrade; 
    [SerializeField] private GameObject startCam;  
    [SerializeField] private GameObject character;  
    [SerializeField] private GameObject bomb;  
    [SerializeField] private GameObject dailyRewards;

    public void OpenProfile()
    {
        mainUI.SetActive(false);
        profileBar.SetActive(false);
        profilePanel.SetActive(true);
        spinPanel.SetActive(false);
        noAds.SetActive(false);
        store.SetActive(false);
        dailyPanel.SetActive(false);
        weeklyPanel.SetActive(false);
        achievementPanel.SetActive(false);
        charactersInventory.SetActive(false);
        bombsInventory.SetActive(false);
        startCam.SetActive(true);
        
        
    }

    public void CloseProfile()
    {
        mainUI.SetActive(true);
        profileBar.SetActive(true);
        profilePanel.SetActive(false);
        character.SetActive(true);
        bomb.SetActive(false);
        characterUpgrade.SetActive(false);
        bombsUpgrade.SetActive(false);
    }

    public void OpenDailyRewards()
    {
        mainUI.SetActive(true);
        profileBar.SetActive(true);
        profilePanel.SetActive(false);
        character.SetActive(true);
        bomb.SetActive(false);
        characterUpgrade.SetActive(false);
        bombsUpgrade.SetActive(false);
        dailyRewards.SetActive(true);
    }
}
