using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

[CreateAssetMenu(fileName ="Map_Mesh_Data", menuName ="Map Mesh/Map Mesh Data")]
public class MapMeshDataWrapper : ScriptableObject
{

    public MapMeshType meshType;
    [HideInInspector]
    public TorusMesh torus = new TorusMesh();

    [HideInInspector] public bool foldout;

    public MapMesh GetMesh()
    { 
        switch (meshType)
        {
            case MapMeshType.Tunnel:
                return torus;
        }

        return torus;
    }
}
