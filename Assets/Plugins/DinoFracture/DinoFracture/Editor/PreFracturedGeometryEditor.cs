using System;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace DinoFracture.Editor
{
    [CustomEditor(typeof(PreFracturedGeometry))]
    public class PreFracturedGeometryEditor : UnityEditor.Editor
    {
        private bool _waitForClick = false;

        private GUIStyle _centerStyle;
        private GUIStyle _buttonStyle;

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            PreFracturedGeometry geom = (PreFracturedGeometry)target;

            if (geom.RunningFracture == null)
            {
                if (GUILayout.Button("Create Fractures"))
                {
                    geom.GenerateFractureMeshes(SaveToDisk);
                }
            }
            else
            {
                Color color = GUI.backgroundColor;
                GUI.backgroundColor = Color.red;
                if (GUILayout.Button("Stop Fracture"))
                {
                    geom.StopRunningFracture();
                }
                GUI.backgroundColor = color;
            }

            if (_waitForClick)
            {
                if (_buttonStyle == null)
                {
                    _buttonStyle = new GUIStyle(GUI.skin.button);
                    _buttonStyle.normal.textColor = Color.white;
                }

                Color color = GUI.backgroundColor;
                GUI.backgroundColor = Color.red;
                if (GUILayout.Button("Click on the Object", _buttonStyle))
                {
                    _waitForClick = false;
                }
                GUI.backgroundColor = color;
            }
            else
            {
                if (geom.RunningFracture == null)
                {
                    if (GUILayout.Button("Create Fractures at Point"))
                    {
                        _waitForClick = true;
                    }
                }
            }

            if (Application.isPlaying)
            {
                if (GUILayout.Button("Crumble"))
                {
                    geom.Fracture();
                }
            }
        }

        private void OnSceneGUI()
        {
            if (_waitForClick)
            {
                Vector2 mousePos = Event.current.mousePosition;

                if (_centerStyle == null)
                {
                    _centerStyle = new GUIStyle(GUI.skin.label);
                    _centerStyle.alignment = TextAnchor.UpperCenter;
                    _centerStyle.normal.textColor = Color.white;
                    _centerStyle.active.textColor = Color.white;
                    _centerStyle.hover.textColor = Color.white;
                }

                Handles.BeginGUI();
                GUI.Label(new Rect(mousePos.x - 80.0f, mousePos.y - 45.0f, 160.0f, 17.0f),
                    "Click on the object to", _centerStyle);
                GUI.Label(new Rect(mousePos.x - 80.0f, mousePos.y - 28.0f, 160.0f, 17.0f),
                    "create the fracture pieces.", _centerStyle);
                Handles.EndGUI();

                if (Event.current.type == EventType.Layout)
                {
                    HandleUtility.AddDefaultControl(0);
                }

                PreFracturedGeometry geom = (PreFracturedGeometry)target;

                if (Event.current.type == EventType.MouseDown)
                {
                    Ray ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
                    _waitForClick = false;
                    Collider collider = geom.GetComponent<Collider>();
                    if (collider != null)
                    {
                        RaycastHit hit;
                        if (collider.Raycast(ray, out hit, 1000000000.0f))
                        {
                            Vector3 localPoint = geom.transform.worldToLocalMatrix.MultiplyPoint(hit.point);
                            geom.GenerateFractureMeshes(localPoint, SaveToDisk);
                        }
                    }
                }
                else if (Event.current.type == EventType.MouseMove)
                {
                    SceneView.RepaintAll();
                }
            }
        }

        private void SaveToDisk(PreFracturedGeometry geom)
        {
            int choice = EditorUtility.DisplayDialogComplex("Save meshes to disk?",
                "Would you like to save the meshes to disk?  This is necessary to be part of a prefab.",
                "Clear Folder and Save", "Save, no Clear", "Don't Save");

            if (choice < 2)
            {
                string folder = EditorUtility.SaveFolderPanel("Asset Folder", "Assets", geom.gameObject.name);
                if (!String.IsNullOrEmpty(folder) && folder.StartsWith(Application.dataPath))
                {
                    if (choice == 0)
                    {
                        // Delete the contents of the folder
                        DirectoryInfo dir = new DirectoryInfo(folder);
                        foreach (FileInfo file in dir.GetFiles())
                        {
                            string baseName = "";
                            if (file.Name.EndsWith(".asset"))
                            {
                                baseName = file.Name.Substring(0, file.Name.Length - ".asset".Length);
                            }
                            if (!String.IsNullOrEmpty(baseName))
                            {
                                try
                                {
                                    new Guid(baseName);

                                    // Guid resolves, delete the file since it is probably one we created.
                                    AssetDatabase.DeleteAsset("Assets" +
                                                              file.FullName.Substring(Application.dataPath.Length));
                                }
                                catch (Exception)
                                {
                                }
                            }
                        }
                    }

                    // Create a new asset for each mesh
                    folder = folder.Substring(Application.dataPath.Length);
                    for (int i = 0; i < geom.GeneratedPieces.transform.childCount; i++)
                    {
                        MeshFilter mf = geom.GeneratedPieces.transform.GetChild(i).GetComponent<MeshFilter>();
                        if (mf != null && mf.sharedMesh != null)
                        {
                            AssetDatabase.CreateAsset(mf.sharedMesh,
                                String.Format("Assets{0}/{1}.asset", folder, Guid.NewGuid().ToString("B")));
                        }
                    }

                    AssetDatabase.SaveAssets();
                    AssetDatabase.Refresh();
                }
            }
        }
    }
}
