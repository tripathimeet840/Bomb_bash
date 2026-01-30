using TMPro;
using UnityEngine;

public class LeaderboardUI : MonoBehaviour
{
    public Leaderboard leaderboard;
    public Transform contentParent;

    [Header("Prefabs")]
    public GameObject entryPrefab;      // normal rows
    public GameObject playerEntryPrefab; // special player row

    private void OnEnable()
    {
        ShowLeaderboard();
    }

    public void ShowLeaderboard()
    {
        leaderboard.GenerateFakeLeaderboard();

        // Clear old entries
        foreach (Transform t in contentParent)
            Destroy(t.gameObject);

        // Top 10 fake entries
        for (int i = 0; i < leaderboard.fakeEntries.Count; i++)
        {
            var entry = leaderboard.fakeEntries[i];
            GameObject obj = Instantiate(entryPrefab, contentParent);
            LeaderboardEntryUI ui = obj.GetComponent<LeaderboardEntryUI>();
            ui.Setup(i + 1, entry.playerName, entry.score);
        }

        // Player entry  random rank with 3–4 digits
        int playerRank = Random.Range(100, 9999); // e.g. 345, 8921
        GameObject playerObj = Instantiate(playerEntryPrefab, contentParent);
        LeaderboardEntryUI playerUI = playerObj.GetComponent<LeaderboardEntryUI>();
        playerUI.Setup(playerRank, leaderboard.playerEntry.playerName, leaderboard.playerEntry.score);
    }
}
