using UnityEngine;
using UnityEngine.SceneManagement;

public class RopeCut : MonoBehaviour
{
    [Header("References")]
    public GameObject bomb;           
    public Transform newParent;       
    public GameObject rope;           
    public GameObject cutButton;

    private bool isCut = false;
    private Rigidbody2D bombRb;

    void Start()
    {
        if (bomb != null)
        {
            bombRb = bomb.GetComponent<Rigidbody2D>();
            if (bombRb != null)
            {
                bombRb.gravityScale = 0f;
            }
        }

        cutButton.SetActive(true);
    }

    public void CutRope()
    {
        if (isCut) return;
        isCut = true;

        if (bombRb != null)
        {
            bombRb.gravityScale = 50f;
        }

        if (bomb != null && newParent != null)
        {
            bomb.transform.SetParent(newParent, true);
        }

        if (rope != null)
        {
            StartCoroutine(FadeOutRope());
        }

        cutButton.SetActive(false);
    }

    private System.Collections.IEnumerator FadeOutRope()
    {
        SpriteRenderer sr = rope.GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            Color color = sr.color;
            for (float t = 0f; t < 1f; t += Time.deltaTime / 0.5f)
            {
                color.a = Mathf.Lerp(1f, 0f, t);
                sr.color = color;
                yield return null;
            }

            color.a = 0f;
            sr.color = color;
            rope.SetActive(false);
        }
        else
        {
            rope.SetActive(false);
        }
    }

    public void BackToHome()
    {
        SceneManager.LoadScene("LandingScene");
    }
}
