using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class BigBall : MonoBehaviour
{
    [SerializeField] float damage;
    [SerializeField] Ease easeType;

    private void OnEnable()
    {
        transform.position = (Vector3)EventManager.OnGettingPeek?.Invoke().transform.position + Vector3.up * 9;

        transform.DOMoveY(transform.position.y-35, 1f).SetEase(easeType).OnComplete(()=>gameObject.SetActive(false));
    }

    public void Attack(WallBase wall)
    {
        wall.TakeDamage(damage);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.TryGetComponent<WallBase>(out WallBase wall))
        {
            wall.TakeDamage(damage);
        }
    }
}
