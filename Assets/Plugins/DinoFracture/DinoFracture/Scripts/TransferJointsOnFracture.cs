using System;
using System.Collections;
using System.Reflection;
using UnityEngine;

#if UNITY_METRO && !UNITY_EDITOR
using System.Linq;
#endif

namespace DinoFracture
{
    /// <summary>
    /// When this object is fractured, the joint component on the
    /// object will be copied to this piece if this piece is sufficiently
    /// close to the joint position.  Without this component, joints
    /// are broken after fracturing.
    /// </summary>
    public class TransferJointsOnFracture : MonoBehaviour
    {
        /// <summary>
        /// The tree to crawl in search for joints of other objects that need 
        /// to be transferred to this joint.  This search root should be as
        /// scoped as possible.
        /// </summary>
        public Transform IncomingJointsSearchRoot;

        /// <summary>
        /// How close this object must be to the joint in order to transfer.
        /// The larger the number, the more pieces will have joints transferred.
        /// </summary>
        public float DistanceTolerance = 0.05f;

        private void OnFracture(OnFractureEventArgs args)
        {
            if (args.OriginalObject.gameObject == gameObject)
            {
                TransferOurJoint(args);
                RewriteOtherJoints(args);
            }
        }

        private void TransferOurJoint(OnFractureEventArgs args)
        {
            Joint origJoint = args.OriginalObject.GetComponent<Joint>();
            if (origJoint != null)
            {
                Vector3 worldPoint =
                    args.OriginalObject.transform.localToWorldMatrix.MultiplyPoint(origJoint.anchor);

                Transform rootTrans = args.FracturePiecesRootObject.transform;
                for (int i = 0; i < rootTrans.childCount; i++)
                {
                    Transform pieceTrans = rootTrans.GetChild(i);
                    Collider ourCollider = pieceTrans.GetComponent<Collider>();
                    Vector3 closestOnUs = (ourCollider != null)
                        ? ourCollider.ClosestPointOnBounds(worldPoint)
                        : transform.position;

                    if ((worldPoint - pieceTrans.position).sqrMagnitude <
                        (closestOnUs - pieceTrans.position).sqrMagnitude + DistanceTolerance * DistanceTolerance)
                    {
                        Joint ourJoint = (Joint)pieceTrans.gameObject.AddComponent(origJoint.GetType());
                        string ourName = ourJoint.name;

                        // Copy the properties
#if UNITY_METRO && !UNITY_EDITOR
                        foreach (
                            PropertyInfo info in
                                origJoint.GetType().GetRuntimeProperties())
                        {
                            if (info.CanWrite && info.CanRead)
                            {
                                info.SetValue(ourJoint, info.GetValue(origJoint, null), null);
                            }
                        }
#else
                        foreach (
                            PropertyInfo info in
                                origJoint.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance))
                        {
                            if (info.CanWrite && info.CanRead)
                            {
                                info.SetValue(ourJoint, info.GetValue(origJoint, null), null);
                            }
                        }
                        foreach (
                            PropertyInfo info in
                                origJoint.GetType().GetProperties(BindingFlags.Instance | BindingFlags.NonPublic))
                        {
                            if (info.CanWrite && info.CanRead &&
                                info.GetCustomAttributes(typeof (SerializeField), true).Length != 0)
                            {
                                info.SetValue(ourJoint, info.GetValue(origJoint, null), null);
                            }
                        }
#endif

                        ourJoint.name = ourName;

                        // Reanchor
                        ourJoint.anchor = pieceTrans.worldToLocalMatrix.MultiplyPoint(worldPoint);

                        Vector3 connectedAnchorWorldPoint =
                            args.OriginalObject.transform.localToWorldMatrix.MultiplyPoint(origJoint.connectedAnchor);
                        ourJoint.connectedAnchor = pieceTrans.worldToLocalMatrix.MultiplyPoint(connectedAnchorWorldPoint);

                        Vector3 worldAxis =
                            args.OriginalObject.transform.localToWorldMatrix.MultiplyVector(origJoint.axis);
                        ourJoint.axis = pieceTrans.worldToLocalMatrix.MultiplyVector(worldAxis).normalized;
                    }

                    // Make sure each piece has one of these scripts attached
                    if (pieceTrans.GetComponent<TransferJointsOnFracture>() == null)
                    {
                        TransferJointsOnFracture tjof = pieceTrans.gameObject.AddComponent<TransferJointsOnFracture>();
                        tjof.IncomingJointsSearchRoot = IncomingJointsSearchRoot;
                        tjof.DistanceTolerance = DistanceTolerance;
                    }
                }
            }
        }

        private void RewriteOtherJoints(OnFractureEventArgs args)
        {
            Joint[] joints;
            if (IncomingJointsSearchRoot != null)
            {
                joints = IncomingJointsSearchRoot.GetComponentsInChildren<Joint>();
            }
            else
            {
                joints = FindObjectsOfType<Joint>();
            }

            if (joints != null)
            {
                for (int i = 0; i < joints.Length; i++)
                {
                    if (joints[i] != null && joints[i].connectedBody == args.OriginalObject.GetComponent<Rigidbody>())
                    {
                        Transform rootTrans = args.FracturePiecesRootObject.transform;
                        for (int j = 0; j < rootTrans.childCount; j++)
                        {
                            Transform pieceTrans = rootTrans.GetChild(j);

                            Vector3 worldPoint =
                                joints[i].transform.localToWorldMatrix.MultiplyPoint(joints[i].anchor);

                            Collider ourCollider = pieceTrans.GetComponent<Collider>();
                            Vector3 closestOnUs = (ourCollider != null)
                                ? ourCollider.ClosestPointOnBounds(worldPoint)
                                : transform.position;

                            if ((worldPoint - pieceTrans.position).sqrMagnitude <
                                (closestOnUs - pieceTrans.position).sqrMagnitude + DistanceTolerance * DistanceTolerance)
                            {
                                joints[i].connectedBody = pieceTrans.GetComponent<Rigidbody>();
                            }
                        }
                    }
                }
            }
        }
    }
}