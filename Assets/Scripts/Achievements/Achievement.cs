
using UnityEngine;

[System.Serializable]
public class Achievement
{
	public string id;
	public string title;
	public string description;
	
	public enum RewardType { Coins,Gems}
	public RewardType reward;

	[HideInInspector] public int CurrentNumber;
	public int TotalNumber = 1;
	public bool Redeemed;
	public int WonPrice;
}

public enum AchievementID
{
	FirstRun,
    Played1,Played5,Played10,Played20,Played50,Played100,
    Won5,Won10,Won20,Won50,Won100,
	Lose5,Lose10,Lose20,
	PurchaseSomething,
	UnlockCharacter,
	Unlock5Chracater,
	UnlockMap,Unlock5Map
}