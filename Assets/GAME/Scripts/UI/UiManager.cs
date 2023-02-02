using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using DG.Tweening;

public class UiManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI moneyText;

    [SerializeField] private TextMeshProUGUI clickIncomeText;
    [SerializeField] private Transform clickIncomeParent;
    [SerializeField] private RectTransform clickIncomeRect;

    [SerializeField] private RectTransform canvasRectTransform;
    [SerializeField] private RectTransform clickSign;

    [SerializeField] Camera mainCamera;

    float totalMoney;

    [SerializeField] float clickMoney = 1;

    private void OnEnable()
    {
        EventManager.OnGettingAbbreviation += GetAbbreviation;

        EventManager.OnIncrementClickIncme += IncrementClickIncome;

        EventManager.OnMouseClickDown += ClickIncomeEffect;

        EventManager.OnMouseClickDown += SignClickedPoint;
    }

    private void OnDisable()
    {
        EventManager.OnGettingAbbreviation -= GetAbbreviation;

        EventManager.OnIncrementClickIncme -= IncrementClickIncome;

        EventManager.OnMouseClickDown -= ClickIncomeEffect;

        EventManager.OnMouseClickDown -= SignClickedPoint;
    }

    private void Update()
    {
        SetUiMoney();

    }

    public void SetUiMoney()
    {
        totalMoney = (float)EventManager.OnSettingTotalMoney?.Invoke();
        moneyText.text = GetAbbreviation((long)totalMoney) + "$";
    }

    public static string GetAbbreviation(long _value)
    {
        if (_value >= 100000000)
        {
            return (_value / 1000000D).ToString("0.#M");
        }
        if (_value >= 1000000)
        {
            return (_value / 1000000D).ToString("0.##M");
        }
        if (_value >= 100000)
        {
            return (_value / 1000D).ToString("0.#k");
        }
        if (_value >= 1000)
        {
            return (_value / 1000D).ToString("0.##k");
        }

        if (_value == 0)
        {
            return "0";
        }

        return _value.ToString("#");
    }

    public void Buy(ShopItemBase item)
    {
        EventManager.OnPurchasingShopItem?.Invoke(item);
    }

    public void StartGame(ButtonBase button)
    {
        button.DoAction();
    }

    private void IncrementClickIncome(float increment)
    {
        clickMoney += increment;
    }

    private void ClickIncomeEffect()
    {
        Vector2 localPoint;
        
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasRectTransform,
            Input.mousePosition,
            mainCamera,
            out localPoint
            );

        TextMeshProUGUI _clickIncomeText = Instantiate(clickIncomeText, clickIncomeParent);
        RectTransform _clickIncomeRect = _clickIncomeText.GetComponent<RectTransform>();

        _clickIncomeText.text = clickMoney.ToString() + " $";

        _clickIncomeRect.anchoredPosition = localPoint + new Vector2(Random.Range(-100,100), Random.Range(-100, 100));

        _clickIncomeRect.DOLocalMoveY(localPoint.y + 100, .75f).OnStepComplete(()=>Destroy(_clickIncomeText.gameObject));
    }

    private void SignClickedPoint()
    {
        clickSign.gameObject.SetActive(true);

        clickSign.localScale = Vector3.one;

        Vector2 localPoint;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasRectTransform,
            Input.mousePosition,
            mainCamera,
            out localPoint
            );

        clickSign.anchoredPosition = localPoint;

        clickSign.DOScale(Vector3.one * 0.25f, 0.2f).OnStepComplete(()=>clickSign.gameObject.SetActive(false));
    }
}
