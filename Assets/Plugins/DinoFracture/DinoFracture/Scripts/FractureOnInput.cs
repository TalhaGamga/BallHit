using DinoFracture;
using UnityEngine;
using System.Collections;

[RequireComponent(typeof(FractureGeometry))]
public class FractureOnInput : MonoBehaviour
{
    public KeyCode Key;

    void Update()
    {
        if (Input.GetKeyDown(Key))
        {
            GetComponent<FractureGeometry>().Fracture();
        }
    }
}
