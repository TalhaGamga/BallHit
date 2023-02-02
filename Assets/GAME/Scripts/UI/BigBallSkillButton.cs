using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class BigBallSkillButton : MonoBehaviour
{
    [SerializeField] private bool isReady = false;

    [SerializeField] private float coolDown;

    private float currentCoolDown;

    [SerializeField] GameObject bigBall;

    [SerializeField] private TextMeshProUGUI coolDownText;

    private void Start()
    {
        currentCoolDown = coolDown;
    }
    void Update()
    {
        CoolDown();
    }

    private void CoolDown()
    {
        if (!isReady)
        {
            currentCoolDown -= Time.deltaTime;

            coolDownText.text = EventManager.OnGettingAbbreviation?.Invoke((long)currentCoolDown);

            if (currentCoolDown <= 0)
            {
                isReady = true;

                coolDownText.text = "READY";
            }
        }
    }

    public void UseBigBall()
    {
        if (isReady)
        {
            bigBall.SetActive(true);

            currentCoolDown = coolDown;

            isReady = false;
        }
    }
}
