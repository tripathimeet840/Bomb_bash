using UnityEngine;
using UnityEngine.UI;

public class ChestButton : MonoBehaviour
{
    [Header("Chest Icons")]
    [SerializeField] private Sprite defaultIcon;
    [SerializeField] private Sprite newIcon;

    [Header("UI Reference")]
    [SerializeField] private Image chestImage; 

    private bool isDefault = true;

    public void OnChestButtonPressed()
    {
        if (isDefault)
            chestImage.sprite = newIcon;
        else
            chestImage.sprite = defaultIcon;

        isDefault = !isDefault;
    }
}
