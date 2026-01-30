using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class LeaderboardEntry
{
    public string playerName;
    public int score;

    public LeaderboardEntry(string name, int score)
    {
        playerName = name;
        this.score = score;
    }
}

public class Leaderboard : MonoBehaviour
{
    public List<LeaderboardEntry> fakeEntries = new List<LeaderboardEntry>();
    public LeaderboardEntry playerEntry;   // separate player entry
    public int numberOfEntries = 10;

    private string[] randomNames = new string[]
    {
        "Ace", "Blaze", "Shadow", "Nova", "Rogue",
        "Hunter", "Flash", "Viper", "Ghost", "Storm"
    };

    public void GenerateFakeLeaderboard()
    {
        fakeEntries.Clear();

        // 1. Generate Top 10 fake entries
        for (int i = 0; i < numberOfEntries; i++)
        {
            string name = randomNames[Random.Range(0, randomNames.Length)] + Random.Range(1, 9999);
            int score = Random.Range(100, 10000);
            fakeEntries.Add(new LeaderboardEntry(name, score));
        }

        // Sort descending
        fakeEntries.Sort((a, b) => b.score.CompareTo(a.score));

        // 2. Player entry (separate, NOT in fakeEntries list)
        string playerName = PlayerPrefs.GetString("PlayerName", "You");
        int playerScore = PlayerPrefs.GetInt("PlayerScore", Random.Range(1000, 5000));

        playerEntry = new LeaderboardEntry(playerName, playerScore);
    }
}
