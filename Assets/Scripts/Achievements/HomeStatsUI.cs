using TMPro;
using UnityEngine;

public class HomeStatsUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI gamesPlayedText;
    [SerializeField] private TextMeshProUGUI gamesWonText;
    [SerializeField] private TextMeshProUGUI gamesLostText;
    [SerializeField] private TextMeshProUGUI winStreakText;


    private void Start()
    {
        UpdateStatsUI();
    }

    public void UpdateStatsUI()
    {
        int played = PlayerPrefs.GetInt("GamesPlayed", 0);
        int won = PlayerPrefs.GetInt("GamesWon", 0);
        int lost = PlayerPrefs.GetInt("GamesLost", 0);
        int streak = PlayerPrefs.GetInt("WinStreak", 0);

        if (gamesPlayedText != null)
            gamesPlayedText.text = $"Games Played: {played}";

        if (gamesWonText != null)
            gamesWonText.text = $"Games Won: {won}";

        if (gamesLostText != null)
            gamesLostText.text = $"Games Lost: {lost}";

        if (winStreakText != null)
            winStreakText.text = $"Win Streak: {streak}";
    }

}
