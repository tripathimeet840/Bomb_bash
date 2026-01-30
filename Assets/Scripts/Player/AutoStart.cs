using UnityEngine;

public class AutoStart : MonoBehaviour
{
    void Start()
    {
        // instantly start the game
        if (GameManager.instance != null)
        {
            GameManager.instance.StartGame(0f);
        }
        else
        {
            Debug.LogError("GameManager not found in scene!");
        }
    }
}
