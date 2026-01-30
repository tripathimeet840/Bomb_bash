using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Health : MonoBehaviour
{
    [SerializeField] public float maxHealth;
    [SerializeField] public float currentHealth;

    [SerializeField] public Image healthBar;
    [SerializeField] private Animator animator;

    public delegate void OnDamageTaken(float damage);
    public event OnDamageTaken DamageTaken;

    public delegate void OnDeath();
    public event OnDeath Died;
    public bool enemyHealth;
    GameManager gameManager;
    public bool alreadyDied;

    private void Start()
    {
        gameManager = GameManager.instance;
        if (enemyHealth)
        {
            EnableRandomSkin();
        }

        currentHealth = maxHealth;
    }

    void EnableRandomSkin()
    {
        int childCount = Mathf.Min(5, transform.childCount);
        GameObject[] children = new GameObject[childCount];

        for (int i = 0; i < childCount; i++)
        {
            children[i] = transform.transform.GetChild(i).gameObject;
        }

        int randomIndex = Random.Range(0, childCount);

        for (int i = 0; i < childCount; i++)
        {
            children[i].SetActive(i == randomIndex);
        }
    }


    public void HealthInc(float amt)
    {
        currentHealth = Mathf.Min(currentHealth + amt, maxHealth); // Cap health to max

        if (healthBar != null)
        {
            healthBar.fillAmount = currentHealth / maxHealth; // Update the bar
        }
    }


    private void OnEnable()
    {
        if (enemyHealth)
        {
            currentHealth = maxHealth;
        }
    }


    public void TakeDamage(float damage)
    {
        if (currentHealth <= 0)
        {

            return;
        }

        currentHealth -= damage;

        DamageTaken?.Invoke(damage);

        if (healthBar != null)
        {
            healthBar.fillAmount = (currentHealth / maxHealth);
        }

        if (animator != null)
        {
            animator.SetTrigger("TakeDamage");

            if (currentHealth <= 0)
            {
                Died?.Invoke();

                gameObject.layer = 0;
                if (enemyHealth && !alreadyDied)
                {
                    alreadyDied = true;
                    GetComponent<Target>().enabled = false;
                    gameManager.EnemyDied();
                    SoundManager.soundManager.EnemyDiedSound();
                }
                else
                {
                    if (!alreadyDied)
                    {
                        alreadyDied = true;

                        //StartCoroutine(DeactivateAfterTime());
                        gameManager.PlayerLose();
                        GetComponent<PlayerMovement>().dead = true;
                    }
                }

            }
        }

        IEnumerator DeactivateAfterTime()
        {
            GetComponent<RagdolMaintainer>().TurnOnRagdoll();
            yield return new WaitForSeconds(1f);
            SoundManager.soundManager.PlayPlayerDeath();
            yield return new WaitForSeconds(5f);
            gameObject.SetActive(false);
        }
    }
}