using DG.Tweening;
using UnityEngine;

public class TrainingGuide : MonoBehaviour
{
    public RectTransform handImage; // Assign this to the hand image in the Inspector
    public float duration = 2f; // Duration for one loop of the infinity symbol
    public float scaleAmount = 1.2f; // Amount to scale up and down
    public float size = 100f; // Size of the infinity loop

    private void Start()
    {
        AnimateHand();
    }

    private void AnimateHand()
    {
        // Define the points for the infinity shape
        Vector3[] path = new Vector3[8];
        float halfSize = size / 2f;

        // Points for the infinity loop
        path[0] = handImage.anchoredPosition;
        path[1] = path[0] + new Vector3(halfSize, halfSize);
        path[2] = path[1] + new Vector3(halfSize, -halfSize);
        path[3] = path[2] + new Vector3(-halfSize * 2, 0);
        path[4] = path[3] + new Vector3(-halfSize, halfSize);
        path[5] = path[4] + new Vector3(-halfSize, -halfSize);
        path[6] = path[5] + new Vector3(halfSize * 2, 0);
        path[7] = path[0]; // Loop back to the start

        // Create a looping path animation
        handImage.DOPath(path, duration, PathType.CatmullRom)
                 .SetOptions(true) // Close the path
                 .SetEase(Ease.Linear)
                 .SetLoops(-1, LoopType.Restart);

        // Create a looping scale animation
        handImage.DOScale(scaleAmount, duration / 2)
                 .SetEase(Ease.InOutSine)
                 .SetLoops(-1, LoopType.Yoyo);
    }
}
