using System;
using UnityEngine;
public static class EventManager
{
    public static Action<WallBase> OnKillingWall;

    public static Func<bool> OnMergingBalls;

    public static Action OnSettingHeight;

    public static Func<WallBase> OnGettingPeek;

    public static Func<WallBase> OnGettingWall;

    public static Func<BallType, bool> GetBall;

    public static Action<float> IncrementMoney;

    public static Action<float> IncrementPerSecondMoney;

    public static Action<float> DecrementPerSecondMoney;

    public static Action<float> OnGettingBreakingIncome;

    public static Action EnhancePerSecondMoneyFactory;

    public static Action<ShopItemBase> OnPurchasingShopItem;

    public static Func<bool> OnCheckingMergeable;

    public static Func<long, string> OnGettingAbbreviation;

    public static Func<float> OnSettingTotalMoney;

    public static Action OnMouseClickDown;

    public static Action OnMouseClickUp;

    public static Action OnSettingMergeSIDeactivated;

    public static Action OnStartingGame;

    public static Action<float> OnIncrementClickIncme;
}