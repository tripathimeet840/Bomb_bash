using UnityEngine;

public class UpgradesMenu : MonoBehaviour
{
    [SerializeField] private GameObject charactersUpgradePanel;
    [SerializeField] private GameObject bombsUpgradePanel;
    [SerializeField] private GameObject upgradesPanel;
    [SerializeField] private GameObject mainPanel;
    [SerializeField] private GameObject mainCam;
    [SerializeField] private GameObject shopCam;

    public void OpenCharacterUpgradePanel()
    {
        charactersUpgradePanel.SetActive(true);
        bombsUpgradePanel.SetActive(false);
        upgradesPanel.SetActive(false);
        mainPanel.SetActive(false);

        mainCam.SetActive(false);
        shopCam.SetActive(true);
    }

    public void OpenBombUpgradePanel()
    {
        charactersUpgradePanel.SetActive(false);
        bombsUpgradePanel.SetActive(true);
        upgradesPanel.SetActive(false);
        mainPanel.SetActive(false);

        mainCam.SetActive(true);
        shopCam.SetActive(false);
    }

    public void CloseUpgradesPanel()
    {
        charactersUpgradePanel.SetActive(false);
        bombsUpgradePanel.SetActive(false);
        upgradesPanel.SetActive(true);
        mainPanel.SetActive(true);

        mainCam.SetActive(true);
        shopCam.SetActive(false);
    }
}
