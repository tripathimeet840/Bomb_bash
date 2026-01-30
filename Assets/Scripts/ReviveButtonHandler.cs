using UnityEngine;

public class ReviveButtonHandler : MonoBehaviour
{
    public void OnReviveButtonPressed()
    {
        Mediation_Manager.mediation_Manager.ShowRewardedAd(RewardType.Revive);
    }
}
