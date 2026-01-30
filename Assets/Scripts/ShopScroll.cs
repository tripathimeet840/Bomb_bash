using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ShopScroll : MonoBehaviour
{
    [SerializeField] private ScrollRect shopScroll;
    [SerializeField] private GameObject shopPanel;
    [SerializeField] private float scrollDuration = 0.5f;

    public void OpenShopAndScrollToBottom()
    {
        shopPanel.SetActive(true);
        StartCoroutine(SmoothScrollToBottom());
    }

    private IEnumerator SmoothScrollToBottom()
    {
        float elapsedTime = 0f;
        float startPos = shopScroll.verticalNormalizedPosition;
        float targetPos = 0f;

        while (elapsedTime < scrollDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / scrollDuration;
            shopScroll.verticalNormalizedPosition = Mathf.Lerp(startPos, targetPos, t);
            yield return null;
        }

        shopScroll.verticalNormalizedPosition = targetPos;
    }
}
