using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TorusMesh : MapMesh
{


    [Header("Tunnel noise")]
    public float noiseStrength;

    public override void Generate(MeshWrapper meshWrapper)
    {
        base.Generate(meshWrapper);
        meshWrapper.mesh.Clear();
        SetVertices(meshWrapper);
        SetUV(meshWrapper);
        SetTriangles(meshWrapper);
        meshWrapper.mesh.normals = CaculateNormals();
    }
    protected override void SetVertices(MeshWrapper meshWrapper)
    {
        vertices = new Vector3[roadSegmentCount * meshWrapper.curveSegmentCount * 4];
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

        Vector3 vertex = GetPointOnSurface(meshWrapper, u * uStep, 0f, GetDistance(meshWrapper, (float)u / meshWrapper.curveSegmentCount, 0));
        Vector3 vertexB = GetPointOnSurface(meshWrapper, 0, 0f, GetDistance(meshWrapper,0, 0));
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
                vertices[i + 1] = vertexB = GetPointOnSurface(meshWrapper, 0, v * vStep, GetDistance(meshWrapper,0, (float)v / roadSegmentCount));
            }

            // u
            vertices[i + 2] = vertex;
            vertices[i + 3] = vertex = GetPointOnSurface(meshWrapper, u * uStep, v * vStep, GetDistance(meshWrapper,( float)u / meshWrapper.curveSegmentCount, (float)v / roadSegmentCount));

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

    public override float GetDistance(MeshWrapper meshWrapper, float u, float v)
    {
        u = u * Mathf.PI * 2;
        v = v * Mathf.PI * 2 + meshWrapper.cumulativeRelativeRotation * Mathf.Deg2Rad;

        float x = Mathf.Sin(v)* Mathf.Cos(u);
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
    public override Vector3 GetPointOnSurface(MeshWrapper meshWrapper, float u, float v, float radius)
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


}
