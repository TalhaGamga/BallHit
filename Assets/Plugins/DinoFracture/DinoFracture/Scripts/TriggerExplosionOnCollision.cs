using DinoFracture;
using UnityEngine;
using System.Threading;
using System.Collections;

namespace DinoFracture
{
    /// <summary>
    /// Triggers an explosion when an object hits this one
    /// </summary>
    public class TriggerExplosionOnCollision : MonoBehaviour
    {
        /// <summary>
        /// List of explosions to trigger
        /// </summary>
        public FractureGeometry[] Explosives;

        /// <summary>
        /// The force behind the explosions
        /// </summary>
        public float Force;

        /// <summary>
        /// The radius of the explosions
        /// </summary>
        public float Radius;

        private void OnCollisionEnter(Collision col)
        {
            AsyncFractureResult[] results = new AsyncFractureResult[Explosives.Length];

            for (int i = 0; i < Explosives.Length; i++)
            {
                if (Explosives[i] != null && Explosives[i].gameObject.activeSelf)
                {
                    results[i] = Explosives[i].Fracture();
                }
            }

            for (int i = 0; i < results.Length; i++)
            {
                if (results[i] != null)
                {
                    while (!results[i].IsComplete)
                    {
#if !UNITY_METRO || UNITY_EDITOR
                        Thread.Sleep(1);
#endif
                    }

                    Explode(results[i].PiecesRoot, results[i].EntireMeshBounds);
                }
            }
        }

        private void Explode(GameObject root, Bounds bounds)
        {
            Vector3 center = root.transform.localToWorldMatrix.MultiplyPoint(bounds.center);
            Transform rootTrans = root.transform;
            for (int i = 0; i < rootTrans.childCount; i++)
            {
                Transform pieceTrans = rootTrans.GetChild(i);
                Rigidbody body = pieceTrans.GetComponent<Rigidbody>();
                if (body != null)
                {
                    body.AddExplosionForce(Force, center, Radius);
                }
            }
        }
    }
}