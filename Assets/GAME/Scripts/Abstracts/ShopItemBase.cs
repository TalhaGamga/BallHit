using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public abstract class ShopItemBase : MonoBehaviour
{
    public float cost;
    [SerializeField] TextMeshProUGUI price;

    private float startCost;

    private int priceUpdateLevel = 1;

    [SerializeField] private float increaseFactor;

    private void Awake()
    {
        price.text = cost.ToString();

        startCost = cost;
    }

    public void UpdatePrice()
    {
        cost *= 1.1f;

        price.text = EventManager.OnGettingAbbreviation?.Invoke((long)cost);
    }

    public void UpdateCurrentCost()
    {
        cost += UpgradeData.Instance.CalculateExtraCostValue(priceUpdateLevel, increaseFactor);

        price.text = EventManager.OnGettingAbbreviation?.Invoke((long)cost);

        priceUpdateLevel++;
    }

    public abstract bool DoOperation();
}
