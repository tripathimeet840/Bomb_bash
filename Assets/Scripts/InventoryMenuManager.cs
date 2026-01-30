using UnityEngine;

public class InventoryMenuManager : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] private GameObject characterInventoryPanel;
    [SerializeField] private GameObject bombsInventoryPanel;
    [SerializeField] private GameObject mainPanel;

    [Header("Cameras")]
    [SerializeField] private GameObject mainCam;
    [SerializeField] private GameObject inventoryCam;

    [Header("Inventory Objects")]
    [SerializeField] private GameObject characterObjects;
    [SerializeField] private GameObject bombObjects;

    public void OpenInventory()
    {
        mainPanel.SetActive(false);
        characterInventoryPanel.SetActive(true);
        bombsInventoryPanel.SetActive(false);

        mainCam.SetActive(false);
        inventoryCam.SetActive(true);

        characterObjects.SetActive(true);
        bombObjects.SetActive(false);
    }

    public void CloseInventory()
    {
        mainPanel.SetActive(true);
        characterInventoryPanel.SetActive(false);
        bombsInventoryPanel.SetActive(false);

        mainCam.SetActive(true);
        inventoryCam.SetActive(false);

        characterObjects.SetActive(true);
        bombObjects.SetActive(false);


    }

    public void OpenBombsInventory()
    {
        characterInventoryPanel.SetActive(false);
        bombsInventoryPanel.SetActive(true);

        characterObjects.SetActive(false);
        bombObjects.SetActive(true);
    }

    public void OpenCharacterInventory()
    {
        characterInventoryPanel.SetActive(true);
        bombsInventoryPanel.SetActive(false);

        characterObjects.SetActive(true);
        bombObjects.SetActive(false);
    }
}
