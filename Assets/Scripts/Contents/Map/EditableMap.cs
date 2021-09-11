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
		public bool toggle = true;

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
		public float curveRadius;

		public float percent;

		public float angle
		{
			get
			{
				if (ti == null)
					return 0;

				if (ti is MeshObstacle)
                {
					return ((MeshObstacle)ti).angle;
                }

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
				if (ti is MeshObstacle)
				{
					return ((MeshObstacle)ti).angle;
				}
				return ti.transform.GetChild(0).localEulerAngles.x;
            }
        }

		public ObjectEditInfo(MapItem ti, float curveAngle, float curveRadius, float percent)
		{
			this.ti = ti;
			this.curveAngle = curveAngle;
			this.curveRadius = curveRadius;
			this.percent = percent;
		}
	}

	public MapMeshType meshType;
	public MapMeshWrapper meshWrapper;
	//[HideInInspector]
	public TorusMesh torusMesh = new TorusMesh();

	public List<PrefabObjectEditInfo> prefabObjectEditInfos = new List<PrefabObjectEditInfo>();


    private void Awake()
    {
		gameObject.SetActive(false);
    }

    public void AddItemToInfos(List<MapItemGenerateInfo> infos)
    {
		
		List<KeyValuePair<MapItem, ObjectEditInfo >> l = new List<KeyValuePair<MapItem, ObjectEditInfo>>();

		for (int i = 0; i < prefabObjectEditInfos.Count; i++)
		{
			for (int j = 0; j < prefabObjectEditInfos[i].spawnedObjectInfos.Count; j++)
			{
				l.Add(new KeyValuePair<MapItem, ObjectEditInfo>(prefabObjectEditInfos[i].itemPrefab, prefabObjectEditInfos[i].spawnedObjectInfos[j] ));
			}
		}

		l.Sort(delegate (KeyValuePair<MapItem, ObjectEditInfo> x1, KeyValuePair<MapItem, ObjectEditInfo> x2)
		{
			if (x1.Value.percent < x2.Value.percent)
				return -1;
			else if (x1.Value.percent > x2.Value.percent)
				return 1;
			return 0;
		});

		float previewPos = 0;
		for (int i = 0; i < l.Count; i++)
		{
			float curArc = l[i].Value.curveRadius * l[i].Value.percent * l[i].Value.curveAngle * Mathf.Deg2Rad;
			MapItemGenerateInfo migi = new MapItemGenerateInfo()
			{
				prefab = l[i].Key,
				percent = l[i].Value.percent,
				curveArc = curArc - previewPos,
				angle = l[i].Value.angle
			};
			if(l[i].Key is LongObstacle)
            {
				LongObstacle lo = (LongObstacle)l[i].Value.ti;
				migi.size = lo.size;
				migi.angleInTunnel = lo.angleInTunnel;
				migi.middleSizePercent = lo.middleSizePercent;
				migi.curve = lo.curve;
				migi.noise = lo.noiseStrength;
            }
			else if(l[i].Key is SurfaceObstacle)
			{
				SurfaceObstacle so = (SurfaceObstacle)l[i].Value.ti;
				migi.sizePercent = so.sizePercent;
				migi.roadWidth = so.roadWidth;
				migi.curveLength = so.curveLength;
				migi.curve = so.curve;
				migi.noise = so.noiseStrength;
				migi.sideNoise = so.sideNoiseStrength;
			}
			else if (l[i].Key is SurfacePartialObstacle)
			{
				SurfacePartialObstacle so = (SurfacePartialObstacle)l[i].Value.ti;
				migi.sizePercent = so.sizePercent;
				migi.anglePercent = so.anglePercent;
				migi.curveLength = so.curveLength;
				migi.noise = so.noiseStrength;
				migi.sideNoise = so.sideNoiseStrength;
			}
			infos.Add(migi);

			previewPos = curArc;
		}

		float _curArc = l[l.Count - 1].Value.curveRadius * l[l.Count - 1].Value.curveAngle * Mathf.Deg2Rad;
		if (_curArc > 0)
		{
			infos.Add(new MapItemGenerateInfo()
			{
				percent = 1,
				curveArc = _curArc - previewPos,
				angle = l[l.Count - 1].Value.curveAngle
			});
		}

	}

    #region Mesh Generate
    public void Generate()
	{
		Vector3 oldPos = transform.localPosition;
		Vector3 oldAngle = transform.localEulerAngles;
		transform.localEulerAngles = Vector3.zero;
		transform.localPosition = Vector3.zero;

		meshWrapper.Init(GetMesh(), meshType);
		meshWrapper.GenerateMesh( false);

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

	public void Refresh()
	{
		for (int i = prefabObjectEditInfos.Count - 1; i >= 0; i--)
		{
			PrefabObjectEditInfo spoei = prefabObjectEditInfos[i];
			for (int j = spoei.spawnedObjectInfos.Count - 1; j >= 0; j--)
			{
				ObjectEditInfo oei = spoei.spawnedObjectInfos[j];

				MapItem preMi = oei.ti;

				MapItem mi = Instantiate(spoei.itemPrefab);
				mi.transform.SetParent(transform);
				mi.transform.localPosition = Vector3.zero;

				float angle = oei.angle;
				oei.ti = mi;

				DuplicateSetting(mi, preMi);

				UpdateObject(new Vector2Int(i, j),
					oei.percent,
					angle);

				DestroyImmediate(preMi);
			}
		}
	}

	public void DuplicateSetting(MapItem to, MapItem from)
	{
		if (to is LongObstacle)
		{
			if (from as LongObstacle != null)
			{
				LongObstacle lo = (LongObstacle)to;
				lo.size = ((LongObstacle)from).size;
				lo.angleInTunnel = ((LongObstacle)from).angleInTunnel;
				lo.curve = new AnimationCurve(((LongObstacle)from).curve.keys);
				lo.noiseStrength = ((LongObstacle)from).noiseStrength;
				lo.middleSizePercent = ((LongObstacle)from).middleSizePercent;
			}
		}
		else if (to is SurfaceObstacle)
		{
			if (from as SurfaceObstacle != null)
			{
				SurfaceObstacle so = (SurfaceObstacle)to;
				so.sizePercent = ((SurfaceObstacle)from).sizePercent;
				so.roadWidth = ((SurfaceObstacle)from).roadWidth;
				so.curve = new AnimationCurve(((SurfaceObstacle)from).curve.keys);
				so.noiseStrength = ((SurfaceObstacle)from).noiseStrength;
				so.curveLength = ((SurfaceObstacle)from).curveLength;
				so.sideNoiseStrength = ((SurfaceObstacle)from).sideNoiseStrength;
			}
		}
		else if (to is SurfacePartialObstacle)
		{
			if (from as SurfacePartialObstacle != null)
			{
				SurfacePartialObstacle so = (SurfacePartialObstacle)to;
				so.sizePercent = ((SurfacePartialObstacle)from).sizePercent;
				so.anglePercent = ((SurfacePartialObstacle)from).anglePercent;
				so.noiseStrength = ((SurfacePartialObstacle)from).noiseStrength;
				so.curveLength = ((SurfacePartialObstacle)from).curveLength;
				so.sideNoiseStrength = ((SurfacePartialObstacle)from).sideNoiseStrength;
			}
		}
	}

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

		for(int i = transform.childCount - 1; i >= 0 ; i--)
		{
			bool find = false;
			for (int j = prefabObjectEditInfos.Count - 1; j >= 0; j--)
			{
				for (int k = prefabObjectEditInfos[j].spawnedObjectInfos.Count - 1; k >= 0; k--)
				{
					if (prefabObjectEditInfos[j].spawnedObjectInfos[k].ti == transform.GetChild(i).GetComponent<MapItem>())
					{
						find = true;
						break;
					}
				}
				if (find)
					break;
			}

            if (!find)
            {
				DestroyImmediate(transform.GetChild(i).gameObject);
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
		ti.transform.SetParent(transform);
		ti.transform.localPosition = Vector3.zero;


		for (int j = 0; j < spoei.spawnedObjectInfos.Count; j++)
		{
			if (percent < spoei.spawnedObjectInfos[j].percent)
			{
				spoei.spawnedObjectInfos.Insert(j, new ObjectEditInfo(ti, meshWrapper.curveAngle, meshWrapper.curveRadius, percent));
				UpdateObject(new Vector2Int(i, j),
					percent,
					spoei.spawnedObjectInfos[j].angle);
				return new Vector2Int(i, j);
			}
		}

		spoei.spawnedObjectInfos.Add(new ObjectEditInfo(ti, meshWrapper.curveAngle, meshWrapper.curveRadius, percent));
		UpdateObject(new Vector2Int(i, spoei.spawnedObjectInfos.Count - 1), 
			percent,
			spoei.spawnedObjectInfos[spoei.spawnedObjectInfos.Count - 1].angle);
		return new Vector2Int(i, spoei.spawnedObjectInfos.Count - 1);
	}

	public void UpdateObject(Vector2Int index, float percent, float angle)
	{
		ObjectEditInfo oei = prefabObjectEditInfos[index.x].spawnedObjectInfos[index.y];
		oei.percent = percent;

		angle = angle % 360;
		if (angle < 0)
			angle += 360;

		oei.ti.Setting(meshWrapper, percent, angle / 360, GetMesh().GetDistance(meshWrapper, percent, angle / 360));

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


	#endregion
}
