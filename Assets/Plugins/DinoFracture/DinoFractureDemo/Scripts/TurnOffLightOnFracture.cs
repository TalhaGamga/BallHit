using DinoFracture;
using UnityEngine;
using System.Collections;

namespace DinoFractureDemo
{
    public class TurnOffLightOnFracture : MonoBehaviour
    {
        private void OnFracture(OnFractureEventArgs fractureRoot)
        {
            GetComponent<Renderer>().material.color = new Color(0.3f, 0.3f, 0.3f, 1.0f);
        }
    }
}