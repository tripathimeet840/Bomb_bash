using UnityEngine;

public class UiBombCollision : MonoBehaviour
{
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Bomb"))
        {
            Debug.Log("Bomb collided with bottom box!");
        }
    }
}
