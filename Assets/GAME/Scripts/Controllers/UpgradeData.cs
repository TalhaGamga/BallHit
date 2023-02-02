using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeData : Singleton<UpgradeData>
{
    public int CalculateExtraCostValue(int _level, float _increaseFactor)
    {
        int _sum = 0;

        for (int i = 1; i <= _level; i++)
        {
            _sum += (int)(i * _increaseFactor);
        }

        return _sum;
    }

}
