using UnityEngine;

public class EnemyBombSelector : MonoBehaviour
{
    [Header("Bomb References (Attach all bombs as children to enemy head)")]
    public GameObject[] bombs;  // Array of bomb GameObjects attached to enemy’s head

    void Start()
    {
        ActivateRandomBomb();
    }

    public void ActivateRandomBomb()
    {
        // First disable all bombs
        foreach (GameObject bomb in bombs)
        {
            if (bomb != null)
                bomb.SetActive(false);
        }

        // Choose a random bomb
        if (bombs.Length > 0)
        {
            int randomIndex = Random.Range(0, bombs.Length);
            bombs[randomIndex].SetActive(true);
        }
    }
}
