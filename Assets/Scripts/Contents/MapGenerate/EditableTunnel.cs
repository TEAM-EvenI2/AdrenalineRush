using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EditableTunnel : MonoBehaviour
{
	[System.Serializable]
	public class PrefabObjectEditInfo
    {
		public TunnelItem itemPrefab;
		public Color c;

		public List<ObjectEditInfo> spawnedObjectInfos = new List<ObjectEditInfo>();

		public PrefabObjectEditInfo(TunnelItem prefab)
        {
			itemPrefab = prefab;
			c= new Color(Random.Range(0, 1f), Random.Range(0, 1f), Random.Range(0, 1f));
		}

		public ObjectEditInfo GetObject(int index)
        {

			return spawnedObjectInfos[index];
        }
	}

	[System.Serializable]
	public class ObjectEditInfo
	{
		public TunnelItem ti;
		[HideInInspector] public int tunnelLength;

		public float distanceFromCenter;

		public float percent
		{
			get
			{
				if (ti == null)
					return 0;
				return ti.transform.localPosition.x / tunnelLength;
			}
		}

		public float angle
		{
			get
			{
				if (ti == null)
					return 0;

				Vector3 a = ti.transform.GetChild(0).localEulerAngles;

				if (a.y > 90 && a.z > 90)
				{
					a.x = 180 - a.x;
				}
				else
				{
					if (a.x >= 270)
						a.x = -(360 - a.x);
				}

				return a.x;
			}
		}

		public ObjectEditInfo(TunnelItem ti, int tunnelLength, float distanceFromCenter)
		{
			this.ti = ti;
			this.tunnelLength = tunnelLength;
			this.distanceFromCenter = distanceFromCenter;
		}
	}

	public float tunnelRadius;
	public int tunnelSegmentCount = 20;
	public int tunnelLength;

	private Mesh mesh;
	private Vector3[] vertices;
	private int[] triangles;

	public List<PrefabObjectEditInfo> prefabObjectEditInfos = new List<PrefabObjectEditInfo>();


    private void Awake()
    {
		gameObject.SetActive(false);
    }


    #region Mesh Generate
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

	#endregion

	#region Edit Object

	public void ValidateCheck()
    {
		for(int i = prefabObjectEditInfos.Count - 1; i >=0; i--)
        {
			for(int j = prefabObjectEditInfos[i].spawnedObjectInfos.Count - 1; j >= 0; j--)
            {
				if(prefabObjectEditInfos[i].spawnedObjectInfos[j].ti == null)
                {
					prefabObjectEditInfos[i].spawnedObjectInfos.RemoveAt(j);
                }
            }
			if(prefabObjectEditInfos[i].spawnedObjectInfos.Count <= 0)
            {
				prefabObjectEditInfos.RemoveAt(i);
            }
        }
    }

	public Vector2Int AddObject(TunnelItem obj, float percent)
	{
		PrefabObjectEditInfo spoei;

		int i = 0;
		for (i = 0; i < prefabObjectEditInfos.Count; i++)
		{
			if (obj == prefabObjectEditInfos[i].itemPrefab)
            {
				break;
            }
		}

		if(i >= prefabObjectEditInfos.Count)
        {
			spoei = new PrefabObjectEditInfo(obj);
			prefabObjectEditInfos.Add(spoei);
        }
        else
        {
			spoei = prefabObjectEditInfos[i];
        }

		TunnelItem ti = Instantiate(spoei.itemPrefab);
		float posZ = percent * tunnelLength;

		ti.transform.SetParent(transform);
		ti.transform.localPosition = GetPointOnSurface(posZ, ti.transform.GetChild(0).localEulerAngles.x * Mathf.Deg2Rad, tunnelRadius);



		for (int j = 0; j < spoei.spawnedObjectInfos.Count; j++)
		{
			if (percent < spoei.spawnedObjectInfos[j].percent)
			{
				spoei.spawnedObjectInfos.Insert(j, new ObjectEditInfo(ti, tunnelLength, tunnelRadius));
				return new Vector2Int(i, j);
			}
		}

		spoei.spawnedObjectInfos.Add(new ObjectEditInfo(ti, tunnelLength, tunnelRadius));
		return new Vector2Int(i, spoei.spawnedObjectInfos.Count - 1);
	}

	public void UpdateObject(Vector2Int index, float percent, float angle, float distanceFormCenter)
	{
		Transform t = prefabObjectEditInfos[index.x].spawnedObjectInfos[index.y].ti.transform;
		t.localPosition = GetPointOnSurface(tunnelLength * percent, angle * Mathf.Deg2Rad, distanceFormCenter);
		t.GetChild(0).localEulerAngles = Vector3.right * angle;
		prefabObjectEditInfos[index.x].spawnedObjectInfos[index.y].distanceFromCenter = distanceFormCenter;
	}

	public void RemoveObject(ref Vector2Int index)
	{
		if (prefabObjectEditInfos.Count < 0)
			return;
		if (prefabObjectEditInfos[index.x].spawnedObjectInfos[index.y].ti != null)
			DestroyImmediate(prefabObjectEditInfos[index.x].spawnedObjectInfos[index.y].ti.gameObject);
		prefabObjectEditInfos[index.x].spawnedObjectInfos.RemoveAt(index.y);
		if (prefabObjectEditInfos[index.x].spawnedObjectInfos.Count <= index.y)
			index.y = prefabObjectEditInfos[index.x].spawnedObjectInfos.Count - 1;

		if(prefabObjectEditInfos[index.x].spawnedObjectInfos.Count <= 0)
        {
			prefabObjectEditInfos.RemoveAt(index.x);
			if (index.x >= prefabObjectEditInfos.Count)
				index.x = prefabObjectEditInfos.Count - 1;
        }
	}

	public Vector2Int GetInfoIndex(int index)
    {
		Vector2Int vIndex = Vector2Int.zero;
		for (int  i = 0; i < prefabObjectEditInfos.Count; i++)
        {
			if (index >= prefabObjectEditInfos[i].spawnedObjectInfos.Count)
			{
				vIndex.x++;
				index -= prefabObjectEditInfos[i].spawnedObjectInfos.Count;
			}
			else
				break;
		}
		vIndex.y = index;

		return vIndex;
    }


	#endregion
}
