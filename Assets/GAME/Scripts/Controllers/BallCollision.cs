using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class BallCollision : MonoBehaviour
{
    [SerializeField] UnityEvent<WallBase> OnTouchingWall;

    bool isCollided = false;

    float timer = 0;
    private void OnCollisionEnter(Collision collision)
    {
        if (!isCollided && collision.gameObject.TryGetComponent<WallBase>(out WallBase wall))
        {
            isCollided = true;

            StartCoroutine(ResetIsCollided());

            OnTouchingWall?.Invoke(wall);
        }
    }

    private IEnumerator ResetIsCollided()
    {
        yield return new WaitForSeconds(.2f);
        isCollided = false;
    }
}
