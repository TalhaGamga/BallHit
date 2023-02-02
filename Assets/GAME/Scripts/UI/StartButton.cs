using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;
public class StartButton : ButtonBase
{
    [SerializeField] private TextMeshProUGUI text;

    [SerializeField] Animator initBallAnim;

    float alpha;
    private void Start()
    {
        text.material.DOFade(0, 3);
    }

    public override void DoAction()
    {
        EventManager.OnStartingGame?.Invoke();

        GameManager.Instance.isGameStarted = true;

        initBallAnim.enabled = true;

        this.gameObject.SetActive(false);
    }
}
