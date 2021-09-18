
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SurfaceObstacle : MeshObstacle
{

	public Mesh mesh;

	public AnimationCurve curve;
	public int curveSegmentCount = 5;
	public int radiusSegmentCount = 5;


	public float roadWidth = 1;
	public float curveLength;

	public float sizePercent = 1f;
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

	private float CalcAngle(float p, float angle)
	{
		float _angle = p * 360 + angle;
		if (_angle < 0)
			_angle += 360;

		return _angle;
	}

	private void SetVertices(MapMeshWrapper mw, float percent, float angle)
	{
		List<Vector3> vertices = new List<Vector3>();
		List<Vector2> uv = new List<Vector2>();
		List<int> triangles = new List<int>();
		// [width, splited_mesh_count, start-end]
		int[,,] verticesStartEndCount = new int[curveSegmentCount + 1, 2, 2];

		float width = curveLength;
		float height = 2 * Mathf.PI * mw.mapMesh.mapSize;

		Vector2 forwardVec = GetForwardVector();
		Vector2 left = new Vector2(-forwardVec.y / width, forwardVec.x / height) * roadWidth;

		System.Func<float, float> leftFunc = (float x) => { return (curve.Evaluate(x - left.x) + left.y); };
		System.Func<float, float> rightFunc = (float x) => { return (curve.Evaluate(x + left.x) - left.y); }; ;

		float curArc = mw.curveRadius * mw.curveAngle * Mathf.Deg2Rad;
		float enableArc = curArc - (curveLength);

		Noise noise = new Noise();
		// ---------------------------- Set Vertices ----------------------------
		for (int i = 0; i < curveSegmentCount + 1; i++)
		{
			float u = (float)i / (curveSegmentCount);
			float l_u = leftFunc(u) * ((float)(radiusSegmentCount - 1) / radiusSegmentCount);
			float r_u = rightFunc(u) * ((float)(radiusSegmentCount - 1) / radiusSegmentCount);

			bool enterRoad = false;
				verticesStartEndCount[i, 0, 0] = verticesStartEndCount[i, 0, 1] = -1;
				verticesStartEndCount[i, 1, 0] = verticesStartEndCount[i, 1, 1] = -1;

			for (int j = 0; j < radiusSegmentCount; j++)
			{
				float v = (float)j / radiusSegmentCount;


				float x = Mathf.Sin(v* 360 * Mathf.Deg2Rad) * Mathf.Cos(u * 360 * Mathf.Deg2Rad);
				float y = Mathf.Sin(v * 360 * Mathf.Deg2Rad) * Mathf.Sin(u * 360 * Mathf.Deg2Rad);
				float z = Mathf.Cos(v * 360 * Mathf.Deg2Rad);

				float noiseValue = (noise.Evaluate(new Vector3(x, y, z)) + 1f) *0.5f * noiseStrength;

				float _sizePercent = (1 - sizePercent * ( 1 + noiseValue)) ;

				float arc = u * curveLength + enableArc * percent;
				float _angle = v * 360 + angle;
				if (_angle < 0)
					_angle += 360;

				if (v > l_u )
				{
					// 위쪽에 있는 mesh에서 첫번째 vertices index
					verticesStartEndCount[i, 1, 1] = vertices.Count;
				}
				else if (v < r_u )
				{
					// 아래쪽에 있는 mesh에서 첫번째 vertex index
					if (verticesStartEndCount[i, 0, 0] == -1)
						verticesStartEndCount[i, 0, 0] = vertices.Count;
				}
				else
				{
					// 아래쪽이랑 위쪽 mesh에서 격자에 안 맞는 위치에 대한 할당
					// 격자에 맞지 않는 위치라서 따로 설정해줘야 함.
					if (!enterRoad)
					{
						//if (r_u >= 0)
						{
							_angle = (r_u * 360 + angle);/// 360;
							_angle = _angle % 360;
							if (_angle < 0)
								_angle += 360;
							verticesStartEndCount[i, 0, 1] = vertices.Count;
							vertices.Add(mw.mapMesh.GetPointOnSurface(mw, (arc) / mw.curveRadius, _angle * Mathf.Deg2Rad, mw.mapMesh.GetDistance(mw, arc / curArc, _angle / 360) * _sizePercent) );
							uv.Add(new Vector2(u, r_u));
						}
						//if (l_u <= 1)
						{
							_angle = (l_u * 360 + angle);/// 360;
							_angle = _angle % 360;
							if (_angle < 0)
								_angle += 360;
							verticesStartEndCount[i, 1, 0] = vertices.Count;
							vertices.Add(mw.mapMesh.GetPointOnSurface(mw, (arc) / mw.curveRadius, _angle * Mathf.Deg2Rad, mw.mapMesh.GetDistance(mw, arc / curArc, _angle / 360) * _sizePercent) );
							uv.Add(new Vector2(u, l_u));
						}
					}
					enterRoad = true;

					continue;
				}

				Vector3 point = mw.mapMesh.GetPointOnSurface(mw, (arc) / mw.curveRadius, _angle * Mathf.Deg2Rad, mw.mapMesh.GetDistance(mw, arc / curArc, _angle / 360) * _sizePercent);
				vertices.Add(point);
				uv.Add(new Vector2(u, v));
			}

			if(enterRoad == false)
            {
				Debug.LogError("Curve graph is wrong, can't find path");
            }
		}

		int mainVertexCount = vertices.Count;

		// Start, End wall
		for (int u = 0; u < 2; u++)
		{
			float l_u = leftFunc(u) * ((float)(radiusSegmentCount - 1) / radiusSegmentCount);
			float r_u = rightFunc(u) * ((float)(radiusSegmentCount - 1) / radiusSegmentCount);

			bool enterRoad = false;

			for (int j = 0; j < radiusSegmentCount; j++)
			{
				float v = (float)j / radiusSegmentCount;

				float x = Mathf.Sin(v * 360 * Mathf.Deg2Rad) * Mathf.Cos(u * 360 * Mathf.Deg2Rad);
				float y = Mathf.Sin(v * 360 * Mathf.Deg2Rad) * Mathf.Sin(u * 360 * Mathf.Deg2Rad);
				float z = Mathf.Cos(v * 360 * Mathf.Deg2Rad);

				float noiseValue = (noise.Evaluate(new Vector3(x, y, z)) + 1f) * 0.5f * noiseStrength;

				float arc = (u - noiseValue * sideNoiseStrength * (u == 0? 1 : -1)) * curveLength + enableArc * percent ;
				float _angle = v * 360 + angle;

				if (v >= r_u && v <= l_u)
				{
					// 아래쪽이랑 위쪽 mesh에서 격자에 안 맞는 위치에 대한 할당
					// 격자에 맞지 않는 위치라서 따로 설정해줘야 함.
					if (!enterRoad)
					{
						//if (r_u >= 0)
						{
							_angle = (r_u * 360 + angle) ;
							vertices.Add(mw.mapMesh.GetPointOnSurface(mw, (arc) / mw.curveRadius, _angle * Mathf.Deg2Rad, mw.mapMesh.GetDistance(mw, arc / curArc, _angle / 360)));
							uv.Add(new Vector2(u, r_u));
						}
						//if (l_u <= 1)
						{
							_angle = (l_u * 360 + angle) ;
							vertices.Add(mw.mapMesh.GetPointOnSurface(mw, (arc) / mw.curveRadius, _angle * Mathf.Deg2Rad, mw.mapMesh.GetDistance(mw, arc / curArc, _angle / 360)));
							uv.Add(new Vector2(u, l_u));
						}
					}
					enterRoad = true;

					continue;
				}

				Vector3 point = mw.mapMesh.GetPointOnSurface(mw, (arc) / mw.curveRadius, _angle * Mathf.Deg2Rad, mw.mapMesh.GetDistance(mw, arc / curArc, _angle / 360));
				vertices.Add(point);
				uv.Add(new Vector2(u, v));
			}
		}

		// 
		for (int i = 1; i < curveSegmentCount; i++)
		{
			float u = (float)i / (curveSegmentCount);
			float l_u = leftFunc(u) * ((float)(radiusSegmentCount - 1) / radiusSegmentCount);
			float r_u = rightFunc(u) * ((float)(radiusSegmentCount - 1) / radiusSegmentCount);

			float arc = u * curveLength + enableArc * percent;
			
			//if (r_u >= 0)
			{
				float _angle = (r_u * 360 + angle) ;
				vertices.Add(mw.mapMesh.GetPointOnSurface(mw, (arc) / mw.curveRadius, _angle * Mathf.Deg2Rad, mw.mapMesh.GetDistance(mw, arc/ curArc, _angle / 360)));
				uv.Add(new Vector2(u, r_u));
			}
			//if (l_u <= 1)
			{
				float _angle = (l_u * 360 + angle) ;
				vertices.Add(mw.mapMesh.GetPointOnSurface(mw, (arc) / mw.curveRadius, _angle* Mathf.Deg2Rad, mw.mapMesh.GetDistance(mw, arc / curArc, _angle / 360)));
				uv.Add(new Vector2(u, l_u));
			}
		}



		// ---------------------------- Set Triangles --------------------------------------------------
		for (int i = 0; i < curveSegmentCount; i++)
		{ 
			for(int k = 0; k < 2; k++)
			{
				int st = verticesStartEndCount[i, k, 0];
				int ed = verticesStartEndCount[i, k, 1];
				int nSt = verticesStartEndCount[i + 1, k, 0];
				int nEd = verticesStartEndCount[i + 1, k, 1];
                //print("[" + i + ", " + k + "]: " + st + ", " + ed + ", " + nSt + ", " + nEd);
                if (st != -1 && ed != -1 && nSt != -1 && nEd != -1)
				{
					int nextHeight = nEd - nSt + 1;
					int curHeight = ed - st + 1;
					int gap = curHeight - nextHeight;
					if (gap < 0)
						gap = 0;
					for (int idx = 0; idx < curHeight; idx++)
					{
						if ( idx >= gap)
						{
							if (nSt + (idx- gap) + 1 <= nEd)
							{
                                AddTriangle(triangles, st + idx, nSt + idx - gap, nSt + idx - gap + 1);
                                if (st + idx + 1 <= ed)
								{
                                    AddTriangle(triangles, st + idx, nSt + idx - gap + 1, st + idx + 1);
                                }
                            }

							if(idx >= curHeight - 1)
                            {
                                if (k == 1)
                                {
                                    int lSt = verticesStartEndCount[i, 0, 0];
                                    int nLSt = verticesStartEndCount[i + 1, 0, 0];


                                    //print("[" + i + "]: " + lSt + ", " + nLSt + ", " + (curHeight - nextHeight));
                                    if (lSt != -1 && nLSt != -1)
                                    {
                                        AddTriangle(triangles, st + idx, nSt + idx - (curHeight - nextHeight), nLSt);
                                        AddTriangle(triangles, st + idx, nLSt, lSt);

                                    }
                                }
                            }
						}
                        else
                        {
                            if (gap - idx == 1)
                            {
                                AddTriangle(triangles, st + idx, nSt + idx - gap + 1, st + idx + 1);
                            }
                        }
					}
				}
			}
		}

		int wallVertex = 0;
		// For start and end wall
		for (int u = 0; u <2; u++)
		{
			int i = u * curveSegmentCount;
			for (int k = 0; k < 2; k++)
			{
				int st = verticesStartEndCount[i, k, 0];
				int ed = verticesStartEndCount[i, k, 1];
				if (st != -1 && ed != -1 )
				{
					int curHeight = ed - st + 1;
					for (int idx = 0; idx < curHeight; idx++)
					{

						if(st + idx + 1 <= ed)
                        {
							AddTriangle(triangles,st + idx, mainVertexCount + wallVertex + idx + 1, mainVertexCount + wallVertex + idx, u == 1);
							AddTriangle(triangles,st + idx, st + idx + 1, mainVertexCount + wallVertex + idx + 1, u == 1);
						}
                        else
                        {
							if (k == 1)
							{
								int lSt = verticesStartEndCount[i, 0, 0];
								int lEd = verticesStartEndCount[i, 0, 1];

								if (lSt != -1)
								{
									AddTriangle(triangles, st + idx, mainVertexCount + wallVertex - (lEd - lSt + 1), mainVertexCount + wallVertex + idx, u == 1);
									AddTriangle(triangles, st + idx, lSt, mainVertexCount + wallVertex - (lEd - lSt + 1), u == 1);
								}
							}
                        }
					}
					wallVertex += curHeight;
				}
			}
		}
        //----

        // For middle wall----
        for (int k = 0; k < 2; k++)
        {
            int p = verticesStartEndCount[0, k, 1 - k];
            int np = verticesStartEndCount[1, k, 1 - k];
            if (p != -1 && np != -1)
            {
                AddTriangle(triangles, p, mainVertexCount + wallVertex + k, mainVertexCount + p, k == 1);
                AddTriangle(triangles, p, np, mainVertexCount + wallVertex + k, k == 1);

            }
        }
        for (int i = 1; i < curveSegmentCount - 1; i++)
        {
            for (int k = 0; k < 2; k++)
            {
                int p = verticesStartEndCount[i, k, 1 - k];
                int np = verticesStartEndCount[i + 1, k, 1 - k];
                if (p != -1 && np != -1)
                {
                    AddTriangle(triangles, p, mainVertexCount + wallVertex + 2 + k, mainVertexCount + wallVertex + k, k == 1);
                    AddTriangle(triangles, p, np, mainVertexCount + wallVertex + 2 + k, k == 1);

                }
            }
            wallVertex += 2;

        }

        int stCount = 0;
        for (int k = 0; k < 2; k++)
        {
            int st = verticesStartEndCount[0, k, 0];
            int ed = verticesStartEndCount[0, k, 1];
            if (st != -1 && ed != -1)
                stCount += ed - st + 1;
        }

        for (int k = 0; k < 2; k++)
        {
            int p = verticesStartEndCount[curveSegmentCount - 1, k, 1 - k];
            int np = verticesStartEndCount[curveSegmentCount, k, 1 - k];

            if (p != -1 && np != -1)
			{
                //print("[" + k + "]: " + p + ", " + np + ", " );
                int v = verticesStartEndCount[curveSegmentCount , 0, 1] - verticesStartEndCount[curveSegmentCount , 0, 0];
                AddTriangle(triangles, p, mainVertexCount + stCount + v +k , mainVertexCount + wallVertex + k, k == 1);
                AddTriangle(triangles, p, np, mainVertexCount + stCount + v +k, k == 1);
            }
        }

        //----

        mesh.vertices = vertices.ToArray();
		mesh.triangles = triangles.ToArray();
		mesh.uv = uv.ToArray();
	}

	private void AddTriangle(List<int> triangles, int a, int b, int c, bool flip = false)
    {

		triangles.Add(a);
		if (flip)
		{

			triangles.Add(c);
			triangles.Add(b);
		}
		else
		{
			triangles.Add(b);
			triangles.Add(c);
		}
	}

	private Vector2 GetForwardVector()
	{
		// Least-Square
		float a = 0, b;

		float mean_u = 0;
		float mean_f_u = 0;
		for (int i = 0; i < curveSegmentCount + 1; i++)
		{
			float u = (float)i / (curveSegmentCount);
			float f_u = curve.Evaluate(u);
			mean_u += u;
			mean_f_u += f_u;
		}
		mean_u /= curveSegmentCount + 1;
		mean_f_u /= curveSegmentCount + 1;

		float p = 0;
		for (int i = 0; i < curveSegmentCount + 1; i++)
		{
			float u = (float)i / (curveSegmentCount);
			float f_u = curve.Evaluate(u);
			a += (u - mean_u) * (f_u - mean_f_u);
			p += (u - mean_u) * (u - mean_u);
		}
		a = a / p;
		b = mean_f_u - mean_u * a;

		Vector2 pointA = new Vector2(0, b);
		Vector2 pointB = new Vector2(1, a + b);

		Vector2 vecAB = pointB - pointA;

		return vecAB.normalized;
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

}
