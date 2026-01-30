using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ThrowButton : MonoBehaviour
{
    public Button throwButton;

    private void OnEnable()
    {
        StartCoroutine(DelayedBind());
    }

    private IEnumerator DelayedBind()
    {
        yield return null;
        BindToActivePlayer();
    }

    public void BindToActivePlayer()
    {
        PlayerMovement activePlayer = FindObjectOfType<PlayerMovement>();

        if (activePlayer != null && throwButton != null)
        {
            throwButton.onClick.RemoveAllListeners();
            throwButton.onClick.AddListener(activePlayer.OnThrowButtonPressed);
        }
        else
        {
            Debug.LogWarning("No active player or throwButton not assigned!");
        }
    }
}
