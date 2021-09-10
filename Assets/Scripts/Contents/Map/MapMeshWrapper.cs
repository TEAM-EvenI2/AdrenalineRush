using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class MapMeshWrapper : MonoBehaviour
{

    public Mesh mesh;

    public float relativeRotation;
    public float curveRadius;

    public int curveSegmentCount;

    public float curveAngle;

    public MapMeshType meshType { get; private set; }
    private MapMesh _mapMesh;
    public MapMesh mapMesh
    {
        get { return _mapMesh; }
        set
        {
            if(_mapMesh == null)
            {
                _mapMesh = value;
            }
        }
    }

    public float cumulativeRelativeRotation;

    public Vector3 position;
    public Vector3 angle;

    // About Item spawn
    private List<MapItemGenerateInfo> _infos;
    private int _infoIndex = 0;
    float finishedArc = 0;

    private void Awake()
    {
        GetComponent<MeshFilter>().mesh = mesh = new Mesh();
    }

    public void ResetGenerateItem()
    {
        if (_infos == null)
            _infos = new List<MapItemGenerateInfo>();
        else
            _infos.Clear();
        _infoIndex = 0;
        finishedArc = 0;
    }

    public void AddGenerateItem(MapItemGenerateInfo info)
    {
        _infos.Add(info);
    }

    public void SpawnItem()
    {
        StartCoroutine(CoSpawnItem());
    }

    private IEnumerator CoSpawnItem()
    {
        if (mapMesh != null)
        {
            while (_infos != null && _infos.Count > _infoIndex)
            {
                float curArc = curveRadius * curveAngle * Mathf.Deg2Rad;
                MapItemGenerateInfo info = _infos[_infoIndex];
                finishedArc += info.curveArc;

                if (info.prefab != null)
                {

                    MapItem item = Instantiate(info.prefab);
                    float angle = info.angle - cumulativeRelativeRotation;
                    angle = angle % 360;
                    if (angle < 0)
                        angle += 360;

                    float distance = mapMesh.GetDistance(this, info.percent, angle / 360);
                    if (item is ScoreItem)
                        distance = mapMesh.mapSize;
                    else if (item is LongObstacle)
                    {
                        LongObstacle lo = (LongObstacle)item;
                        lo.size = info.size;
                        lo.angleInTunnel = info.angleInTunnel;
                        lo.middleSizePercent = info.middleSizePercent;
                        lo.curve = new AnimationCurve(info.curve.keys);
                        lo.noiseStrength = info.noise;
                    }
                    else if (item is SurfaceObstacle)
                    {
                        SurfaceObstacle so = (SurfaceObstacle)item;
                        so.sizePercent = info.sizePercent;
                        so.roadWidth = info.roadWidth;
                        so.curveLength = info.curveLength;
                        so.curve = new AnimationCurve(info.curve.keys);
                        so.noiseStrength = info.noise;
                    }
                    else if (item is SurfacePartialObstacle)
                    {
                        SurfacePartialObstacle so = (SurfacePartialObstacle)item;
                        so.sizePercent = info.sizePercent;
                        so.anglePercent = info.anglePercent;
                        so.curveLength = info.curveLength;
                        so.noiseStrength = info.noise;
                        so.sideNoiseStrength = info.sideNoise;
                    }

                    item.Setting(this, info.percent, angle / 360, distance);
                }
                _infoIndex++;
                yield return null;
            }
        }
    }

    public void Init(MapMesh mapMesh, MapMeshType type)
    {
        if (mesh == null)
        {
            GetComponent<MeshFilter>().mesh = mesh = new Mesh();
        }
        mesh.name = type.ToString();
        meshType = type;
        _mapMesh = mapMesh;
        _mapMesh.Init(this);
    }

    public void GenerateMesh(bool destoryChild = true)
    {
        _mapMesh.Generate(this);

        if (destoryChild)
            DestoryChild();
    }

    public void DestoryChild()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            Destroy(transform.GetChild(i).gameObject);
        }
    }


    public Vector3 GetPointOnSurface(float i, float j, float k)
    {
        return _mapMesh.GetPointOnSurface(this, i, j, k);
    }

    public void AlignWith(MapMeshWrapper meshWrapper)
    {
        relativeRotation =
        Random.Range(0, curveSegmentCount) * 360f / _mapMesh.roadSegmentCount;

        transform.SetParent(meshWrapper.transform, false);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.Euler(0f, 0f, -meshWrapper.curveAngle);
        transform.Translate(0f, meshWrapper.curveRadius, 0f);
        transform.Rotate(relativeRotation, 0f, 0f);
        transform.Translate(0f, -curveRadius, 0f);
        transform.SetParent(meshWrapper.transform.parent);
        transform.localScale = Vector3.one;

    }
}
