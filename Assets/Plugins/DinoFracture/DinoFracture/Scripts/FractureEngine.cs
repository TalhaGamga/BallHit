using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace DinoFracture
{
    /// <summary>
    /// Argument passed to OnFracture message
    /// </summary>
    public sealed class OnFractureEventArgs
    {
        public OnFractureEventArgs(FractureGeometry orig, GameObject root)
        {
            OriginalObject = orig;
            FracturePiecesRootObject = root;
        }

        /// <summary>
        /// The object that fractured.
        /// </summary>
        public FractureGeometry OriginalObject;

        /// <summary>
        /// The root of the pieces of the resulting fracture.
        /// </summary>
        public GameObject FracturePiecesRootObject;
    }

    /// <summary>
    /// The result of a fracture.
    /// </summary>
    public sealed class AsyncFractureResult
    {
        /// <summary>
        /// Returns true if the operation has finished; false otherwise.
        /// This value will always be true for synchronous fractures.
        /// </summary>
        public bool IsComplete { get; private set; }

        /// <summary>
        /// The root of the pieces of the resulting fracture
        /// </summary>
        public GameObject PiecesRoot { get; private set; }

        /// <summary>
        /// The bounds of the original mesh
        /// </summary>
        public Bounds EntireMeshBounds { get; private set; }

        internal bool StopRequested { get; private set; }

        internal void SetResult(GameObject rootGO, Bounds bounds)
        {
            if (IsComplete)
            {
                UnityEngine.Debug.LogWarning("DinoFracture: Setting AsyncFractureResult's results twice.");
            }
            else
            {
                PiecesRoot = rootGO;
                EntireMeshBounds = bounds;
                IsComplete = true;
            }
        }

        public void StopFracture()
        {
            StopRequested = true;
        }
    }

    /// <summary>
    /// This component is created on demand to manage the fracture coroutines.
    /// It is not intended to be added by the user.
    /// </summary>
    public sealed class FractureEngine : MonoBehaviour
    {
        private struct FractureInstance
        {
            public AsyncFractureResult Result;
            public IEnumerator Enumerator;

            public FractureInstance(AsyncFractureResult result, IEnumerator enumerator)
            {
                Result = result;
                Enumerator = enumerator;
            }
        }

        private static FractureEngine _instance;

        private bool _suspended;

        private List<FractureInstance> _runningFractures = new List<FractureInstance>(); 

        private static FractureEngine Instance
        {
            get
            {
                if (_instance == null)
                {
                    GameObject inst = new GameObject("Fracture Engine");
                    _instance = inst.AddComponent<FractureEngine>();
                }

                return _instance;
            }
        }

        /// <summary>
        /// True if all further fracture operations should be a no-op.
        /// </summary>
        public static bool Suspended
        {
            get { return Instance._suspended; }
            set { Instance._suspended = value; }
        }

        // Returns true if there are fractures currently in progress
        public static bool HasFracturesInProgress
        {
            get { return Instance._runningFractures.Count > 0; }
        }

        private void OnDestroy()
        {
            if (_instance == this)
            {
                _instance = null;
            }
        }

        /// <summary>
        /// Starts a fracture operation
        /// </summary>
        /// <param name="details">Fracture info</param>
        /// <param name="callback">The object to fracture</param>
        /// <param name="piecesParent">The parent of the resulting fractured pieces root object</param>
        /// <param name="transferMass">True to distribute the original object's mass to the fracture pieces; false otherwise</param>
        /// <param name="hideAfterFracture">True to hide the originating object after fracturing</param>
        /// <returns></returns>
        public static AsyncFractureResult StartFracture(FractureDetails details, FractureGeometry callback, Transform piecesParent, bool transferMass, bool hideAfterFracture)
        {
            AsyncFractureResult res = new AsyncFractureResult();
            if (Suspended)
            {
                res.SetResult(null, new Bounds());
            }
            else
            {
                if (details.Asynchronous)
                {
                    IEnumerator it = Instance.WaitForResults(FractureBuilder.Fracture(details), callback, piecesParent, transferMass, hideAfterFracture, res);
                    if (it.MoveNext())
                    {
#if UNITY_EDITOR
                        if (Instance._runningFractures.Count == 0 && !Application.isPlaying)
                        {
                            EditorApplication.update += Instance.OnEditorUpdate;
                        }
#endif

                        Instance._runningFractures.Add(new FractureInstance(res, it));
                    }
                }
                else
                {
                    // There should only be one iteration
                    IEnumerator it = Instance.WaitForResults(FractureBuilder.Fracture(details), callback, piecesParent, transferMass, hideAfterFracture, res);
                    while (it.MoveNext())
                    {
                        Debug.LogWarning("DinoFracture: Sync fracture taking more than one iteration");
                    }
                }
            }
            return res;
        }

        private void OnEditorUpdate()
        {
            UpdateFractures();

            if (_runningFractures.Count == 0)
            {
#if UNITY_EDITOR
                EditorApplication.update -= OnEditorUpdate;
#endif
                DestroyImmediate(gameObject);
            }
        }

        private void Update()
        {
            UpdateFractures();
        }

        private void UpdateFractures()
        {
            for (int i = _runningFractures.Count - 1; i >= 0; i--)
            {
                if (_runningFractures[i].Result.StopRequested)
                {
                    _runningFractures.RemoveAt(i);
                }
                else
                {
                    if (!_runningFractures[i].Enumerator.MoveNext())
                    {
                        _runningFractures.RemoveAt(i);
                    }
                }
            }
        }

        private IEnumerator WaitForResults(AsyncFractureOperation operation, FractureGeometry callback, Transform piecesParent, bool transferMass, bool hideAfterFracture, AsyncFractureResult result)
        {
            while (!operation.IsComplete)
            {
                // Async fractures should not happen while in edit mode because game objects don't update too often
                // and the coroutine is not pumped. Sync fractures should not reach this point.
                System.Diagnostics.Debug.Assert(Application.isPlaying && operation.Details.Asynchronous);
                yield return null;
            }

            Rigidbody origBody = null;
            if (transferMass)
            {
                origBody = callback.GetComponent<Rigidbody>();
            }
            float density = 0.0f;
            if (origBody != null)
            {
                // Calculate the density by setting the density to
                // a known value and see what the mass comes out to.
                float mass = origBody.mass;
                origBody.SetDensity(1.0f);
                float volume = origBody.mass;
                density = mass / volume;

                // Reset the mass
                origBody.mass = mass;
            }

            List<FracturedMesh> meshes = operation.Result.GetMeshes();

            GameObject rootGO = new GameObject(callback.gameObject.name + " - Fracture Root");
            rootGO.transform.parent = (piecesParent ?? callback.transform.parent);
            rootGO.transform.position = callback.transform.position;
            rootGO.transform.rotation = callback.transform.rotation;
            rootGO.transform.localScale = Vector3.one;  // Scale is controlled by the value in operation.Details

            Material[] sharedMaterials = callback.GetComponent<Renderer>().sharedMaterials;

            for (int i = 0; i < meshes.Count; i++)
            {
                GameObject go = (GameObject)Instantiate(callback.FractureTemplate);
                go.name = "Fracture Object " + i;
                go.transform.parent = rootGO.transform;
                go.transform.localPosition = meshes[i].Offset;
                go.transform.localRotation = Quaternion.identity;
                go.transform.localScale = Vector3.one;
                go.SetActive(true);

                MeshFilter mf = go.GetComponent<MeshFilter>();
                mf.mesh = meshes[i].Mesh;

                MeshRenderer meshRenderer = go.GetComponent<MeshRenderer>();
                if (meshRenderer != null)
                {
                    Material[] materials = new Material[sharedMaterials.Length - meshes[i].EmptyTriangleCount + 1];
                    int matIdx = 0;
                    for (int m = 0; m < sharedMaterials.Length; m++)
                    {
                        if (!meshes[i].EmptyTriangles[m])
                        {
                            materials[matIdx++] = sharedMaterials[m];
                        }
                    }
                    if (!meshes[i].EmptyTriangles[sharedMaterials.Length])
                    {
                        materials[matIdx] = callback.InsideMaterial;
                    }

                    meshRenderer.sharedMaterials = materials;
                }

                MeshCollider meshCol = go.GetComponent<MeshCollider>();
                if (meshCol != null)
                {
                    meshCol.sharedMesh = mf.sharedMesh;
                }

                if (transferMass && origBody != null)
                {
                    Rigidbody rb = go.GetComponent<Rigidbody>();
                    if (rb != null)
                    {
                        rb.SetDensity(density);
                        rb.mass = rb.mass;  // Need to explicity set it for the editor to reflect the changes
                    }
                }

                FractureGeometry fg = go.GetComponent<FractureGeometry>();
                if (fg != null)
                {
                    fg.InsideMaterial = callback.InsideMaterial;
                    fg.FractureTemplate = callback.FractureTemplate;
                    fg.PiecesParent = callback.PiecesParent;
                    fg.NumGenerations = callback.NumGenerations - 1;
                    fg.DistributeMass = callback.DistributeMass;
                }
            }

            OnFractureEventArgs args = new OnFractureEventArgs(callback, rootGO);
            if (Application.isPlaying)
            {
                callback.gameObject.SendMessage("OnFracture", args, SendMessageOptions.DontRequireReceiver);
            }
            else
            {
                callback.OnFracture(args);
            }

            if (hideAfterFracture)
            {
                callback.gameObject.SetActive(false);
            }

            if (Application.isPlaying)
            {
                Transform trans = rootGO.transform;
                for (int i = 0; i < trans.childCount; i++)
                {
                    trans.GetChild(i).gameObject.SendMessage("OnFracture", args, SendMessageOptions.DontRequireReceiver);
                }
            }

            result.SetResult(rootGO, operation.Result.EntireMeshBounds);
        }
    }
}