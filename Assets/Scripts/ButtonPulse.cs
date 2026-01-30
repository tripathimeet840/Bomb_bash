using UnityEngine;
using DG.Tweening;

public class ButtonPulse : MonoBehaviour
{
    [SerializeField] private float pulseScale = 1.1f;  
    [SerializeField] private float pulseDuration = 1.2f;

    private void Start()
    {
        transform.localScale = Vector3.one;

        transform.DOScale(Vector3.one * pulseScale, pulseDuration)
            .SetEase(Ease.InOutSine)
            .SetLoops(-1, LoopType.Yoyo);
    }
}
