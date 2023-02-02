using System;
using System.Collections;
using UnityEngine;

namespace DinoFracture
{
    /// <summary>
    /// This component will cause a fracture to happen at the point of impact.
    /// </summary>
    [RequireComponent(typeof (FractureGeometry))]
    public class FractureOnCollision : MonoBehaviour
    {
        /// <summary>
        /// The minimum amount of force required to fracture this object.
        /// Set to 0 to have any amount of force cause the fracture.
        /// </summary>
        public float ForceThreshold;

        /// <summary>
        /// Falloff radius for transferring the force of the impact
        /// to the resulting pieces.  Any piece outside of this falloff
        /// from the point of impact will have no additional impulse
        /// set on it.
        /// </summary>
        public float ForceFalloffRadius = 1.0f;

        /// <summary>
        /// If true and this is a kinematic body, an impulse will be
        /// applied to the colliding body to counter the effects of'
        /// hitting a kinematic body.  If false and this is a kinematic
        /// body, the colliding body will bounce off as if this were an
        /// unmovable wall.
        /// </summary>
        public bool AdjustForKinematic = true;

        private Vector3 _impactVelocity;
        private float _impactMass;
        private Vector3 _impactPoint;
        private Rigidbody _impactBody;

        private void OnCollisionEnter(Collision col)
        {
            if (col.contacts.Length > 0)
            {
                _impactBody = col.rigidbody;
                _impactMass = (col.rigidbody != null) ? col.rigidbody.mass : 1.0f;
                _impactVelocity = col.relativeVelocity;

                Rigidbody rb = GetComponent<Rigidbody>();
                if (rb != null)
                {
                    // Always have the impact velocity point in our moving direction
                    _impactVelocity *= Mathf.Sign(Vector3.Dot(rb.velocity, _impactVelocity));
                }

                float mag = _impactVelocity.magnitude;
                Vector3 force = 0.5f * _impactMass * _impactVelocity * mag;

                if ((ForceThreshold * ForceThreshold) <=
                    force.sqrMagnitude)
                {
                    _impactPoint = Vector3.zero;

                    for (int i = 0; i < col.contacts.Length; i++)
                    {
                        _impactPoint += col.contacts[i].point;
                    }
                    _impactPoint *= 1.0f / col.contacts.Length;

                    Vector3 localPoint = transform.worldToLocalMatrix.MultiplyPoint(_impactPoint);

                    GetComponent<FractureGeometry>().Fracture(localPoint);
                }
            }
        }

        private void OnFracture(OnFractureEventArgs args)
        {
            if (args.OriginalObject.gameObject == gameObject)
            {
                float radiusSq = ForceFalloffRadius * ForceFalloffRadius;

                for (int i = 0; i < args.FracturePiecesRootObject.transform.childCount; i++)
                {
                    Transform piece = args.FracturePiecesRootObject.transform.GetChild(i);

                    Rigidbody rb = piece.GetComponent<Rigidbody>();
                    if (rb != null)
                    {
                        Vector3 force = _impactMass * _impactVelocity / (rb.mass + _impactMass);

                        if (ForceFalloffRadius > 0.0f)
                        {
                            float distSq = (piece.position - _impactPoint).sqrMagnitude;
                            force *= Mathf.Clamp01(1.0f - distSq / (radiusSq));
                        }

                        rb.AddForceAtPosition(force * rb.mass, _impactPoint, ForceMode.Impulse);
                    }
                }

                if (AdjustForKinematic)
                {
                    // If the fractured body is kinematic, the collision for the colliding body will
                    // be as if it hit an unmovable wall.  Try to correct for that by adding the same
                    // force to colliding body.
                    Rigidbody thisBody = GetComponent<Rigidbody>();
                    if (thisBody != null && thisBody.isKinematic && _impactBody != null)
                    {
                        Vector3 force = thisBody.mass * _impactVelocity / (thisBody.mass + _impactMass);
                        _impactBody.AddForceAtPosition(force * _impactMass, _impactPoint, ForceMode.Impulse);
                    }
                }
            }
        }
    }
}
