using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallPool :MonoBehaviour
{
    public List<BallType> ballTypes = new List<BallType>();

    Dictionary<BallType, Queue<BallBase>> ballDict = new Dictionary<BallType, Queue<BallBase>>();

    [SerializeField] private List<BallBase> prefList;
    [SerializeField] private Transform parent;
    [SerializeField] int num;

    private void Awake()
    {
        foreach (BallType ballType in ballTypes)
        {
            ballDict[ballType] = new Queue<BallBase>();
        }

        Init();
    }

    private void Init()
    {
        foreach (BallBase ball in prefList)
        {
            for (int i = 0; i < num; i++)
            {
                BallBase _ball = Instantiate(ball, parent);

                ballDict[_ball.ballType].Enqueue(_ball);

                _ball.gameObject.SetActive(false);
            }
        }
    }

    public BallBase Get(BallType ballType)
    {
        BallBase ball = ballDict[ballType].Dequeue();

        ballDict[ballType].Enqueue(ball);

        ball.gameObject.SetActive(true);

        return ball;
    }

    public void Kill(BallBase ball)
    {
        ball.transform.SetParent(parent);

        ball.gameObject.SetActive(false);
    }
}
