using TMPro;
using System.Collections.Generic;
using UnityEngine;

public class AchievementsManager : MonoBehaviour
{
    public AchievementDatabase database;
    public AchievementsNotificationController achievementsNotificationController;
    public GameObject achievementItemPrefab;
    public Transform content;
    public AchievementID achievementToShow;
    [SerializeField] GameObject AchivementWindow;
    [SerializeField] public List<AchievementItemController> achievementItems;
    public static AchievementsManager instance;
    public TMP_Text achievemntsCounts;
    public GameObject achievementCountPanel;


    private void Awake()
    {
        //if (instance == null)
        //{
        //    instance = this;
        //}
        //else
        //{
        //    Destroy(gameObject);
        //}
    }


    private void Start()
    {
        LoadAchievements();
        CheckAvaialble();
    }

    public void CheckAvaialble()
    {
        int count = 0;
        foreach (AchievementItemController controller in achievementItems)
        {
            if (controller.unlocked && !controller.achievement.Redeemed)
            {
                count++;
            }
            controller.RefreshView();
        }
        if (count > 0)
        {
            achievementCountPanel.SetActive(true);
            achievemntsCounts.text = count.ToString();
        }
        else
        {
            achievementCountPanel.SetActive(false);
        }
    }

    public void ShowNotification(Achievement achievement)
    {
        //Achievement achievement = database.achievements[(int)achievementToShow];
        achievementsNotificationController.ShowNotification(achievement);
        CheckAvaialble();
    }
   

    public void ShowAchievementWindow(bool _enabled)
    {
        SoundManager.soundManager.PlayButtonSound();

        // LoadAchievements();
        foreach (AchievementItemController controller in achievementItems)
        {
            if (PlayerPrefs.GetInt(controller.achievement.id) == 1)
                controller.unlocked = true;
            controller.RefreshView();
        }
        AchivementWindow.SetActive(_enabled);
      
    }

    [ContextMenu("LoadAchievements()")]
    public void LoadAchievements()
    {
        
        foreach (AchievementItemController controller in achievementItems)
        {
            if (controller != null)
                DestroyImmediate(controller.gameObject);
        }
        achievementItems.Clear();

        foreach (Achievement achievement in database.achievements)
        {
            GameObject obj = Instantiate(achievementItemPrefab, content);
            AchievementItemController controller = obj.GetComponent<AchievementItemController>();
            bool unlocked = PlayerPrefs.GetInt(achievement.id, 0) == 1;
            controller.unlocked = unlocked;
            controller.achievement = achievement;

            controller.RefreshView();
            achievementItems.Add(controller);
        }
    }
    public void UnlockAchievement()
    {
        UnlockAchievement(achievementToShow);
    }

    public void UnlockAchievement(AchievementID achievement)
    {
        AchievementItemController item = achievementItems[(int)achievement];
        if (item.unlocked)
            return;

        ShowNotification(item.achievement);
        PlayerPrefs.SetInt(item.achievement.id, 1);
        item.unlocked = true;
        PlayerPrefs.Save(); // Ensure that the data is saved
        item.RefreshView();
    }

    public void UnlockAchievement(Achievement achievement)
    {
        AchievementItemController item = achievementItems.Find(controller => controller.achievement == achievement);
        if (item == null || item.unlocked) 
        {
            Debug.Log("Here");
        return; 
        }

        ShowNotification(achievement);
        PlayerPrefs.SetInt(achievement.id, 1);
        item.unlocked = true;
        PlayerPrefs.Save(); // Ensure that the data is saved
        item.RefreshView();
    }
}