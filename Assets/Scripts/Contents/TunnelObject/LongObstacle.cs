using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LongObstacle : MeshObstacle
{

    public Mesh mesh;

    public float size = 1;
    public int radiusSegmentCount = 5;
    public int lengthSegmentCount = 1;
	public float angleInTunnel;


	private Vector3[] vertices;
	private Vector2[] uv;
	private int[] triangles;

	public override void Generate(MapMeshWrapper mw, float percent, float angle)
    {
		base.Generate(mw, percent, angle);

		this.angle = angle;
        if (mesh == null)
        {
            GetComponent<MeshFilter>().mesh = mesh = new Mesh();
			GetComponent<MeshCollider>().sharedMesh = mesh;
			mesh.name = "Long Obstacle";
		}
		mesh.Clear();
		SetVertices(mw, percent, angle);
		//SetUV();
        mesh.normals = CaculateNormals();
    }

    private void SetVertices(MapMeshWrapper mw, float percent, float angle)
	{
		vertices = new Vector3[radiusSegmentCount * (lengthSegmentCount + 1)];
		triangles = new int[(radiusSegmentCount + 1) * (lengthSegmentCount + 1) * 6];

		int triIndex = 0;

		float curArc = mw.curveAngle * Mathf.Deg2Rad * mw.curveRadius;
		float enableArc = curArc - (size + Mathf.Tan(angleInTunnel * Mathf.Deg2Rad) * mw.mapMesh.mapSize) * 2;

		for (int i = 0; i < lengthSegmentCount + 1; i++)
		{
			float lengthPercent = (float)i / lengthSegmentCount;
			for (int j = 0; j < radiusSegmentCount; j++)
			{

				int idx = i * radiusSegmentCount + j;
				float fixedPercent = i > lengthSegmentCount / 2 ? ((lengthPercent - 0.5f) * 2) : ((0.5f - lengthPercent) * 2);

				// angle + (wantedTunnelArc / tunnelRadius), wantedTunnelArc = cos(\theta) * size
				float _angle = angle + Mathf.Cos(((float)j / radiusSegmentCount) * 360 * Mathf.Deg2Rad) * size * Mathf.Rad2Deg;
				if (i > lengthSegmentCount / 2)
				{
					_angle += 180;
				}
				float arc = Mathf.Tan(angleInTunnel * Mathf.Deg2Rad) * mw.mapMesh.mapSize + size + enableArc * percent +
					(Mathf.Sin(((float)j / radiusSegmentCount) * 360 * Mathf.Deg2Rad) * size) * fixedPercent +
					Mathf.Tan(angleInTunnel * Mathf.Deg2Rad) * mw.mapMesh.mapSize * -(lengthPercent - 0.5f) * 2;

				_angle = _angle % 360;
				if (_angle < 0)
					_angle += 360;

				float distance = mw.mapMesh.GetDistance(mw, arc / curArc, _angle / 360) * fixedPercent;

				vertices[idx] = mw.mapMesh.GetPointOnSurface(mw, arc / mw.curveRadius, _angle * Mathf.Deg2Rad, distance);

				if (i != lengthSegmentCount)
				{
					if (j != radiusSegmentCount - 1)
					{
						if (i > lengthSegmentCount / 2 - 1)
						{
							AddTriangle(triIndex, idx, idx + radiusSegmentCount + 1, idx + radiusSegmentCount);
							triIndex += 3;
							AddTriangle(triIndex, idx, idx + 1, idx + radiusSegmentCount + 1);
							triIndex += 3;
						}
                        else
                        {
							AddTriangle(triIndex, idx, idx + radiusSegmentCount, idx + radiusSegmentCount + 1);
							triIndex += 3;
							AddTriangle(triIndex, idx, idx + radiusSegmentCount + 1, idx + 1);
							triIndex += 3;
						}
					}
					else
					{
						if (i  > lengthSegmentCount / 2 - 1)
						{
							AddTriangle(triIndex, idx, i * radiusSegmentCount + radiusSegmentCount, idx + radiusSegmentCount);
							triIndex += 3;
							AddTriangle(triIndex, idx, i * radiusSegmentCount, i * radiusSegmentCount + radiusSegmentCount);
							triIndex += 3;
						}
						else
						{
							AddTriangle(triIndex, idx, idx + radiusSegmentCount, i * radiusSegmentCount + radiusSegmentCount);
							triIndex += 3;
							AddTriangle(triIndex, idx, i * radiusSegmentCount + radiusSegmentCount, i * radiusSegmentCount);
							triIndex += 3;
						}
					}
				}
			}
		}

		mesh.vertices = vertices;
		mesh.triangles = triangles;
	}

	private void AddTriangle(int triIndex, int a, int b, int c)
    {
		triangles[triIndex] = a;
		triangles[triIndex + 1] = b;
		triangles[triIndex + 2] = c;
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
