using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
public class WallPiece : MonoBehaviour
{
    Vector3 originalPos;
    Quaternion originalRotation;
    Vector3 originalScale;

    Vector3 dir;

    [SerializeField] Rigidbody rb;

    float multier = .05f;

    [SerializeField] WallBase parent;

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        originalPos = transform.localPosition;
        originalRotation = transform.localRotation;
        originalScale = transform.localScale;
    }


    public void ProceduralBreaking(float rate)
    {
        dir = (Vector3.zero - transform.localPosition).normalized;

        transform.localPosition -= dir * rate * multier;
    }


    public void ResetPiece()
    {
        rb.isKinematic = true;

        transform.localPosition = originalPos;
        transform.localRotation = originalRotation;
        transform.localScale = originalScale;
    }

    public void Explode(Vector3 center)
    {
        rb.isKinematic = false;

        Vector3 dir = (transform.position - center).normalized;

        rb.AddForce(10 * dir+Vector3.up*3, ForceMode.Impulse);

        transform.DOScale(Vector3.one * .2f, 3);
    }
}