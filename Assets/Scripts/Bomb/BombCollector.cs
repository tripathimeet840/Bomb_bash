using UnityEngine;

public class BombCollector : MonoBehaviour
{
    public int bombCount = 0;
    private Animator anim;

    // Track which bomb type player currently has
    public string currentBombType = "";

    public GameObject blastButton;

    private void Start()
    {
        anim = GetComponent<Animator>();
        anim.SetLayerWeight(1, 0f);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (bombCount > 0) return;

        if (collision.collider.CompareTag("CollectableBomb") ||
            collision.collider.CompareTag("CollectableTimeBomb") ||
            collision.collider.CompareTag("CollectableClusterBomb") ||
            collision.collider.CompareTag("CollectableIceBomb") ||
            collision.collider.CompareTag("CollectableStickyBomb"))
        {
            bombCount++;
            currentBombType = collision.collider.tag;
            Destroy(collision.gameObject);
            UpdateBombState();
        }
    }

    private void UpdateBombState()
    {
        anim.SetLayerWeight(1, bombCount > 0 ? 1f : 0f);
    }

    public void EnableBlastButton()
    {
        blastButton.SetActive(true);
    }

    public void DisableBlastButton()
    {
        blastButton.SetActive(false);
    }

}
