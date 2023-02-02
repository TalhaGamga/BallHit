using DinoFracture;
using UnityEngine;
using System.Collections;

namespace DinoFractureDemo
{
    [RequireComponent(typeof (FractureGeometry))]
    [RequireComponent(typeof (Collider))]
    public class FractureOnClick : MonoBehaviour
    {
        // Update is called once per frame
        private void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                RaycastHit hit;
                if (GetComponent<Collider>().Raycast(ray, out hit, 100.0f))
                {
                    Vector3 localPoint = this.transform.worldToLocalMatrix.MultiplyPoint(hit.point);
                    GetComponent<FractureGeometry>().Fracture(localPoint);
                }
            }
        }
    }
}
