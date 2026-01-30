
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName ="CurrencyItem")]
public class BombPrice : ScriptableObject

{
    public List<CurrencyItem> currencyItems;
   
}

[System.Serializable]
public class CurrencyItem
{

    public PriceType curreny;
    public int price;
}