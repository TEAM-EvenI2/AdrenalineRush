using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test1 : MonoBehaviour
{
    public MapSystem mapSystem;
    public int c;
    public int p;

    [Range(0, 1f)]
    public float percent = 0;
    public float angle1 = 0;
    public float angle2 = 0;
    public int index = 0;

    public float size =1;
    public int radiusSegmentCount=1;
    public int lengthSegmentCount=5;
    [Range(.06f, 1f)]
    public float minPercent;

    public LongObstacle prefab;

    public AnimationCurve curve ;
    public float noiseStrength;

    void Start()
    {
        for (int i = 0; i < radiusSegmentCount * (lengthSegmentCount + 1); i++)
        {
            GameObject go = GameObject.CreatePrimitive(PrimitiveType.Cube);
            go.transform.SetParent(transform);
            go.transform.localScale = Vector3.one * 0.05f;
        }
        curve = AnimationCurve.EaseInOut(0, 0, 1, 1);
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

        Noise noise = new Noise();
        for (int i = 0; i < lengthSegmentCount + 1; i++)
        {
            float lengthPercent = (float)i / lengthSegmentCount;
            for (int j = 0; j < radiusSegmentCount; j++)
            {
                int idx = i * radiusSegmentCount + j;
                Transform t = transform.GetChild(idx);
                float fixedPercent = (lengthPercent - 0.5f) * 2;
                float innerAngle = ((float)j / radiusSegmentCount) * 360;
                if (i >= lengthSegmentCount / 2)
                {
                    innerAngle += 180;
                    innerAngle *= -1;
                }
                innerAngle *= Mathf.Deg2Rad;
                float x = Mathf.Sin(((float)j / radiusSegmentCount) * 360 * Mathf.Deg2Rad) * Mathf.Cos(lengthPercent * 360 * Mathf.Deg2Rad);
                float y = Mathf.Sin(((float)j / radiusSegmentCount) * 360 * Mathf.Deg2Rad) * Mathf.Sin(lengthPercent * 360 * Mathf.Deg2Rad);
                float z = Mathf.Cos(((float)j / radiusSegmentCount) * 360 * Mathf.Deg2Rad);

                float noiseValue = noise.Evaluate(new Vector3(x, y, z)) * noiseStrength;
                float _size = size * (1 + noiseValue);

                // angle + (wantedTunnelArc / tunnelRadius), wantedTunnelArc = cos(\theta) * size
                float _angle = angle1 + Mathf.Cos(innerAngle) * _size * Mathf.Rad2Deg;
                if(i > lengthSegmentCount / 2)
                {
                    _angle += 180;
                }

                float absPercent = curve.Evaluate(Mathf.Abs(fixedPercent));
                float arc = Mathf.Tan(angle2 * Mathf.Deg2Rad) * mw.mapMesh.mapSize + size + enableArc * percent +
                    (Mathf.Sin(innerAngle) * _size) * (absPercent < minPercent ? minPercent : absPercent) +
                    Mathf.Tan(angle2 * Mathf.Deg2Rad) * mw.mapMesh.mapSize * fixedPercent;

                _angle = _angle % 360;
                if (_angle < 0)
                    _angle += 360;

                float distance = mw.mapMesh.GetDistance(mw, arc / curArc, _angle / 360) * Mathf.Abs(fixedPercent) ;

                float new_percent = ((absPercent < minPercent ? minPercent : absPercent) - Mathf.Abs(fixedPercent)) / ( minPercent + 0.001f);
                Vector3 point1 = mw.mapMesh.GetPointOnSurface(mw, arc / mw.curveRadius, _angle * Mathf.Deg2Rad, distance) 
                    + new Vector3(0, -Mathf.Sin(angle1 * Mathf.Deg2Rad), Mathf.Cos(angle1 * Mathf.Deg2Rad)) * minPercent * new_percent * -Mathf.Sign(fixedPercent) * _size * Mathf.Cos(innerAngle);


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
