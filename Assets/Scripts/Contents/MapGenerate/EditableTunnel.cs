using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EditableTunnel : MonoBehaviour
{

	public float tunnelRadius;
	public int tunnelSegmentCount = 20;
	public int tunnelLength;

	private Mesh mesh;
	private Vector3[] vertices;
	private int[] triangles;

	


	public void Generate()
	{
		if(mesh == null)
		{
			GetComponent<MeshFilter>().mesh = mesh = new Mesh();
			mesh.name = "Tunnel";
		}

		transform.localEulerAngles = Vector3.zero;
		transform.localPosition = Vector3.zero;
		mesh.Clear();
		SetVertices();
		SetTriangles();
		mesh.normals = CaculateNormals();
	}

	private void SetVertices()
	{
		vertices = new Vector3[tunnelSegmentCount * tunnelLength * 4];
		int iDelta = tunnelSegmentCount * 4;

		for (int u = 1, i = 0; u <= tunnelLength; u++, i += iDelta)
		{
			CreateQuadRing(u, i);
		}

		mesh.vertices = vertices;
	}

	private void CreateQuadRing(float u, int i)
	{
		float vStep = (2f * Mathf.PI) / tunnelSegmentCount;
		int ringOffset = tunnelSegmentCount * 4;

		Vector3 vertex = GetPointOnSurface(u, 0f, tunnelRadius);
		Vector3 vertexB = GetPointOnSurface(0, 0f, tunnelRadius);
		for (int v = 1; v <= tunnelSegmentCount; v++, i += 4)
		{

			// u - 1
			if (i >= ringOffset)
			{
				vertices[i] = vertices[i - ringOffset + 2];
				vertices[i + 1] = vertices[i - ringOffset + 3];
			}
			else
			{
				vertices[i ] = vertexB;
				vertices[i + 1] = vertexB = GetPointOnSurface(0, v * vStep, tunnelRadius);
			}

			// u
			vertices[i + 2] = vertex;
			vertices[i + 3] = vertex = GetPointOnSurface(u, v * vStep, tunnelRadius);

		}
	}

	public Vector3 GetPointOnSurface(float l, float v, float radius)
	{


		return new Vector3(l, radius * Mathf.Cos(v), radius * Mathf.Sin(v));

	}

	private void SetTriangles()
	{
		triangles = new int[tunnelSegmentCount * tunnelLength * 6];
		for (int t = 0, i = 0; t < triangles.Length; t += 6, i += 4)
		{
			triangles[t] = triangles[t + 5] = i;
			triangles[t + 1] = i + 2;
			triangles[t + 2] = triangles[t + 3] = i + 3;
			triangles[t + 4] = i + 1;
		}
		mesh.triangles = triangles;
	}

	private Vector3[] CaculateNormals()
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

	private Vector3 SurfaceNormalFromIndices(int indexA, int indexB, int indexC)
	{
		Vector3 pointA = vertices[indexA];
		Vector3 pointB = vertices[indexB];
		Vector3 pointC = vertices[indexC];

		Vector3 sideAB = pointB - pointA;
		Vector3 sideAC = pointC - pointA;
		return Vector3.Cross(sideAB, sideAC).normalized;
	}
}
