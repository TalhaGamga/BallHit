using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DinoFracture
{
    /// <summary>
    /// This is the base class for the PreFractureGeometry and RuntimeFractureGeometry components.
    /// As such, it is not intended to be directly added to any game object even though fracture
    /// initiator components rely on it.
    /// </summary>
    public abstract class FractureGeometry : MonoBehaviour
    {
        /// <summary>
        /// The material assigned to the “inside” triangles of the fracture pieces.
        /// These are the triangles that DinoFracture creates.  The surface triangles 
        /// of the original mesh retain their materials.
        /// </summary>
        public Material InsideMaterial;

        /// <summary>
        /// This game object will be cloned for each facture piece.  It is required to
        /// have a MeshFilter component.  If a MeshCollider component is added, it will
        /// be assigned the fracture mesh.
        /// </summary>
        public GameObject FractureTemplate;

        /// <summary>
        /// The parent of the generated pieces.  Each fracture produces a root object
        /// with fracture pieces (clones of FractureTemplate) as children.  The root
        /// object is parented to PiecesParent.
        /// </summary>
        public Transform PiecesParent;

        /// <summary>
        /// The number of fracture pieces generated per iteration.  Fault lines are
        /// spread evenly around the fracture point.  The number of total pieces
        /// generated is NumFracturePieces ^ NumIterations.
        /// </summary>
        public int NumFracturePieces = 5;

        /// <summary>
        /// The number of passes of fracturing.  Using lower piece count with a higher
        /// iteration count is computationally faster than a higher piece count with a lower
        /// iteration count.  Ex: 5 pieces with 2 iterations is faster than 25 pieces and 
        /// 1 iteration.  The downside to using more iterations is fractures can become 
        /// less uniform.  In general, keep this number below 4.  The number of total pieces 
        /// generated is NumFracturePieces ^ NumIterations.    
        /// </summary>
        /// <remarks>It is recommended you use an iteration count of 1 when 0 &lt; FractureRadius &lt; 1.</remarks>
        public int NumIterations = 2;

        /// <summary>
        /// To allow for fracture pieces to be further fractured, the FractureTemplate should
        /// have a FractureGeometry component.  NumGenerations dictates how many times the
        /// geometry can be re-fractured.  The count is decremented and passed on to the 
        /// component in each generated piece.  Ex: A value of 2 means this piece can be
        ///  fractured and each generated piece can be fractured.  The second generation 
        /// of fractures cannot be fractured further.  
        /// </summary>
        public int NumGenerations = 1;

        /// <summary>
        /// A value between 0 and 1 that indicates how clustered the fracture lines are.  A
        /// value of 0 or 1 means fractures are evenly distributed across the mesh.  A value
        /// between means they are clustered within a percentage of the mesh bounds.
        /// Ex: a value of 0.3 means fractures are clustered around the fracture point in a 
        /// volume 30% the size of the mesh.  Pre-fracture geometry typically has this value
        /// set to 0 or 1 because there isn’t always a pre-determined point of fracture.
        /// </summary>
        public float FractureRadius;

        /// <summary>
        /// If set to EntireMesh, the UV map for each inside triangle will be mapped to a box 
        /// the size of the original mesh.  If set to piece, inside triangles will be mapped to 
        /// a box the size of the individual fracture piece.
        /// </summary>
        public FractureUVScale UVScale = FractureUVScale.Piece;

        /// <summary>
        /// If true and both this game object and the FractureTemplate have a RigidBody component,
        /// each fracture piece will have a mass set to a value proportional to its volume.
        /// That is, the density of the fracture piece will equal the density of the original mesh.
        /// If false, the mass property goes untouched.
        /// </summary>
        public bool DistributeMass = true;

        private bool _processingFracture = false;

        /// <summary>
        /// Initiate a fracture at the origin
        /// </summary>
        /// <returns></returns>
        public AsyncFractureResult Fracture()
        {
            if (NumGenerations == 0 || _processingFracture)
            {
                return null;
            }

            return FractureInternal(Vector3.zero);
        }

        /// <summary>
        /// Initiate a fracture at the specified position relative to this object.
        /// </summary>
        /// <param name="localPos"></param>
        /// <returns></returns>
        public AsyncFractureResult Fracture(Vector3 localPos)
        {
            if (NumGenerations == 0 || _processingFracture)
            {
                return null;
            }

            return FractureInternal(localPos);
        }

        protected AsyncFractureResult Fracture(FractureDetails details, bool hideAfterFracture)
        {
            if (NumGenerations == 0 || _processingFracture)
            {
                return null;
            }

            if (FractureTemplate == null || FractureTemplate.GetComponent<MeshFilter>() == null)
            {
                Debug.LogError("DinoFracture: A fracture template with a MeshFilter component is required.");
            }

            _processingFracture = true;

            if (details.Mesh == null)
            {
                MeshFilter meshFilter = GetComponent<MeshFilter>();
                SkinnedMeshRenderer skinnedRenderer = GetComponent<SkinnedMeshRenderer>();

                if (meshFilter == null && skinnedRenderer == null)
                {
                    Debug.LogError("DinoFracture: A mesh filter required if a mesh is not supplied.");
                    return null;
                }

                Mesh mesh;
                if (meshFilter != null)
                {
                    mesh = meshFilter.sharedMesh;
                }
                else
                {
                    mesh = new Mesh();
                    skinnedRenderer.BakeMesh(mesh);
                }

                details.Mesh = mesh;
            }

            if (details.MeshScale == Vector3.zero)
            {
                details.MeshScale = transform.localScale;
            }

            // Unassigned transforms aren't actually assigned to null by Unity, so we need check for it here.
            Transform piecesParent = (PiecesParent == null) ? null : PiecesParent;

            return FractureEngine.StartFracture(details, this, piecesParent, DistributeMass, hideAfterFracture);
        }

        protected void StopFracture()
        {
            _processingFracture = false;
        }

        protected abstract AsyncFractureResult FractureInternal(Vector3 localPos);

        /// <summary>
        /// Called when this object is fractured or spawned as a result of a fracture
        /// </summary>
        /// <param name="args"></param>
        internal virtual void OnFracture(OnFractureEventArgs args)
        {
            if (args.OriginalObject == this)
            {
                _processingFracture = false;
            }
        }
    }
}
