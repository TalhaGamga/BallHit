using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class MergeSI : ShopItemBase
{
    private void OnEnable()
    {
        EventManager.OnSettingMergeSIDeactivated += SetDeactivated;
    }

    private void OnDisable()
    {
        EventManager.OnSettingMergeSIDeactivated -= SetDeactivated;
    }

    public override bool DoOperation()
    {
        bool isMerged = (bool)EventManager.OnMergingBalls?.Invoke();
        
        return isMerged;
    }

    private void SetDeactivated()
    {
        gameObject.SetActive(false);
    }
}
