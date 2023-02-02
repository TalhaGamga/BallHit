using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallManager : MonoBehaviour
{
    [SerializeField] WallPool pool;

    [SerializeField] Transform spawnPoint;

    [SerializeField] int initNum;

    int index = 0;

    int counter;
    private void OnEnable()
    {
        EventManager.OnKillingWall += Kill;

        //EventManager.OnReplacingWall += ReplaceWall;

        EventManager.OnGettingWall += GetWall;

        EventManager.OnGettingPeek += GetPeekWall;
    }

    private void OnDisable()
    {
        EventManager.OnKillingWall -= Kill;

        //EventManager.OnReplacingWall -= ReplaceWall;

        EventManager.OnGettingWall -= GetWall;

        EventManager.OnGettingPeek -= GetPeekWall;
    }

    private void Start()
    {
        Create();

        counter = pool.wallList.Count;

        InitSpawn();
    }

    private void Create()
    {
        pool.Create(initNum);
    }

    private void InitSpawn()
    {
        for (int i = 0; i < initNum; i++)
        {
            WallBase wall = pool.Get(i);

            wall.transform.position = spawnPoint.position;

            spawnPoint.position = wall.stackOffSet.position;

            wall.gameObject.SetActive(true);
        }
    }

    private WallBase GetWall()
    {
        WallBase wall = pool.Get(initNum);

        WallBase firstWall = pool.wallList[0];

        pool.wallList.Remove(firstWall);

        pool.wallList.Add(firstWall);

        wall.transform.position = spawnPoint.position;

        spawnPoint.position = wall.stackOffSet.position;

        wall.gameObject.SetActive(true);

        index++;

        return wall;   
    }

    //private void ReplaceWall(WallBase wall)
    //{
    //    wall.transform.position = spawnPoint.position;

    //    spawnPoint.position = wall.stackOffSet.position;
    //}

    private void Kill(WallBase wall)
    {
        pool.Kill(wall);
    }

    private WallBase GetPeekWall() 
    {
        return pool.wallList[0]; 
    }
}