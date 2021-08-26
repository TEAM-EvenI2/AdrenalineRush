using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

[System.Serializable]
public abstract class MapMesh 
{
	protected Vector3[] vertices;
	protected Vector2[] uv;
	protected int[] triangles;

	public int roadSegmentCount;
	public float minCurveRadius, maxCurveRadius;
	public int minCurveSegmentCount, maxCurveSegmentCount;


	public float mapSize; // tunnelRadius

	public virtual void Generate(MeshWrapper meshWrapper)
    {
		meshWrapper.curveRadius = Random.Range(minCurveRadius, maxCurveRadius);
		meshWrapper.curveSegmentCount =
			Random.Range(minCurveSegmentCount, maxCurveSegmentCount + 1);
	}

	protected abstract void SetVertices(MeshWrapper meshWrapper);

	protected abstract void SetUV(MeshWrapper meshWrapper);
	protected abstract void SetTriangles(MeshWrapper meshWrapper);

	public abstract Vector3 GetPointOnSurface(MeshWrapper meshWrapper, float i, float j, float k);

	protected Vector3[] CaculateNormals()
	{
		Vector3[] vertexNormals = new Vector3[vertices.Length];
		int triangleCount = triangles.Length / 3;
		for (int i = 0; i < triangleCount; i++)
		{
			int normalTriangleIndex = i * 3;
			int vertextIndexA = triangles[normalTriangleIndex];
			int vertextIndexB = triangles[normalTriangleIndex + 1];
			int vertextIndexC = triangles[normalTriangleIndex + 2];

			Vector3 triangleNormal = SurfaceNormalFromIndices(vertextIndexA, vertextIndexB, vertextIndexC);

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

	protected Vector3 SurfaceNormalFromIndices(int indexA, int indexB, int indexC)
	{
		Vector3 pointA = vertices[indexA];
		Vector3 pointB = vertices[indexB];
		Vector3 pointC = vertices[indexC];

		Vector3 sideAB = pointB - pointA;
		Vector3 sideAC = pointC - pointA;
		return Vector3.Cross(sideAB, sideAC).normalized;
	}

}
