using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TorusMesh : MapMesh
{ 

    [Header("Tunnel noise")]
    public float noiseStrength;

    #region Type1

    public override void Generate(MapMeshWrapper meshWrapper)
    {
        base.Generate(meshWrapper);
        meshWrapper.mesh.Clear();
        SetVertices(meshWrapper);
        //SetUV(meshWrapper);
        SetTriangles(meshWrapper);
        meshWrapper.mesh.normals = CaculateNormals();
    }
    protected override void SetVertices(MapMeshWrapper meshWrapper)
    {
        vertices = new Vector3[roadSegmentCount * (meshWrapper.curveSegmentCount + 1)];
        int iDelta = roadSegmentCount;

        int i = 0;

        float vStep = (2f * Mathf.PI) / roadSegmentCount;
        float uStep = ringDistance / meshWrapper.curveRadius;
        for (int u = 0; u <= meshWrapper.curveSegmentCount; u++)
        {
            for (int v = 0; v < roadSegmentCount; v++, i++)
            {
                vertices[i] = GetPointOnSurface(meshWrapper, u * uStep, v * vStep, GetDistance(meshWrapper, (float)u / meshWrapper.curveSegmentCount, (float)v / roadSegmentCount));
            }
        }

        meshWrapper.mesh.vertices = vertices;
    }

    protected override void SetUV(MapMeshWrapper meshWrapper)
    {
        uv = new Vector2[vertices.Length];
        for (int i = 0; i < vertices.Length; i += 4)
        {
            uv[i] = Vector2.zero;
            uv[i + 1] = Vector2.right;
            uv[i + 2] = Vector2.up;
            uv[i + 3] = Vector2.one;
        }
        meshWrapper.mesh.uv = uv;
    }

    protected override void SetTriangles(MapMeshWrapper meshWrapper)
    {
        triangles = new int[(roadSegmentCount + 1) * (meshWrapper.curveSegmentCount  + 1)* 6];

        int triIndex = 0;

        for (int u = 0; u <= meshWrapper.curveSegmentCount; u++)
        {
            for (int v = 0; v < roadSegmentCount; v++)
            {
                int i = v + u * roadSegmentCount;
                if (u != meshWrapper.curveSegmentCount)
                {
                    if (v != roadSegmentCount - 1)
                    {
                        triangles[triIndex] = i;
                        triangles[triIndex + 2] = i + roadSegmentCount + 1;
                        triangles[triIndex + 1] = i + roadSegmentCount;

                        triangles[triIndex + 3] = i;
                        triangles[triIndex + 5] = i + 1;
                        triangles[triIndex + 4] = i + roadSegmentCount + 1;
                    }
                    else
                    {
                        triangles[triIndex] = i;
                        triangles[triIndex + 2] = u * roadSegmentCount + roadSegmentCount;
                        triangles[triIndex + 1] = i + roadSegmentCount;

                        triangles[triIndex + 3] = i;
                        triangles[triIndex + 5] = u * roadSegmentCount;
                        triangles[triIndex + 4] = u * roadSegmentCount + roadSegmentCount;
                    }
                    triIndex += 6;
                }
            }
        }

        meshWrapper.mesh.triangles = triangles;
    }

    public override float GetDistance(MapMeshWrapper meshWrapper, float u, float v)
    {
        u = u * Mathf.PI * 2;
        v = v * Mathf.PI * 2 + meshWrapper.cumulativeRelativeRotation * Mathf.Deg2Rad;

        float x = Mathf.Sin(v) * Mathf.Cos(u);
        float y = Mathf.Sin(v) * Mathf.Sin(u);
        float z = Mathf.Cos(v);

        return mapSize * (1 + GetNoiseValue(new Vector3(x, y, z)));
    }


    /// <summary>
    /// 
    /// </summary>
    /// <param name="u">curve angle (radian) </param>
    /// <param name="v">tunnel angle (radian) </param>
    /// <param name="radius"> tunnel radius </param>
    /// <returns></returns>
    public override Vector3 GetPointOnSurface(MapMeshWrapper meshWrapper, float u, float v, float radius)
    {
        Vector3 p;
        float r = (meshWrapper.curveRadius + radius * Mathf.Cos(v));
        p.x = r * Mathf.Sin(u);
        p.y = r * Mathf.Cos(u);
        p.z = radius * Mathf.Sin(v);
        return p;
    }

    private float GetNoiseValue(Vector3 p)
    {
        Noise noise = new Noise();
        float noiseValue = (noise.Evaluate(p) + 1) * 0.5f * noiseStrength;

        return noiseValue;
    }
    #endregion


}
