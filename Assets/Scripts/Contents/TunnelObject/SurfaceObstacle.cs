
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SurfaceObstacle : MeshObstacle
{

	public Mesh mesh;

	public AnimationCurve[] curves;
	public int curveSegmentCount = 5;
	public int radiusSegmentCount = 5;


	public float loadWidth = 1;
	public float curveLength;

	public float sizePercent = 1f;


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
			mesh.name = "Surface Obstacle";
		}
		if(transform.childCount == 0)
        {
			for(int i = 0; i < curves.Length + 1; i++)
            {
				GameObject go = new GameObject();
				go.transform.SetParent(transform);
				go.transform.localPosition = go.transform.localEulerAngles = Vector3.zero;
				go.name = "Test Mesh " + i;
				go.AddComponent<MeshFilter>().mesh = new Mesh();
                go.AddComponent<MeshRenderer>().material = GetComponent<MeshRenderer>().sharedMaterial;
            }
        }

		mesh.Clear();
		SetVertices(mw, percent, angle);
        //SetUV();
        mesh.normals = CaculateNormals(mesh.vertices, mesh.triangles);
    }

	private void SetVertices(MapMeshWrapper mw, float percent, float angle)
	{

        AddDefaultSurface(mw, percent, angle);
        for (int i = 0; i < curves.Length; i++)
        {
			AddRoadSurface(mw, percent, angle, i);
        }
		//Substarction();
	}

	private void AddDefaultSurface(MapMeshWrapper mw, float percent, float angle)
	{

		Vector3[] vertices = new Vector3[(radiusSegmentCount ) * (curveSegmentCount + 2 + 1)];
		int[] triangles = new int[(radiusSegmentCount) * (curveSegmentCount + 2 + 1) * 6];


		float curArc = mw.curveRadius * mw.curveAngle * Mathf.Deg2Rad;
		float enableArc = curArc - (curveLength);

		int triIndex = 0;
		int vertexIndex = 0;
		//
		for (int j = 0; j < radiusSegmentCount; j++)
		{
			float v = (float)j / (radiusSegmentCount );

			float arc = enableArc * percent;
			float _angle = v * 360 + angle;
			vertices[vertexIndex] = mw.mapMesh.GetPointOnSurface(mw, (arc) / mw.curveRadius, _angle * Mathf.Deg2Rad, mw.mapMesh.GetDistance(mw, arc / curArc, _angle / 360));
			if (j != radiusSegmentCount - 1)
			{
				AddTriangle(triangles, triIndex, vertexIndex, vertexIndex + radiusSegmentCount, vertexIndex + radiusSegmentCount + 1);
				triIndex += 3;
				AddTriangle(triangles, triIndex, vertexIndex, vertexIndex + radiusSegmentCount + 1, vertexIndex + 1);
				triIndex += 3;
			}
			else
			{
				AddTriangle(triangles, triIndex, vertexIndex, vertexIndex + radiusSegmentCount, radiusSegmentCount);
				triIndex += 3;
				AddTriangle(triangles, triIndex, vertexIndex, radiusSegmentCount, 0);
				triIndex += 3;
			}
			vertexIndex++;
		}

		// Main
		for (int i = 0; i < curveSegmentCount + 1; i++)
		{
			float u = (float)i / (curveSegmentCount);
			for (int j = 0; j < radiusSegmentCount; j++)
			{
				float v = (float)j / radiusSegmentCount;

				float arc = u * curveLength + enableArc * percent;
				float _angle = v * 360 + angle;

				vertices[vertexIndex] = mw.mapMesh.GetPointOnSurface(mw, (arc) / mw.curveRadius, _angle * Mathf.Deg2Rad, mw.mapMesh.GetDistance(mw, arc / curArc, _angle / 360) * (1 - sizePercent));
				if (j < radiusSegmentCount - 1)
				{
					AddTriangle(triangles, triIndex, vertexIndex, vertexIndex + radiusSegmentCount, vertexIndex + radiusSegmentCount + 1);
					triIndex += 3;
					AddTriangle(triangles, triIndex, vertexIndex, vertexIndex + radiusSegmentCount + 1, vertexIndex + 1);
					triIndex += 3;
				}
				else
				{
                    AddTriangle(triangles, triIndex, vertexIndex, vertexIndex + radiusSegmentCount, (i + 1) * radiusSegmentCount + radiusSegmentCount);
                    triIndex += 3;
                    AddTriangle(triangles, triIndex, vertexIndex, (i + 1) * radiusSegmentCount + radiusSegmentCount, (i + 1) * radiusSegmentCount);
                    triIndex += 3;
				}
				vertexIndex++;

			}
		}
		for (int j = 0; j < radiusSegmentCount; j++)
		{
			float v = (float)j / (radiusSegmentCount );

			float arc = curveLength + enableArc * percent;
			float _angle = v * 360 + angle;
			vertices[vertexIndex] = mw.mapMesh.GetPointOnSurface(mw, (arc) / mw.curveRadius, _angle * Mathf.Deg2Rad, mw.mapMesh.GetDistance(mw, arc / curArc, _angle / 360));
			vertexIndex++;
		}

		Mesh mesh = transform.GetChild(0).GetComponent<MeshFilter>().sharedMesh;
		mesh.Clear();
		mesh.vertices = vertices;
		mesh.triangles = triangles;

		mesh.normals = CaculateNormals(vertices, triangles);
	}

	private void AddRoadSurface(MapMeshWrapper mw, float percent, float angle, int curveIndex)
	{

		Vector3[] vertices = new Vector3[2 * (curveSegmentCount + 1)];
		int[] triangles = new int[(curveSegmentCount) * 6];

		float curArc = mw.curveRadius * mw.curveAngle * Mathf.Deg2Rad;
		float enableArc = curArc - (curveLength);

		int verticesIndex = 0;
		int triIndex = 0;
		for (int i = 0; i < curveSegmentCount + 1; i++)
		{
			float u = (float)i / (curveSegmentCount);
			Vector2 point = new Vector2(u, curves[curveIndex].Evaluate(u));

			Vector2 forward = Vector2.zero;
			if (i < curveSegmentCount)
			{
				float _u = (float)(i + 1) / (curveSegmentCount);
				forward += new Vector2(_u, curves[curveIndex].Evaluate(_u)) - point;
			}

			if (i > 0)
			{
				float _u = (float)(i - 1) / (curveSegmentCount);
				forward += point - new Vector2(_u, curves[curveIndex].Evaluate(_u));
			}
			forward.Normalize();

			Vector2 left = new Vector2(-forward.y, forward.x);

			float arcLeft = point.x * curveLength + left.x * loadWidth / 2 + enableArc * percent;
			float angleLeft = point.y * 360 + left.y * (loadWidth / 2) / mw.mapMesh.mapSize * Mathf.Rad2Deg + angle;
			float arcRight = point.x * curveLength - left.x * loadWidth / 2 + enableArc * percent;
			float angleRight = point.y * 360 - left.y * (loadWidth / 2) / mw.mapMesh.mapSize * Mathf.Rad2Deg + angle;

			vertices[verticesIndex] = mw.mapMesh.GetPointOnSurface(mw,
				(arcLeft) / mw.curveRadius,
				(angleLeft) * Mathf.Deg2Rad,
				mw.mapMesh.GetDistance(mw, point.x + (left.x * loadWidth / 2) / curveLength, point.y + ((left.y * (loadWidth / 2) / mw.mapMesh.mapSize) * Mathf.Rad2Deg) / 360) * (1-sizePercent));
			vertices[verticesIndex + 1] = mw.mapMesh.GetPointOnSurface(mw,
				(arcRight) / mw.curveRadius,
				(angleRight) * Mathf.Deg2Rad,
				mw.mapMesh.GetDistance(mw, point.x + (-left.x * loadWidth / 2) / curveLength, point.y + ((-left.y * (loadWidth / 2) / mw.mapMesh.mapSize) * Mathf.Rad2Deg) / 360) * (1 - sizePercent));

			if (i < curveSegmentCount)
			{
				AddTriangle(triangles, triIndex, verticesIndex, verticesIndex + 1, verticesIndex + 2);
				triIndex += 3;
				AddTriangle(triangles, triIndex, verticesIndex + 1, verticesIndex + 3, verticesIndex + 2);
				triIndex += 3;
			}
			verticesIndex += 2;
		}

		Mesh mesh = transform.GetChild(curveIndex + 1).GetComponent<MeshFilter>().sharedMesh;
		mesh.Clear();
		mesh.vertices = vertices;
		mesh.triangles = triangles;

	}

}
