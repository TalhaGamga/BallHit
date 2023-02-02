using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;
public class BallManager : MonoBehaviour
{
    [SerializeField] BallPool pool;

    Dictionary<BallType, List<BallBase>> scnBallGroups = new Dictionary<BallType, List<BallBase>>();

    List<BallBase> ballObjs = new List<BallBase>();

    [SerializeField] Transform spawnPoint;
    [SerializeField] Transform parent;

    private BallBase ball;

    [SerializeField] float radius;
    [SerializeField] float height = 0;

    BallType nextBallType;

    [SerializeField] Ease mergeEaseType;

    //[SerializeField] BallBase initBall;

    private void OnEnable()
    {
        EventManager.OnMergingBalls += Merge;

        //EventManager.OnSettingHeight += SetHeight;

        EventManager.GetBall += GetFromPool;

        EventManager.OnCheckingMergeable += CheckIsMergeable;

        EventManager.OnSettingHeight += SetHeight;
    }

    private void OnDisable() 
    {
        EventManager.OnMergingBalls -= Merge;

        //EventManager.OnSettingHeight -= SetHeight;

        EventManager.GetBall -= GetFromPool;

        EventManager.OnCheckingMergeable -= CheckIsMergeable;

        EventManager.OnSettingHeight -= SetHeight;
    }

    private void Start()
    {
        foreach (BallType ballType in pool.ballTypes)
        {
            scnBallGroups[ballType] = new List<BallBase>();
        }

        Init();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)) 
        {
            GetFromPool(BallType.White);

            SetPositions();
        }

        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            Merge();
        }
    }

    private void Init()
    {
        GetFromPool(BallType.White);
    }

    private bool GetFromPool(BallType ballType)
    {
        if (scnBallGroups[ballType].Count >= 10)
        {
            return false;
        }

        if (!scnBallGroups.ContainsKey(ballType))
        {
            return false;
        }

        BallBase ball = pool.Get(ballType);

        ball.ActivatePassiveIncome();

        ball.transform.position = spawnPoint.position;

        ball.transform.SetParent(parent);

        scnBallGroups[ball.ballType].Add(ball);

        ballObjs.Add(ball);

        ball.transform.localPosition = Vector3.zero;

        SetPositions();

        return true;
    }

    private bool CheckIsMergeable()
    {
        foreach (BallType ballType in pool.ballTypes)
        {
            if (scnBallGroups[ballType].Count >= 3)
            {
                return true;
            }
        }

        return false;
    }

    private void Kill(BallBase ball)
    {
        ball.DeactivatePassiveIncome();

        pool.Kill(ball);
    }

    private bool Merge()
    {
        foreach (BallType ballType in pool.ballTypes)
        {
            if (scnBallGroups[ballType].Count >= 3)
            {
                StartCoroutine(IEMerge(ballType));
                return true;
            }
        }

        return false;
    }

    IEnumerator IEMerge(BallType ballType)
    {
        for (int i = 0; i < 3; i++)
        {
            BallBase ball = scnBallGroups[ballType][0];

            nextBallType = ball.nextBallType;

            scnBallGroups[ballType].Remove(ball);

            ballObjs.Remove(ball);

            ball.anim.enabled = false;

            ball.ballObj.transform.DOMove(parent.transform.position + Vector3.up * 4.3f, 1f).SetEase(mergeEaseType).OnStepComplete(() =>
            {

                ball.Reset();

                Kill(ball);

            });
        }

        yield return new WaitForSeconds(1f);

        GetFromPool(nextBallType);

        if (!CheckIsMergeable())
        {
            EventManager.OnSettingMergeSIDeactivated?.Invoke();
        }

        SetPositions();

        yield return null;
    }


    private void SetHeight()
    {
        WallBase wall = EventManager.OnGettingPeek?.Invoke();

        parent.transform.DOMove(wall.transform.position, .5f);
    }

    public void SetPositions()
    {
        float num = ballObjs.Count;
        int cnt = 0;

        float angle = 360 / num;

        if (num == 2 || num == 3)
        {
            foreach (BallBase ball in ballObjs)
            {
                Vector3 pos = new Vector3(radius * Mathf.Cos(Mathf.Deg2Rad * cnt * angle), 0, radius * Mathf.Sin(Mathf.Deg2Rad * cnt * angle));

                ball.Replace(pos);

                cnt++;
            }

            return;
        }


        ballObjs[0].Replace(Vector3.zero);

        angle = 360 / (num - 1);

        for (int i = 1; i < num; i++)
        {
            Vector3 pos = new Vector3(radius * Mathf.Cos(Mathf.Deg2Rad * (i - 1) * angle), 0, radius * Mathf.Sin(Mathf.Deg2Rad * (i - 1) * angle));

            ballObjs[i].Replace(pos);
        }

        return;  
    }
}