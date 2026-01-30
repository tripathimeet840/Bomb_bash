
using UnityEngine;

public class PlayerBombsHandler : MonoBehaviour
{
    public BombInfo[] allBombs;
    public BombPrice bombPrice;

    private void OnEnable()
    {

        for (int i = 0; i < allBombs.Length; i++)
        {
            allBombs[i].Price = bombPrice.currencyItems[i].price;
            allBombs[i].priceType = bombPrice.currencyItems[i].curreny;
        }
        
    }
}
