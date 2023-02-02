using System;
using System.Threading;
using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace DinoFracture
{
    /// <summary>
    /// Apply this component to any game object you wish to pre-fracture.
    /// Pre-fracturing is a way of baking fracture pieces into the scene.
    /// Each time the object is fractured, the same set of pieces will
    /// activate.  This is very useful when creating a large number of
    /// pieces or high poly meshes, which would be too slow to create at
    /// runtime.  The pieces will be in the scene as a disabled root object
    /// with piece children.  When the object is fractured, those pieces
    /// will activate.
    /// </summary>
    public class PreFracturedGeometry : FractureGeometry
    {
        /// <summary>
        /// A reference to the root of the pre-fractured pieces.
        /// This is not normally set manually.  Instead, you press
        /// the “Create Fractures” button in the inspector window
        /// to generate the fracture immediately.  
        /// </summary>
        /// <remarks>The “Create Fractures” button is only intended to be used in edit mode; not game mode.</remarks>
        public GameObject GeneratedPieces;

        /// <summary>
        /// The encapsulating bounds of the entire set of pieces.  In local space.
        /// </summary>
        public Bounds EntireMeshBounds;

        private Action<PreFracturedGeometry> _completionCallback;
        private AsyncFractureResult _runningFracture;
        private bool _ignoreOnFractured;

        public AsyncFractureResult RunningFracture
        {
            get { return _runningFracture; }
        }

        private void Start()
        {
            Prime();
        }

        /// <summary>
        /// Primes the pre-fractured pieces when the game starts by
        /// activating them and then deactivating them.  This avoids
        /// a large delay on fracture if there are a lot of rigid bodies.
        /// </summary>
        public void Prime()
        {
            if (GeneratedPieces != null)
            {
                bool activeSelf = gameObject.activeSelf;
                gameObject.SetActive(false);

                GeneratedPieces.SetActive(true);
                GeneratedPieces.SetActive(false);

                gameObject.SetActive(activeSelf);
            }
        }

        public void GenerateFractureMeshes(Action<PreFracturedGeometry> completedCallback)
        {
            GenerateFractureMeshes(Vector3.zero, completedCallback);
        }

        public void GenerateFractureMeshes(Vector3 localPoint, Action<PreFracturedGeometry> completedCallback)
        {
            if (Application.isPlaying)
            {
                Debug.LogWarning("DinoFracture: Creating pre-fractured pieces at runtime.  This can be slow if there a lot of pieces.");
            }

            if (GeneratedPieces != null)
            {
                if (Application.isPlaying)
                {
                    Destroy(GeneratedPieces);
                }
                else
                {
                    DestroyImmediate(GeneratedPieces);
                }
            }

            FractureDetails details = new FractureDetails();
            details.NumPieces = NumFracturePieces;
            details.NumIterations = NumIterations;
            details.UVScale = FractureUVScale.Piece;
            details.Asynchronous = !Application.isPlaying;  // Async in editor to prevent hangs, sync while playing
            details.FractureCenter = localPoint;
            details.FractureRadius = FractureRadius;

            _runningFracture = Fracture(details, false);
            _completionCallback = completedCallback;
            if (Application.isPlaying)
            {
                if (!_runningFracture.IsComplete)
                {
                    // Should never get here.
                    Debug.LogError("DinoFracture: Prefracture task is not complete");
                }

                OnPreFractureComplete();
            }
            else
            {
#if UNITY_EDITOR
                EditorApplication.update += EditorUpdate;
#endif
            }
        }

        public void StopRunningFracture()
        {
#if UNITY_EDITOR
            EditorApplication.update -= EditorUpdate;
#endif
            _runningFracture.StopFracture();
            _runningFracture = null;
            StopFracture();
        }

        private void EditorUpdate()
        {
            if (_runningFracture == null)
            {
#if UNITY_EDITOR
                EditorApplication.update -= EditorUpdate;
#endif
            }
            else if (_runningFracture.IsComplete)
            {
#if UNITY_EDITOR
                EditorApplication.update -= EditorUpdate;
#endif
                OnPreFractureComplete();
            }
        }

        private void OnPreFractureComplete()
        {
            GeneratedPieces = _runningFracture.PiecesRoot;
            EntireMeshBounds = _runningFracture.EntireMeshBounds;
            GeneratedPieces.SetActive(false);
            _runningFracture = null;

            if (_completionCallback != null)
            {
                _completionCallback(this);
            }
        }

        protected override AsyncFractureResult FractureInternal(Vector3 localPos)
        {
            if (gameObject.activeSelf)
            {
                if (GeneratedPieces == null)
                {
                    GenerateFractureMeshes(localPos, null);

                    EnableFracturePieces();
                }
                else
                {
                    EnableFracturePieces();

                    OnFractureEventArgs args = new OnFractureEventArgs(this, GeneratedPieces);

                    // Notify scripts on this object
                    _ignoreOnFractured = true;
                    gameObject.SendMessage("OnFracture", args,
                        SendMessageOptions.DontRequireReceiver);
                    _ignoreOnFractured = false;

                    // Notify each fracture piece
                    Transform trans = GeneratedPieces.transform;
                    for (int i = 0; i < trans.childCount; i++)
                    {
                        trans.GetChild(i).gameObject.SendMessage("OnFracture", args, SendMessageOptions.DontRequireReceiver);
                    }
                }

                gameObject.SetActive(false);

                AsyncFractureResult result = new AsyncFractureResult();
                result.SetResult(GeneratedPieces, EntireMeshBounds);
                return result;
            }
            else
            {
                AsyncFractureResult result = new AsyncFractureResult();
                result.SetResult(null, new Bounds());
                return result;
            }
        }

        private void EnableFracturePieces()
        {
            Transform gpt = GeneratedPieces.transform;
            Transform t = transform;

            gpt.position = t.position;
            gpt.rotation = t.rotation;
            gpt.localScale = Vector3.one; // Scale has already been applied

            GeneratedPieces.SetActive(true);
        }

        internal override void OnFracture(OnFractureEventArgs args)
        {
            if (!_ignoreOnFractured)
            {
                base.OnFracture(args);

                GeneratedPieces = args.FracturePiecesRootObject;
                GeneratedPieces.SetActive(false);
            }
        }
    }
}