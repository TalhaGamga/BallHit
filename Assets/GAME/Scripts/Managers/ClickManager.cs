using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickManager : MonoBehaviour 
{
    [SerializeField] float maxSpeed;
    [SerializeField] float speedAcceleration;

    SpeedController speedController;
    private void Awake()
    {
        speedController = new SpeedController(maxSpeed, speedAcceleration);
    }

    private void Start()
    {
        speedController.OnEnable();
    }

    private void OnDisable()
    {
        speedController.OnDisable();
    }

    private void Update()
    {
        EventManager.OnMouseClickUp?.Invoke();
    }

    public void OnMouseClickDown()
    {
        EventManager.OnMouseClickDown?.Invoke();
    }
}