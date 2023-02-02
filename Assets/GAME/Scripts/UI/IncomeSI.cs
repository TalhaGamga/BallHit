using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IncomeSI : ShopItemBase
{
    public override bool DoOperation()
    {
        EventManager.EnhancePerSecondMoneyFactory?.Invoke();;

        UpdateCurrentCost();

        return true;
    }
}
 