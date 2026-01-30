using System.Collections;
using UnityEngine.UI;
using UnityEngine;
using TMPro;

[RequireComponent(typeof(Animator))]
public class AchievementsNotificationController : MonoBehaviour
{

    [SerializeField] TMP_Text achievementTitleLabel,achivementPriceText;
    [SerializeField] Image Icon;
    [SerializeField] Sprite coinsSpirte, GemSprite;
    private Animator m_animator;

    private void Awake()
    {
        m_animator = GetComponent<Animator>();
    }

    public void ShowNotification(Achievement achievement)
    {

        achievementTitleLabel.text = achievement.title;
        achivementPriceText.text = "+"+achievement.WonPrice;

        if(achievement.reward== Achievement.RewardType.Coins)
        {
            Icon.sprite = coinsSpirte;
        }
        else
        {
            Icon.sprite = GemSprite;
        }
        m_animator.SetTrigger("Appear");
    }
}