using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestCoins : MonoBehaviour
{
    public int CoinsAmt, GemsAmt;
    // Start is called before the first frame update
    void Start()
    {

       
    }

    [ContextMenu("IncreaseCoins")]
    public void TestCoinsFn()
    {
        PlayerPrefs.SetInt("Coins", CoinsAmt);
        UIManager.instance.SetCoinsText();
    }


    [ContextMenu("IncreaseGems")]
    public void TestGemsFn()
    {
        PlayerPrefs.SetInt("Gems", GemsAmt);
        UIManager.instance.SetCoinsText();

    }
}
