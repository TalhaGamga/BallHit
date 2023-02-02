using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedController //Animation speed will change according to click freqeuency 
{
     float maxSpeed;
     float speedAcceleration;

    public SpeedController(float maxSpeed, float speedAcceleration) 
    {
        this.maxSpeed = maxSpeed;
        this.speedAcceleration = speedAcceleration;
    }

    private float timeScaler;

    private float TimeScaler
    {
        get
        {
            return timeScaler;
        }

        set
        {
            if (value >= maxSpeed)
            {
                timeScaler = maxSpeed;
            }

            else if (value <= 1) 
            {
                timeScaler = 1;
            }

            else { timeScaler = value; }
        }
    }

    float timer = 0;

    public void OnEnable()
    {
        EventManager.OnMouseClickDown += TimeScaleEnhancer;
        EventManager.OnMouseClickUp += TimeScaleReducer;
    }

    public void OnDisable()
    {
        EventManager.OnMouseClickDown -= TimeScaleEnhancer;
        EventManager.OnMouseClickUp -= TimeScaleReducer;
    }

    private void TimeScaleEnhancer()
    {
        TimeScaler += Time.deltaTime * speedAcceleration;

        Time.timeScale = TimeScaler;

        timer = 1f;
    }

    private void TimeScaleReducer()
    {
        timer -= Time.deltaTime;

        if (timer <=0)
        {
            TimeScaler -= Time.deltaTime * .5f;
            Time.timeScale = TimeScaler;
        }
    }
}
