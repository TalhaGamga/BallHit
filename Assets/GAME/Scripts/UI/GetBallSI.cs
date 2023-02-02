using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetBallSI : ShopItemBase
{   
    public GameObject mergeUi;

    public override bool DoOperation()
    {
        bool isAbleGotten = (bool)EventManager.GetBall?.Invoke(BallType.White); 


        if ((bool)EventManager.OnCheckingMergeable?.Invoke())
        {
            mergeUi.SetActive(true);
        }

        return isAbleGotten;
    }
}
 