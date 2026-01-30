using UnityEngine;

public class LandingSceneManager : MonoBehaviour
{
    public GameObject enterNamePanel; 

    private void Start()
    {
        if (PlayerPrefs.HasKey("PlayerName") && !string.IsNullOrEmpty(PlayerPrefs.GetString("PlayerName")))
        {
            enterNamePanel.SetActive(false);
        }
        else
        {
            enterNamePanel.SetActive(true); 
        }
    }
}
