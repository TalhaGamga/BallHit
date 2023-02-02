using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class WallPool : MonoBehaviour
{
    [SerializeField] private int initNum;

    [SerializeField] private List<WallBase> prefList;

    public List<WallBase> wallList = new List<WallBase>();

    [SerializeField] private Transform parent;

    public void Create(int initNum)
    {
        foreach (WallBase wall in prefList)
        {
            for (int i = 0; i < initNum; i++)
            {

                float yRot = Random.Range(0, 360);

                WallBase _wall = Instantiate(wall, parent);

                _wall.transform.Rotate(0, yRot, 0);

                _wall.gameObject.SetActive(false);

                wallList.Add(_wall);
            }
        }
    }

    public WallBase Get(int index)
    {
        WallBase wall = wallList[index];

        return wall;
    }

    public void Kill(WallBase wall)
    {
        wall.gameObject.SetActive(false);
    }
}