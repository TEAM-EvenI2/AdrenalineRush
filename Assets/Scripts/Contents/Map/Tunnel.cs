using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tunnel : MonoBehaviour
{
	public Transform interiorPrefab; // Temp

	[Header("Tunnel Shape")]

	public float tunnelRadius;
	public int tunnelSegmentCount;

	public float ringDistance;

	public float minCurveRadius, maxCurveRadius;
	public int minCurveSegmentCount, maxCurveSegmentCount;

	private float curveRadius;
	public float CurveRadius
	{
		get
		{
			return curveRadius;
		}
	}

	private int curveSegmentCount;
	public int CurveSegmentCount
	{
		get
		{
			return curveSegmentCount;
		}
	}

	private float curveAngle; 
	public float CurveAngle
	{
		get
		{
			return curveAngle;
		}
	}

	private float relativeRotation;
	public float RelativeRotation
	{
		get
		{
			return relativeRotation;
		}
	}

	private Mesh mesh;
	private Vector3[] vertices;
	private int[] triangles;

	[Header("Tunnel noise")]
	public float noiseStrength;

	[Header("Tunnel Item Generator")]
	public ItemGenerator[] generators;
	public ItemGenerator[] itemGenerator;


	private void Awake()
	{
		GetComponent<MeshFilter>().mesh = mesh = new Mesh();
		mesh.name = "Pipe";
	}


	public void Generate(bool withItems = true)
	{
		curveRadius = Random.Range(minCurveRadius, maxCurveRadius);
		curveSegmentCount =
			Random.Range(minCurveSegmentCount, maxCurveSegmentCount + 1);
		mesh.Clear();
		SetVertices();
		SetTriangles();
		mesh.normals = CaculateNormals();
		for (int i = 0; i < transform.childCount; i++)
		{
			Destroy(transform.GetChild(i).gameObject);
		}
		if (withItems)
		{
		//	if (generators != null && generators.Length > 0)
		//	{
		//		generators[Random.Range(0, generators.Length)].GenerateItems(this);
		//	}

		//	if(generators != null && generators.Length > 0)
		//	{
		//		itemGenerator[Random.Range(0, itemGenerator.Length)].GenerateItems(this);
		//	}
		}
		InteriorPlace();
	}

	private void InteriorPlace()
    {
		// Temp Code
		float angleStep = curveAngle / curveSegmentCount;
		for (int i = 0; i < curveSegmentCount; i++)
		{
			Transform item = Instantiate(interiorPrefab);
			float pipeRotation =
				(Random.Range(0, tunnelSegmentCount) + 0.5f) *
				360f / tunnelSegmentCount;

			item.transform.SetParent(transform, false);
			item.transform.localRotation = Quaternion.Euler(0f, 0f, -i * angleStep);
			item.transform.GetChild(0).localPosition = new Vector3(0f, curveRadius);
			item.transform.GetChild(0).localRotation = Quaternion.Euler(pipeRotation, 0f, 0f);
			item.transform.GetChild(0).GetChild(0).localPosition = new Vector3(0f, Random.Range(0, tunnelRadius));
			item.transform.GetChild(0).GetChild(0).localEulerAngles = new Vector3(Random.Range(0, 180), Random.Range(0, 180), Random.Range(0, 180));
		}
	}

    private void SetVertices()
    {
        vertices = new Vector3[tunnelSegmentCount * curveSegmentCount * 4];
		float uStep = ringDistance / curveRadius;
		curveAngle = uStep * curveSegmentCount * (360f / (2f * Mathf.PI));
        int iDelta = tunnelSegmentCount * 4;

        for (int u = 1, i = 0; u <= curveSegmentCount; u++, i += iDelta)
        {
            CreateQuadRing(u, i);
        }

        mesh.vertices = vertices;
    }

    private void CreateQuadRing(float u, int i)
    {
        float vStep = (2f * Mathf.PI) / tunnelSegmentCount;
		float uStep = ringDistance / curveRadius;
		int ringOffset = tunnelSegmentCount * 4;

        Vector3 vertex = GetPointOnTorus( u * uStep, 0f, GetTunnelRadius(u == curveSegmentCount ? uStep : u * uStep, 0));
		Vector3 vertexB = GetPointOnTorus(0, 0f, GetTunnelRadius(0, 0));
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
				vertices[i] = vertexB;
				vertices[i + 1] = vertexB = GetPointOnTorus(0, v * vStep, GetTunnelRadius(0, v== tunnelSegmentCount? vStep :  v * vStep));
			}

			// u
            vertices[i + 2] = vertex;
            vertices[i + 3] = vertex = GetPointOnTorus(u * uStep, v * vStep, GetTunnelRadius(u == curveSegmentCount ? uStep : u * uStep, v == tunnelSegmentCount ? vStep : v * vStep));

        }
    }

	private float GetTunnelRadius(float u, float v)
    {


		return tunnelRadius + Mathf.PerlinNoise(u, v) * noiseStrength;
    }

    private void SetTriangles()
	{
		triangles = new int[tunnelSegmentCount * curveSegmentCount * 6];
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
		for(int i = 0; i < triangleCount; i++)
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

		for(int i = 0; i < vertexNormals.Length; i++)
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

	public Vector3 GetPointOnTorus(float u, float v, float cylinderRadius)
	{
		Vector3 p;
		float r = (curveRadius + cylinderRadius * Mathf.Cos(v));
		p.x = r * Mathf.Sin(u);
		p.y = r * Mathf.Cos(u);
		p.z = cylinderRadius * Mathf.Sin(v);
		return p;
	}

	public void AlignWith(Tunnel tunnel)
	{
		relativeRotation =
		Random.Range(0, curveSegmentCount) * 360f / tunnelSegmentCount;

		transform.SetParent(tunnel.transform, false);
		transform.localPosition = Vector3.zero;
		transform.localRotation = Quaternion.Euler(0f, 0f, -tunnel.curveAngle);
		transform.Translate(0f, tunnel.curveRadius, 0f);
		transform.Rotate(relativeRotation, 0f, 0f);
		transform.Translate(0f, -curveRadius, 0f);
		transform.SetParent(tunnel.transform.parent);
		transform.localScale = Vector3.one;
	}
}
