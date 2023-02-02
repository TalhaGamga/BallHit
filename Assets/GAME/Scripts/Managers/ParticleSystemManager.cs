using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleSystemManager : MonoBehaviour
{
    [SerializeField] ParticleSystem hitEffect;

    [SerializeField] ParticleSystem mergeEffect;

    private void PlayHitEffect(Color color, Vector3 pos)
    {
        hitEffect.startColor = color;
        mergeEffect.transform.position = pos;
        hitEffect.Play();
    }


    private void PlayMergeEffect(Color color, Vector3 pos)
    {
        mergeEffect.startColor = color;
        mergeEffect.transform.position = pos;
        mergeEffect.Play();
    }
}
