using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class MeshWrapper : MonoBehaviour
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

    private void Awake()
    {
        GetComponent<MeshFilter>().mesh = mesh = new Mesh();
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

        if(destoryChild)
            for (int i = 0; i < transform.childCount; i++)
            {
                Destroy(transform.GetChild(i).gameObject);
            }
    }


    public Vector3 GetPointOnSurface(float i, float j, float k)
    {
        return _mapMesh.GetPointOnSurface(this, i, j, k);
    }

    public void AlignWith(MeshWrapper meshWrapper)
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
