using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TorusMesh : MapMesh
{

    public float ringDistance;

    [Header("Tunnel noise")]
    public float noiseStrength;

    public override void Generate(MeshWrapper meshWrapper)
    {
        base.Generate(meshWrapper);
        meshWrapper.mesh.Clear();
        SetVertices(meshWrapper);
        SetTriangles(meshWrapper);
        meshWrapper.mesh.normals = CaculateNormals();
    }
    protected override void SetVertices(MeshWrapper meshWrapper)
    {
        vertices = new Vector3[roadSegmentCount * meshWrapper.curveSegmentCount * 4];
        float uStep = ringDistance / meshWrapper.curveRadius;
        meshWrapper.curveAngle = uStep * meshWrapper.curveSegmentCount * (360f / (2f * Mathf.PI));
        int iDelta = roadSegmentCount * 4;

        for (int u = 1, i = 0; u <= meshWrapper.curveSegmentCount; u++, i += iDelta)
        {
            CreateQuadRing(meshWrapper, u, i);
        }

        meshWrapper.mesh.vertices = vertices;
    }

    private void CreateQuadRing(MeshWrapper meshWrapper, float u, int i)
    {
        float vStep = (2f * Mathf.PI) / roadSegmentCount;
        float uStep = ringDistance / meshWrapper.curveRadius;
        int ringOffset = roadSegmentCount * 4;

        Vector3 vertex = GetPointOnSurface(meshWrapper, u * uStep, 0f, GetTunnelRadius(u == meshWrapper.curveSegmentCount ? uStep : u * uStep, 0));
        Vector3 vertexB = GetPointOnSurface(meshWrapper, 0, 0f, GetTunnelRadius(0, 0));
        for (int v = 1; v <= roadSegmentCount; v++, i += 4)
        {

            // u - 1
            if (i >= ringOffset)
            {
                vertices[i] = vertices[i - ringOffset + 2];
                vertices[i + 1] = vertices[i - ringOffset + 3];
            }
            else
            {
                vertices[i] = vertexB;
                vertices[i + 1] = vertexB = GetPointOnSurface(meshWrapper, 0, v * vStep, GetTunnelRadius(0, v == roadSegmentCount ? vStep : v * vStep));
            }

            // u
            vertices[i + 2] = vertex;
            vertices[i + 3] = vertex = GetPointOnSurface(meshWrapper, u * uStep, v * vStep, GetTunnelRadius(u == meshWrapper.curveSegmentCount ? uStep : u * uStep, v == roadSegmentCount ? vStep : v * vStep));

        }
    }

    protected override void SetUV(MeshWrapper meshWrapper)
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

    protected override void SetTriangles(MeshWrapper meshWrapper)
    {
        triangles = new int[roadSegmentCount * meshWrapper.curveSegmentCount * 6];
        for (int t = 0, i = 0; t < triangles.Length; t += 6, i += 4)
        {
            triangles[t] = triangles[t + 5] = i;
            triangles[t + 1] = i + 2;
            triangles[t + 2] = triangles[t + 3] = i + 3;
            triangles[t + 4] = i + 1;
        }
        meshWrapper.mesh.triangles = triangles;
    }

    private float GetTunnelRadius(float u, float v)
    {


        return mapSize + Mathf.PerlinNoise(u, v) * noiseStrength;
    }


    /// <summary>
    /// 
    /// </summary>
    /// <param name="u">curve angle (radian) </param>
    /// <param name="v">tunnel angle (radian) </param>
    /// <param name="radius"> tunnel radius </param>
    /// <returns></returns>
    public override Vector3 GetPointOnSurface(MeshWrapper meshWrapper, float u, float v, float radius)
    {
        Vector3 p;
        float r = (meshWrapper.curveRadius + radius * Mathf.Cos(v));
        p.x = r * Mathf.Sin(u);
        p.y = r * Mathf.Cos(u);
        p.z = radius * Mathf.Sin(v);
        return p;
    }

}
