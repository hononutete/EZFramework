using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;



    [CustomEditor(typeof(MeshFilter))]
    public class NormalsVisualizer : Editor
    {

        private Mesh mesh;

        void OnEnable()
        {
            MeshFilter mf = target as MeshFilter;
            if (mf != null)
            {
                mesh = mf.sharedMesh;
            }
        }

        void OnSceneGUI()
        {
            return;
            if (mesh == null)
            {
                return;
            }

            for (int i = 0; i < mesh.vertexCount; i++)
            {
                Handles.matrix = (target as MeshFilter).transform.localToWorldMatrix;
                Handles.color = Color.red;
                Handles.DrawLine(
                    mesh.vertices[i],
                    mesh.vertices[i] + mesh.normals[i]);

                //Handles.color = Color.red;
                //Handles.DrawLine(
                //    mesh.vertices[i],
                //    mesh.vertices[i] + new Vector3(mesh.tangents[i].x, mesh.tangents[i].y, mesh.tangents[i].z));
            }
            //Debug.LogError("tan : " + mesh.tangents.Length);
            //for (int i = 0; i < mesh.vertexCount; i++)
            //{
            //    Handles.matrix = (target as MeshFilter).transform.localToWorldMatrix;
            //    Handles.color = Color.red;
            //    Handles.DrawLine(
            //        mesh.vertices[i],
            //        mesh.vertices[i] + new Vector3(mesh.tangents[i].x, mesh.tangents[i].y, mesh.tangents[i].z));
            //}
        }
    }

