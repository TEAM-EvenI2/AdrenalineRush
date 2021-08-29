using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test1 : MonoBehaviour
{
    public MapSystem mapSystem;
    public int c;
    public int p;

    public float percent = 0;
    public float angle1 = 0;
    public float angle2 = 0;
    public int index = 0;

    public float size =1;
    public int radiusSegmentCount=1;
    public int lengthSegmentCount=5;

    public LongObstacle prefab;

    void Start()
    {
        for (int i = 0; i < radiusSegmentCount * (lengthSegmentCount + 1); i++)
        {
            GameObject go = GameObject.CreatePrimitive(PrimitiveType.Cube);
            go.transform.SetParent(transform);
            go.transform.localScale = Vector3.one * 0.1f;
        }
    }


    private void Update()
    {
        if (index < 0 || index >= mapSystem.maps.Length)
            return;

        MapMeshWrapper mw = mapSystem.maps[index];

        if (Input.GetKeyDown(KeyCode.Space))
        {

            Instantiate(prefab).Setting(mw, 0, 0, 0);
        }

        mw.transform.position = Vector3.zero;
        mw.transform.eulerAngles = Vector3.zero;

        float curArc = mw.curveAngle * Mathf.Deg2Rad * mw.curveRadius ;
        float enableArc = curArc - (size + Mathf.Tan(angle2 * Mathf.Deg2Rad) * mw.mapMesh.mapSize) * 2;
        for (int i = 0; i < lengthSegmentCount + 1; i++)
        {
            float lengthPercent = (float)i / lengthSegmentCount;
            for (int j = 0; j < radiusSegmentCount; j++)
            {
                int idx = i * radiusSegmentCount + j;
                Transform t = transform.GetChild(idx);
                float fixedPercent = i > lengthSegmentCount / 2?((lengthPercent - 0.5f) * 2): ((0.5f - lengthPercent) * 2);

                // angle + (wantedTunnelArc / tunnelRadius), wantedTunnelArc = cos(\theta) * size
                float _angle = angle1 + Mathf.Cos(((float)j / radiusSegmentCount) * 360 * Mathf.Deg2Rad) * size * Mathf.Rad2Deg;
                if(i > lengthSegmentCount / 2)
                {
                    _angle += 180;
                }
                float arc = Mathf.Tan(angle2 * Mathf.Deg2Rad) * mw.mapMesh.mapSize + size + enableArc * percent +
                    (Mathf.Sin(((float)j / radiusSegmentCount) * 360 * Mathf.Deg2Rad) * size) * fixedPercent +
                    Mathf.Tan(angle2 * Mathf.Deg2Rad) * mw.mapMesh.mapSize * -(lengthPercent - 0.5f) * 2;

                _angle = _angle % 360;
                if (_angle < 0)
                    _angle += 360;

                float distance = mw.mapMesh.GetDistance(mw, arc / curArc, _angle / 360) * fixedPercent ;
                Vector3 point1 = mw.mapMesh.GetPointOnSurface(mw, arc / mw.curveRadius, _angle * Mathf.Deg2Rad, distance);


                t.position = point1;
            }
        }
    }

    private void StartFunc1()
    {

        for (int i = 0; i < mapSystem.mapCount; i++)
        {
            for (int j = 0; j < c; j++)
            {
                for (int k = 0; k < p; k++)
                {
                    GameObject a1 = new GameObject();
                    a1.transform.SetParent(transform);
                    GameObject b1 = new GameObject();
                    b1.transform.SetParent(a1.transform);
                    GameObject go = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    go.transform.SetParent(b1.transform);
                    go.transform.localScale = Vector3.one * 0.3f + Vector3.up * 0.2f;
                    go.transform.localPosition = -Vector3.up * 0.25f;

                    go.GetComponent<MeshRenderer>().material.color = new Color(0, 0, k / (float)p);
                }
            }
        }
    }

    private void UpdateFunc1()
    {

        float worldRotation = 0;
        float angle = 360 / p * Mathf.Deg2Rad;
        for (int i = 0; i < mapSystem.mapCount; i++)
        {
            float a = (mapSystem.maps[i].curveAngle / c) * Mathf.Deg2Rad;
            worldRotation += mapSystem.maps[i].cumulativeRelativeRotation / 2;

            for (int j = 0; j < c; j++)
            {
                for (int k = 0; k < p; k++)
                {
                    Transform t = transform.GetChild(i * c * p + j * p + k);

                    t.position = Utils.RotateTranslate(
                        mapSystem.maps[i].GetPointOnSurface(a * j, angle * k, mapSystem.maps[i].mapMesh.GetDistance(mapSystem.maps[i], j / (float)c, k / (float)p)),
                        mapSystem.maps[i].transform.eulerAngles,
                        mapSystem.maps[i].transform.position);
                    t.localEulerAngles = mapSystem.maps[i].transform.localEulerAngles + new Vector3(0, 0, -a * j) * Mathf.Rad2Deg;
                    t.GetChild(0).localEulerAngles = new Vector3(angle * k, 0, 0) * Mathf.Rad2Deg;
                }
            }
        }
    }
}
