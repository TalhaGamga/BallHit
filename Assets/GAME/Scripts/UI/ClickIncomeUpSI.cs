using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickIncomeUpSI : ShopItemBase
{
    int incomeLevel = 2;
    [SerializeField] float incomeIncrementFactor;
    public override bool DoOperation()
    {
        int upgradedClickIncomme = UpgradeData.Instance.CalculateExtraCostValue(incomeLevel, incomeIncrementFactor);

        EventManager.OnIncrementClickIncme?.Invoke(upgradedClickIncomme);

        incomeLevel++;

        UpdateCurrentCost();
         
        return true;
    }
}
 