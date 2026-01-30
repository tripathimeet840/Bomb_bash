using UnityEngine;

public class TaskCenterManager : MonoBehaviour
{
    [SerializeField] private GameObject dailiesPanel;
    [SerializeField] private GameObject weekliesPanel;
    [SerializeField] private GameObject achievementsPanel;

    public void ShowDailies()
    {
        dailiesPanel.SetActive(true);
        weekliesPanel.SetActive(false);
        achievementsPanel.SetActive(false);
    }

    public void ShowWeeklies()
    {
        dailiesPanel.SetActive(false);
        weekliesPanel.SetActive(true);
        achievementsPanel.SetActive(false);
    }

    public void ShowAchievements()
    {
        dailiesPanel.SetActive(false);
        weekliesPanel.SetActive(false);
        achievementsPanel.SetActive(true);
    }

    // Call this from your Close button
    public void CloseAll()
    {
        dailiesPanel.SetActive(false);
        weekliesPanel.SetActive(false);
        achievementsPanel.SetActive(false);
    }
}
