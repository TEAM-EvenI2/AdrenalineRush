using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SurfacePartialObstacle : MeshObstacle
{

	public Mesh mesh;

	public int curveSegmentCount = 5;
	public int radiusSegmentCount = 5;
	private int sideSegmentCount = 1;

	public float curveLength;
	public float sizePercent = 1f;
	public float anglePercent = 1f;

	public float noiseStrength = 1f;
	public float sideNoiseStrength = 0f;

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

		mesh.Clear();
		SetVertices(mw, percent, angle);
		//SetUV();
		mesh.normals = CaculateNormals(mesh.vertices, mesh.triangles);
	}

	private void SetVertices(MapMeshWrapper mw, float percent, float angle)
	{
		Vector3[] vertices = new Vector3[(radiusSegmentCount + sideSegmentCount * 2) * (curveSegmentCount + 1) + (radiusSegmentCount - 2) * 2];
		int[] triangles = new int[((radiusSegmentCount + sideSegmentCount * 2) * (curveSegmentCount + 1) + (radiusSegmentCount - 2) * 2) * 6];


		float curArc = mw.curveRadius * mw.curveAngle * Mathf.Deg2Rad;
		float enableArc = curArc - (curveLength);
		float maxAngle = 360 * anglePercent;

		int triIndex = 0;
		int vertexIndex = 0;

		// Main
		Noise noise = new Noise();
		for (int i = 0; i < curveSegmentCount + 1; i++)
		{
			float u = (float)i / (curveSegmentCount);
			for (int _j = -sideSegmentCount; _j < radiusSegmentCount + sideSegmentCount; _j++)
			{
				int j = _j;
				if (_j < 0)
					j = 0;
				else if (_j >= radiusSegmentCount)
					j = radiusSegmentCount - 1;

				float v = (float)j / radiusSegmentCount;

				float x = Mathf.Sin((float)_j / (radiusSegmentCount + sideSegmentCount * 2) * 360 * Mathf.Deg2Rad) * Mathf.Cos(u * 360 * Mathf.Deg2Rad);
				float y = Mathf.Sin((float)_j / (radiusSegmentCount + sideSegmentCount * 2) * 360 * Mathf.Deg2Rad) * Mathf.Sin(u * 360 * Mathf.Deg2Rad);
				float z = Mathf.Cos((float)_j / (radiusSegmentCount + sideSegmentCount * 2) * 360 * Mathf.Deg2Rad);

				float noiseValue = (noise.Evaluate(new Vector3(x, y, z)) + 1) * 0.5f * noiseStrength;

				float _sizePercent = (1 - sizePercent* ( 1 + noiseValue));
				if (_j < 0)
				{
					_sizePercent = _sizePercent + (1 - _sizePercent) * (Mathf.Abs(_j) / sideSegmentCount);
				}
				else if (_j >= radiusSegmentCount)
				{
					_sizePercent = _sizePercent + (1 - _sizePercent) * ((_j  - radiusSegmentCount + 1) / sideSegmentCount);
				}

				float arc = u * curveLength + enableArc * percent;
				float _angle = (v + noiseValue * sideNoiseStrength * (Mathf.Sign(_j)) * (_sizePercent == 1? 1 : 0)) * maxAngle + angle;
				
				vertices[vertexIndex] = mw.mapMesh.GetPointOnSurface(mw, (arc) / mw.curveRadius, _angle * Mathf.Deg2Rad, mw.mapMesh.GetDistance(mw, arc / curArc, _angle / 360) * _sizePercent);
				if (i < curveSegmentCount)
				{
					if (_j < radiusSegmentCount + sideSegmentCount - 1)
					{
						AddTriangle(triangles, triIndex, vertexIndex, vertexIndex + radiusSegmentCount + sideSegmentCount * 2,  vertexIndex + radiusSegmentCount + sideSegmentCount * 2 + 1);
						triIndex += 3;
                        AddTriangle(triangles, triIndex, vertexIndex, vertexIndex + radiusSegmentCount + sideSegmentCount * 2 + 1, vertexIndex + 1);
                        triIndex += 3;
                    }
                }
			

				vertexIndex++;
			}
		}


		// Front side
		for(int u = 0; u < 1; u++)
		{
			for (int j = 0; j < radiusSegmentCount - 1; j++)
            {
				if (j == 0)
				{
                    AddTriangle(triangles, triIndex, j, j + 1, j + 2);
                    triIndex += 3;
					AddTriangle(triangles, triIndex, j, j + 2, vertexIndex);
					triIndex += 3;
				}
				else
				{
					float v = (float)j / radiusSegmentCount;

					float x = Mathf.Sin(v * 360 * Mathf.Deg2Rad) * Mathf.Cos(u * 360 * Mathf.Deg2Rad);
					float y = Mathf.Sin(v * 360 * Mathf.Deg2Rad) * Mathf.Sin(u * 360 * Mathf.Deg2Rad);
					float z = Mathf.Cos(v * 360 * Mathf.Deg2Rad);

					float noiseValue = (noise.Evaluate(new Vector3(x, y, z)) + 1) * 0.5f * noiseStrength;

					float arc = ((u - noiseValue * sideNoiseStrength) * curveLength) + enableArc * percent ;
					float _angle = v * maxAngle + angle;
					vertices[vertexIndex] = mw.mapMesh.GetPointOnSurface(mw, (arc) / mw.curveRadius, _angle * Mathf.Deg2Rad, mw.mapMesh.GetDistance(mw, arc / curArc, _angle / 360));

					if(j < radiusSegmentCount - 2)
                    {
                        AddTriangle(triangles, triIndex, j + 1,j +2, vertexIndex);
                        triIndex += 3;
                        AddTriangle(triangles, triIndex, vertexIndex, j + 2, vertexIndex + 1);
                        triIndex += 3;
                    }
                    else
					{
						AddTriangle(triangles, triIndex, j + 1, j + 2, vertexIndex);
						triIndex += 3;
						AddTriangle(triangles, triIndex, j + 2, j + 3, vertexIndex);
						triIndex += 3;
					}
					vertexIndex++;
				}
            }
		}

        mesh.vertices = vertices;
		mesh.triangles = triangles;

	}
	
	//private void SetVertices(MapMeshWrapper mw, float percent, float angle)
	//{
	//	Vector3[] vertices = new Vector3[(radiusSegmentCount) * (curveSegmentCount + 2 + 1)];
	//	int[] triangles = new int[(radiusSegmentCount) * (curveSegmentCount + 2 + 1) * 6];


	//	float curArc = mw.curveRadius * mw.curveAngle * Mathf.Deg2Rad;
	//	float enableArc = curArc - (curveLength);
	//	float maxAngle = 360 * anglePercent;

	//	int triIndex = 0;
	//	int vertexIndex = 0;

 //       for (int j = 0; j < radiusSegmentCount; j++)
 //       {
 //           float v = (float)j / (radiusSegmentCount);


 //           float arc = enableArc * percent;
 //           float _angle = v * maxAngle + angle;
 //           vertices[vertexIndex] = mw.mapMesh.GetPointOnSurface(mw, (arc) / mw.curveRadius, _angle * Mathf.Deg2Rad, mw.mapMesh.GetDistance(mw, arc / curArc, _angle / maxAngle));
 //           if (j != radiusSegmentCount - 1)
 //           {
 //               AddTriangle(triangles, triIndex, vertexIndex, vertexIndex + radiusSegmentCount, vertexIndex + radiusSegmentCount + 1);
 //               triIndex += 3;
 //               AddTriangle(triangles, triIndex, vertexIndex, vertexIndex + radiusSegmentCount + 1, vertexIndex + 1);
 //               triIndex += 3;
 //           }
 //           else
 //           {
 //               //AddTriangle(triangles, triIndex, vertexIndex, vertexIndex + radiusSegmentCount, radiusSegmentCount);
 //               //triIndex += 3;
 //               //AddTriangle(triangles, triIndex, vertexIndex, radiusSegmentCount, 0);
 //               //triIndex += 3;
 //           }
 //           vertexIndex++;
 //       }

	//	// Main
	//	for (int i = 0; i < curveSegmentCount + 1; i++)
	//	{
	//		float u = (float)i / (curveSegmentCount);
	//		for (int j = 0; j < radiusSegmentCount; j++)
	//		{
	//			float v = (float)j / radiusSegmentCount;

	//			float arc = u * curveLength + enableArc * percent;
	//			float _angle = v * maxAngle + angle;

	//			vertices[vertexIndex] = mw.mapMesh.GetPointOnSurface(mw, (arc) / mw.curveRadius, _angle * Mathf.Deg2Rad, mw.mapMesh.GetDistance(mw, arc / curArc, _angle / maxAngle) * (1 - sizePercent));
	//			if (j < radiusSegmentCount - 1)
	//			{
	//				AddTriangle(triangles, triIndex, vertexIndex, vertexIndex + radiusSegmentCount, vertexIndex + radiusSegmentCount + 1);
	//				triIndex += 3;
	//				AddTriangle(triangles, triIndex, vertexIndex, vertexIndex + radiusSegmentCount + 1, vertexIndex + 1);
	//				triIndex += 3;
	//			}
	//			else
	//			{
	//				//AddTriangle(triangles, triIndex, vertexIndex, vertexIndex + radiusSegmentCount, (i + 1) * radiusSegmentCount + radiusSegmentCount);
	//				//triIndex += 3;
	//				//AddTriangle(triangles, triIndex, vertexIndex, (i + 1) * radiusSegmentCount + radiusSegmentCount, (i + 1) * radiusSegmentCount);
	//				//triIndex += 3;
	//			}

	//			vertexIndex++;

	//		}
	//	}
 //       for (int j = 0; j < radiusSegmentCount; j++)
 //       {
 //           float v = (float)j / (radiusSegmentCount);

 //           float arc = curveLength + enableArc * percent;
 //           float _angle = v * maxAngle + angle;
 //           vertices[vertexIndex] = mw.mapMesh.GetPointOnSurface(mw, (arc) / mw.curveRadius, _angle * Mathf.Deg2Rad, mw.mapMesh.GetDistance(mw, arc / curArc, _angle / maxAngle));
 //           vertexIndex++;
 //       }

 //       mesh.vertices = vertices;
	//	mesh.triangles = triangles;

	//}
}
