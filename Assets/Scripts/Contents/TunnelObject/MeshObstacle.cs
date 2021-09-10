using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshCollider))]
public class MeshObstacle : MapItem
{
    public float angle;
    public override void Setting(MapMeshWrapper mw, float curverPecent, float ringPercent, float distanceFromCenter)
    {
        Generate(mw, curverPecent, ringPercent * 360);
    }

    public virtual void Generate(MapMeshWrapper mw, float percent, float angle)
    {
        transform.SetParent(mw.transform, false);
        transform.localPosition = Vector3.zero;
    }

	protected Vector3[] CaculateNormals(Vector3[] vertices, int[] triangles)
	{
		Vector3[] vertexNormals = new Vector3[vertices.Length];
		int triangleCount = triangles.Length / 3;
		for (int i = 0; i < triangleCount; i++)
		{
			int normalTriangleIndex = i * 3;
			int vertextIndexA = triangles[normalTriangleIndex];
			int vertextIndexB = triangles[normalTriangleIndex + 1];
			int vertextIndexC = triangles[normalTriangleIndex + 2];

			Vector3 triangleNormal = SurfaceNormalFromIndices(vertices, vertextIndexA, vertextIndexB, vertextIndexC);

			vertexNormals[vertextIndexA] += triangleNormal;
			vertexNormals[vertextIndexB] += triangleNormal;
			vertexNormals[vertextIndexC] += triangleNormal;
		}

		for (int i = 0; i < vertexNormals.Length; i++)
		{
			vertexNormals[i].Normalize();
		}

		return vertexNormals;

	}

	protected Vector3 SurfaceNormalFromIndices(Vector3[] vertices, int indexA, int indexB, int indexC)
	{
		Vector3 pointA = vertices[indexA];
		Vector3 pointB = vertices[indexB];
		Vector3 pointC = vertices[indexC];

		Vector3 sideAB = pointB - pointA;
		Vector3 sideAC = pointC - pointA;
		return Vector3.Cross(sideAB, sideAC).normalized;
	}
	protected void AddTriangle(int[] triangles, int triIndex, int a, int b, int c)
	{
		triangles[triIndex] = a;
		triangles[triIndex + 1] = b;
		triangles[triIndex + 2] = c;
	}
}
