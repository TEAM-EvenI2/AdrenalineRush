using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeformableObject : MonoBehaviour
{
    public void Transformation(MeshWrapper mw)
    {
        float tunnelRadius = mw.mapMesh.mapSize;


        transform.localScale = new Vector3(transform.localScale.x, tunnelRadius * 2, transform.localScale.z);
        transform.localPosition = Vector3.up * -tunnelRadius;
    }
}
