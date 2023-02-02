using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class GameManager : Singleton<GameManager>
{
    public bool isGameStarted = false;

    private void Start()
    {
        Application.targetFrameRate = 60;
    }
}
  