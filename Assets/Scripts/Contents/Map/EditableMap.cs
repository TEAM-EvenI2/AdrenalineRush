using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class EditableMap : MonoBehaviour
{
	[System.Serializable]
	public class PrefabObjectEditInfo
    {
		public MapItem itemPrefab;
		public Color c;

		public List<ObjectEditInfo> spawnedObjectInfos = new List<ObjectEditInfo>();

		public PrefabObjectEditInfo(MapItem prefab)
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
		public MapItem ti;
		public float curveAngle;

		public float percent;

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

		public float pureAngle
        {
            get
            {
				return ti.transform.GetChild(0).localEulerAngles.x;
            }
        }

		public ObjectEditInfo(MapItem ti, float curveAngle, float percent)
		{
			this.ti = ti;
			this.curveAngle = curveAngle;
			this.percent = percent;
		}
	}

	public MapMeshType meshType;
	public MeshWrapper meshWrapper;
	//[HideInInspector]
	public TorusMesh torusMesh = new TorusMesh();

	public List<PrefabObjectEditInfo> prefabObjectEditInfos = new List<PrefabObjectEditInfo>();


    private void Awake()
    {
		gameObject.SetActive(false);
    }

	public void AddItemToInfos(Queue<MapItemGenerateInfo> infos)
    {
		for(int i = 0; i < prefabObjectEditInfos.Count; i++)
		{
			float previewPos = 0;
			prefabObjectEditInfos[i].spawnedObjectInfos.Sort(delegate (ObjectEditInfo x1, ObjectEditInfo x2)
			{
				if (x1.percent < x2.percent)
					return -1;
				else if (x1.percent > x2.percent)
					return 1;
				return 0;
			});

			for (int j = 0; j < prefabObjectEditInfos[i].spawnedObjectInfos.Count; j++)
            {
				ObjectEditInfo oei = prefabObjectEditInfos[i].spawnedObjectInfos[j];

				infos.Enqueue(new MapItemGenerateInfo()
				{
					prefab = prefabObjectEditInfos[i].itemPrefab,
					percent = oei.percent,
					positionDelta = oei.percent * oei.curveAngle - previewPos,
					angle = oei.angle
				}) ;

				previewPos = oei.percent * oei.curveAngle;

			}
        }
    }

    #region Mesh Generate
    public void Generate()
	{
		Vector3 oldPos = transform.localPosition;
		Vector3 oldAngle = transform.localEulerAngles;
		transform.localEulerAngles = Vector3.zero;
		transform.localPosition = Vector3.zero;

		meshWrapper.Generate(GetMesh(), meshType, false);

		transform.localEulerAngles = oldAngle;
		transform.localPosition = oldPos;
	}

	public MapMesh GetMesh()
	{
        switch (meshType)
        {
			case MapMeshType.Tunnel:
				return torusMesh;
        }

		return torusMesh;
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

	public Vector2Int AddObject(MapItem obj, float percent)
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

		MapItem ti = Instantiate(spoei.itemPrefab);
		float pos = percent * meshWrapper.curveAngle;

		ti.transform.SetParent(transform);
		ti.transform.localPosition = meshWrapper.mapMesh.GetPointOnSurface(meshWrapper, pos * Mathf.Deg2Rad, ti.transform.GetChild(0).localEulerAngles.x * Mathf.Deg2Rad, GetMesh().mapSize);



		for (int j = 0; j < spoei.spawnedObjectInfos.Count; j++)
		{
			if (percent < spoei.spawnedObjectInfos[j].percent)
			{
				spoei.spawnedObjectInfos.Insert(j, new ObjectEditInfo(ti, meshWrapper.curveAngle, percent));
				return new Vector2Int(i, j);
			}
		}

		spoei.spawnedObjectInfos.Add(new ObjectEditInfo(ti, meshWrapper.curveAngle, percent));
		return new Vector2Int(i, spoei.spawnedObjectInfos.Count - 1);
	}

	public void UpdateObject(Vector2Int index, float percent, float angle)
	{
		Transform t = prefabObjectEditInfos[index.x].spawnedObjectInfos[index.y].ti.transform;
		float pos = percent * meshWrapper.curveAngle;
		t.localPosition = GetMesh().GetPointOnSurface(meshWrapper, pos * Mathf.Deg2Rad, angle * Mathf.Deg2Rad, GetMesh().mapSize);
		t.localEulerAngles = new Vector3(0, 0, -meshWrapper.curveAngle * percent);
		t.GetChild(0).localEulerAngles = Vector3.right * angle;

		prefabObjectEditInfos[index.x].spawnedObjectInfos[index.y].percent = percent;
	}

	public void RemoveObject(ref Vector2Int index)
	{
		if (prefabObjectEditInfos.Count < 0)
			return;
		if (prefabObjectEditInfos[index.x].spawnedObjectInfos[index.y].ti != null)
			DestroyImmediate(prefabObjectEditInfos[index.x].spawnedObjectInfos[index.y].ti.gameObject);

		prefabObjectEditInfos[index.x].spawnedObjectInfos.RemoveAt(index.y);
		if (index.y >= prefabObjectEditInfos[index.x].spawnedObjectInfos.Count)
			index.y = prefabObjectEditInfos[index.x].spawnedObjectInfos.Count - 1;

		if(prefabObjectEditInfos[index.x].spawnedObjectInfos.Count <= 0)
        {
			index.y = 0;
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
