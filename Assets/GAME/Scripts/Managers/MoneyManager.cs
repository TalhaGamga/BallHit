using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoneyManager : MonoBehaviour
{
    private float money;
    private float totalMoney;
    private float passiveIncomeMoney;
    private float incomeFactory = 1;

    [SerializeField] float increaseIncomeFactory;

    private int incomeLevel = 1;

    [SerializeField] private float clickMoney = 1;


    void OnEnable()
    {
        EventManager.IncrementMoney += IncrementMoney;

        EventManager.IncrementPerSecondMoney += IncrementPerSecondMoney;

        EventManager.DecrementPerSecondMoney += DecrementPerSecondMoney;

        EventManager.EnhancePerSecondMoneyFactory += EnhancePerSecondMoneyFactory;

        EventManager.OnPurchasingShopItem += Purchase;

        EventManager.OnGettingBreakingIncome += GetBreakingIncome;

        EventManager.OnSettingTotalMoney += SetMoney;

        EventManager.OnMouseClickDown += ClickIncome;

        EventManager.OnIncrementClickIncme += IncrementClickIncome;
    }

    private void OnDisable()
    {
        EventManager.IncrementMoney -= IncrementMoney;

        EventManager.IncrementPerSecondMoney -= IncrementPerSecondMoney;

        EventManager.DecrementPerSecondMoney -= DecrementPerSecondMoney;

        EventManager.EnhancePerSecondMoneyFactory -= EnhancePerSecondMoneyFactory;

        EventManager.OnPurchasingShopItem -= Purchase;

        EventManager.OnGettingBreakingIncome -= GetBreakingIncome;

        EventManager.OnSettingTotalMoney -= SetMoney;

        EventManager.OnMouseClickDown -= ClickIncome;
         
        EventManager.OnIncrementClickIncme -= IncrementClickIncome;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            totalMoney += 10000;
        }
    }

    private float SetMoney()
    {
        totalMoney += this.money + (passiveIncomeMoney * incomeFactory) * Time.deltaTime;

        return totalMoney;
    }
    
    private void IncrementMoney(float income) 
    {
        totalMoney += income;
    }

    private void IncrementPerSecondMoney(float income)
    {
        passiveIncomeMoney += income;
    }

    private void DecrementPerSecondMoney(float income)
    {
        passiveIncomeMoney -= income;
    }

    private void GetBreakingIncome(float income)
    {
        IncrementMoney(income);
    }

    private void EnhancePerSecondMoneyFactory()
    {
        incomeFactory += UpgradeData.Instance.CalculateExtraCostValue(incomeLevel, increaseIncomeFactory);
    }

    private void Purchase(ShopItemBase shopItemBase) 
    {
        float cost = shopItemBase.cost;

        if (totalMoney>cost)
        {
            if (shopItemBase.DoOperation())
            {
                totalMoney -= cost;

                shopItemBase.UpdateCurrentCost();
            }
        }
    }

    private void IncrementClickIncome(float increment)
    {
        clickMoney += increment;
    }

    private void ClickIncome()
    {
        IncrementMoney(clickMoney);
    }
}