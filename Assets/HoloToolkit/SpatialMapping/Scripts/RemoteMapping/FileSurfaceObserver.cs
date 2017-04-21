// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using HoloToolkit.Unity;
using UnityEngine;
using UnityEngine.VR.WSA;
using UnityEngine.VR.WSA.Persistence;
using System.Collections;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace HoloToolkit.Unity.SpatialMapping
{
    public class FileSurfaceObserver : SpatialMappingSource
    {
        [Tooltip("The anchor name to use when saving and loading meshes.")]
        string anchorStoreName = "storedmesh-";
        [Tooltip("The file name to use when saving and loading meshes.")]
        public string MeshFileName = "roombackup";

        [Tooltip("Key to press in editor to load a spatial mapping mesh from a .room file.")]
        public KeyCode LoadFileKey = KeyCode.L;

        [Tooltip("Key to press in editor to save a spatial mapping mesh to file.")]
        public KeyCode SaveFileKey = KeyCode.S;


        WorldAnchorStore anchorStore;
        List<MeshFilter> roomMeshFilters;
        int meshCount = 0;

        void Start()
        {
            WorldAnchorStore.GetAsync(AnchorStoreReady);
        }

        void AnchorStoreReady(WorldAnchorStore store)
        {
            anchorStore = store;
        }




        /// <summary>
        /// Loads the SpatialMapping mesh from the specified file.
        /// </summary>
        /// <param name="fileName">The name, without path or extension, of the file to load.</param>
        public void Load(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
            {
                Debug.Log("No mesh file specified.");
                return;
            }

            Cleanup();

            try
            {
                IList<Mesh> storedMeshes = MeshSaver.Load(fileName);

                for(int iMesh = 0; iMesh < storedMeshes.Count; iMesh++)
                {
                    SurfaceObject obj = CreateSurfaceObject(
                        mesh: storedMeshes[iMesh],
                        objectName: "storedmesh-" + iMesh,
                        parentObject: transform,
                        meshID: iMesh,
                        castShadowsOverride: false

                        );
                    if (!anchorStore.Load(storedMeshes[iMesh].name, obj.Object))
                        Debug.Log("WorldAnchor load failed...");
                    AddSurfaceObject(obj);

                }
                for (int iMesh = 0; iMesh < storedMeshes.Count; iMesh++)
                {
                    transform.GetChild(iMesh).gameObject.layer = 8;
                }
            }
            catch
            {
                Debug.Log("Failed to load " + fileName);
            }
        }
        public void Save(string fileName)
        {
            // if the anchor store is not ready then we cannot save the room mesh
            if (anchorStore == null)
                return;

            // delete old relevant anchors
            string[] anchorIds = anchorStore.GetAllIds();
            for (int i = 0; i < anchorIds.Length; i++)
            {
                if (anchorIds[i].Contains(anchorStoreName))
                {
                    anchorStore.Delete(anchorIds[i]);
                }
            }

            Debug.Log("Old anchors deleted...");

            // get all mesh filters used for spatial mapping meshes
            roomMeshFilters = SpatialUnderstanding.Instance.UnderstandingCustomMesh.GetMeshFilters() as List<MeshFilter>;

            Debug.Log("Mesh filters fetched...");

            // create new list of room meshes for serialization
            List<Mesh> roomMeshes = new List<Mesh>();

            // cycle through all room mesh filters
            foreach (MeshFilter filter in roomMeshFilters)
            {
                // increase count of meshes in room
                meshCount++;

                // make mesh name = anchor name + mesh count
                string meshName = anchorStoreName + meshCount.ToString();
                filter.mesh.name = meshName;

                Debug.Log("Mesh " + filter.mesh.name + ": " + filter.transform.position + "\n--- rotation " + filter.transform.localRotation + "\n--- scale: " + filter.transform.localScale);
                // add mesh to room meshes for serialization
                roomMeshes.Add(filter.mesh);

                // save world anchor
                WorldAnchor attachingAnchor = filter.gameObject.GetComponent<WorldAnchor>();
                if (attachingAnchor == null)
                {
                    attachingAnchor = filter.gameObject.AddComponent<WorldAnchor>();
                    Debug.Log("" + filter.mesh.name + ": Using new anchor...");
                }
                else
                {
                    Debug.Log("" + filter.mesh.name + ": Deleting existing anchor...");
                    DestroyImmediate(attachingAnchor);
                    Debug.Log("" + filter.mesh.name + ": Creating new anchor...");
                    attachingAnchor = filter.gameObject.AddComponent<WorldAnchor>();
                }
                if (attachingAnchor.isLocated)
                {
                    if (!anchorStore.Save(meshName, attachingAnchor))
                        Debug.Log("" + meshName + ": Anchor save failed...");
                    else
                        Debug.Log("" + meshName + ": Anchor SAVED...");
                }
                else
                {
                    attachingAnchor.OnTrackingChanged += AttachingAnchor_OnTrackingChanged;
                }
            }

            // serialize and save meshes
            MeshSaver.Save(fileName, roomMeshes);
        }
        private void AttachingAnchor_OnTrackingChanged(WorldAnchor self, bool located)
        {
            if (located)
            {
                string meshName = self.gameObject.GetComponent<MeshFilter>().mesh.name;
                if (!anchorStore.Save(meshName, self))
                    Debug.Log("" + meshName + ": Anchor save failed...");
                else
                    Debug.Log("" + meshName + ": Anchor SAVED...");

                self.OnTrackingChanged -= AttachingAnchor_OnTrackingChanged;
            }
        }
        // Called every frame.
        private void Update()
        {
            // Keyboard commands for saving and loading a remotely generated mesh file.
#if UNITY_EDITOR || UNITY_STANDALONE
            // S - saves the active mesh
            if (Input.GetKeyUp(SaveFileKey))
            {
                Save(MeshFileName);
            }

            // L - loads the previously saved mesh into editor and sets it to be the spatial mapping source.
            if (Input.GetKeyUp(LoadFileKey))
            {
                SpatialMappingManager.Instance.SetSpatialMappingSource(this);
                Load(MeshFileName);
            }
#endif
        }
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(FileSurfaceObserver))]
    public class FileSurfaceObserverEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            // Quick way for the user to get access to the room file location.
            if (GUILayout.Button("Open File Location"))
            {
                System.Diagnostics.Process.Start(MeshSaver.MeshFolderName);
            }
        }
    }
#endif
}