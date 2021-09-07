using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test1 : MonoBehaviour
{
    [System.Serializable]
    public class LongObstacleTest
    {
        [Range(0, 1f)]
        public float percent = 0;
        public float angle1 = 0;
        public float angle2 = 0;
        public int index = 0;

        public float size = 1;
        public int radiusSegmentCount = 1;
        public int lengthSegmentCount = 5;
        [Range(.06f, 1f)]
        public float minPercent;

        public LongObstacle prefab;

        public AnimationCurve curve;

        public float noiseStrength;
    }

    [System.Serializable]
    public class MapObstacleTest
    {
        public int index;
        [Range(0, 1f)]
        public float percent = 0;
        public float angle1 = 0;

        public float curveLength = 5;
        public int curveSegmentCount = 5;
        public int radiusSegmentCount = 5;
        public AnimationCurve curve;

        public SurfaceObstacle prefab;
        public float loadWidth = 1;
    }

    public MapSystem mapSystem;
    public int c;
    public int p;

    [Header("For long obstacle")]
    public LongObstacleTest lot;

    [Header("For mesh obstacle")]
    public MapObstacleTest mot;

    void Start()
    {
        for (int i = 0; i < (mot.radiusSegmentCount) * (mot.curveSegmentCount +1); i++)
        {
            GameObject go = GameObject.CreatePrimitive(PrimitiveType.Cube);
            go.transform.SetParent(transform);
            go.transform.localScale = Vector3.one * 0.05f;
            go.GetComponent<MeshRenderer>().material.color = Color.black;
        }
        
        }

        private Vector2 GetForwardVector()
        {
            // Least-Square
            float a = 0, b;

            float mean_u = 0;
            float mean_f_u = 0;
            for (int i = 0; i < mot.curveSegmentCount + 1; i++)
            {
                float u = (float)i / (mot.curveSegmentCount);
                float f_u = mot.curve.Evaluate(u);
                mean_u += u;
                mean_f_u += f_u;
            }
            mean_u /= mot.curveSegmentCount + 1;
            mean_f_u /= mot.curveSegmentCount + 1;

            float p = 0;
            for (int i = 0; i < mot.curveSegmentCount + 1; i++)
            {
                float u = (float)i / (mot.curveSegmentCount);
                float f_u = mot.curve.Evaluate(u);
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

    private void Update()
    {
        if (mot.index < 0 || mot.index >= mapSystem.maps.Length)
            return;

        MapMeshWrapper mw = mapSystem.maps[mot.index];
        mw.transform.position = Vector3.zero;
        mw.transform.eulerAngles = Vector3.zero;


        if (Input.GetKeyDown(KeyCode.Space))
        {

            Instantiate(mot.prefab).Setting(mw, 0, 0, 0);
        }

        float width = mot.curveLength;
        float height = 2 * Mathf.PI * mw.mapMesh.mapSize;

        Vector2 forwardVec = GetForwardVector();
        Vector2 left = new Vector2(-forwardVec.y / width, forwardVec.x / height) * mot.loadWidth ;

        System.Func<float, float> leftFunc = (float x) => { return (mot.curve.Evaluate(x - left.x) + left.y) ; };
        System.Func<float, float> rightFunc = (float x) => { return (mot.curve.Evaluate(x + left.x) - left.y); };

        float curArc = mw.curveRadius * mw.curveAngle * Mathf.Deg2Rad;
        float enableArc = curArc - (mot.curveLength);

        for (int i = 0; i < mot.curveSegmentCount + 1; i++)
        {
            float u = (float)i / (mot.curveSegmentCount);
            float o_u = mot.curve.Evaluate(u) * ((float)(mot.radiusSegmentCount - 1) / mot.radiusSegmentCount);
            float l_u = leftFunc(u) * ((float)(mot.radiusSegmentCount - 1) / mot.radiusSegmentCount);
            float r_u = rightFunc(u) * ((float)(mot.radiusSegmentCount - 1) / mot.radiusSegmentCount);
            //Debug.DrawLine(forwardVec * u * mot.width, forwardVec * u * mot.width + left, Color.yellow);
            //Debug.DrawLine(forwardVec * u * mot.width, forwardVec * u * mot.width - left, Color.green);
            //Debug.DrawLine(forwardVec * u * mot.width, forwardVec * (u + 1 / (float)(mot.curveSegmentCount)) * mot.width, Color.blue);

            Debug.DrawLine(new Vector3(u * width, o_u * height) , new Vector3((u + 1 / (float)(mot.curveSegmentCount)) * width, mot.curve.Evaluate((u + 1 / (float)(mot.curveSegmentCount))) * ((float)(mot.radiusSegmentCount - 1) / mot.radiusSegmentCount) * height), Color.white);
            Debug.DrawLine(new Vector3(u * width, l_u * height) , new Vector3((u + 1 / (float)(mot.curveSegmentCount)) * width, leftFunc((u + 1 / (float)(mot.curveSegmentCount))) * ((float)(mot.radiusSegmentCount - 1) / mot.radiusSegmentCount) * height), Color.yellow);
            Debug.DrawLine(new Vector3(u * width, r_u * height) , new Vector3((u + 1 / (float)(mot.curveSegmentCount)) * width, rightFunc((u + 1 / (float)(mot.curveSegmentCount))) * ((float)(mot.radiusSegmentCount - 1) / mot.radiusSegmentCount) * height), Color.green);

            for (int j = 0; j < mot.radiusSegmentCount; j++)
            {
                int idx = i * mot.radiusSegmentCount + j;
                Transform t = transform.GetChild(idx);
                float v  = (float)j / mot.radiusSegmentCount ;

                t.GetComponent<MeshRenderer>().material.color = Color.black;
                if (v > l_u )
                {
                    t.GetComponent<MeshRenderer>().material.color = new Color(0, u, v);
                }
                else if (v < r_u )
                {
                    t.GetComponent<MeshRenderer>().material.color = new Color(v, u, 0);
                }
                else
                {
                    //t.position = Vector3.one * -1f;
                   continue;
                }

                float arc = u * mot.curveLength + enableArc * mot.percent;
                float angle = v * 360 + mot.angle1;

                Vector3 point = mw.mapMesh.GetPointOnSurface(mw, (arc) / mw.curveRadius, angle * Mathf.Deg2Rad, mw.mapMesh.GetDistance(mw, u, v));
                //Vector3 point = new Vector2(u * width, v * height);
                t.position = point;
            }
        }
        //for (int i = 0; i < (mot.curveSegmentCount + 1); i++)
        //{
        //    float u = (float)i / (mot.curveSegmentCount);
        //    Vector2 point = new Vector2(u, mot.curve.Evaluate(u));
        //    for (int j = 0; j < 3; j++)
        //    {
        //        int idx = (mot.radiusSegmentCount) * (mot.curveSegmentCount + 1) + i * 3 + j;
        //        Transform t = transform.GetChild(idx);

        //        Vector2 forward = Vector2.zero;
        //        if (i < mot.curveSegmentCount)
        //        {
        //            float _u = (float)(i + 1) / (mot.curveSegmentCount);
        //            forward += new Vector2(_u, mot.curve.Evaluate(_u)) - point;
        //        }

        //        if (i > 0)
        //        {
        //            float _u = (float)(i - 1) / (mot.curveSegmentCount);
        //            forward += point - new Vector2(_u, mot.curve.Evaluate(_u));
        //        }
        //        forward.Normalize();

        //        Vector2 dir = Vector2.zero;
        //        if (j == 0)
        //        {
        //            t.GetComponent<MeshRenderer>().material.color = Color.green;
        //            dir = -new Vector2(-forward.y, forward.x);
        //        }
        //        else if (j == 2)
        //        {
        //            t.GetComponent<MeshRenderer>().material.color = Color.yellow;
        //            dir = new Vector2(-forward.y, forward.x);
        //        }

        //        float arc = point.x * mot.curveLength + dir.x * mot.loadWidth / 2 + enableArc * mot.percent;
        //        float angle = point.y * 360 + dir.y * (mot.loadWidth / 2) / mw.mapMesh.mapSize * Mathf.Rad2Deg + mot.angle1;

        //        Vector3 point1 = mw.mapMesh.GetPointOnSurface(mw,
        //            (arc) / mw.curveRadius,
        //            (angle) * Mathf.Deg2Rad,
        //            mw.mapMesh.GetDistance(mw, point.x + (dir.x * mot.loadWidth / 2) / mot.curveLength, point.y + ((dir.y * (mot.loadWidth / 2) / mw.mapMesh.mapSize) * Mathf.Rad2Deg) / 360));
        //        Vector3 point1 = new Vector3((point.x * mot.width + dir.x * mot.loadWidth / 2), (point.y * mot.height + dir.y * mot.loadWidth / 2));
        //        t.position = point1;
        //    }
        //}
    }

    private void StartLongObstacle()
    {
        for (int i = 0; i < lot.radiusSegmentCount * (lot.lengthSegmentCount + 1); i++)
        {
            GameObject go = GameObject.CreatePrimitive(PrimitiveType.Cube);
            go.transform.SetParent(transform);
            go.transform.localScale = Vector3.one * 0.05f;
        }
        lot.curve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    }

    private void UpdateLongObstacle()
    {
        if (lot.index < 0 || lot.index >= mapSystem.maps.Length)
            return;

        MapMeshWrapper mw = mapSystem.maps[lot.index];

        if (Input.GetKeyDown(KeyCode.Space))
        {

            Instantiate(lot.prefab).Setting(mw, 0, 0, 0);
        }

        mw.transform.position = Vector3.zero;
        mw.transform.eulerAngles = Vector3.zero;

        float curArc = mw.curveAngle * Mathf.Deg2Rad * mw.curveRadius;
        float enableArc = curArc - (lot.size + Mathf.Tan(lot.angle2 * Mathf.Deg2Rad) * mw.mapMesh.mapSize) * 2;

        Noise noise = new Noise();
        for (int i = 0; i < lot.lengthSegmentCount + 1; i++)
        {
            float lengthPercent = (float)i / lot.lengthSegmentCount;
            for (int j = 0; j < lot.radiusSegmentCount; j++)
            {
                int idx = i * lot.radiusSegmentCount + j;
                Transform t = transform.GetChild(idx);
                float fixedPercent = (lengthPercent - 0.5f) * 2;
                float innerAngle = ((float)j / lot.radiusSegmentCount) * 360;
                if (i >= lot.lengthSegmentCount / 2)
                {
                    innerAngle += 180;
                    innerAngle *= -1;
                }
                innerAngle *= Mathf.Deg2Rad;
                float x = Mathf.Sin(((float)j / lot.radiusSegmentCount) * 360 * Mathf.Deg2Rad) * Mathf.Cos(lengthPercent * 360 * Mathf.Deg2Rad);
                float y = Mathf.Sin(((float)j / lot.radiusSegmentCount) * 360 * Mathf.Deg2Rad) * Mathf.Sin(lengthPercent * 360 * Mathf.Deg2Rad);
                float z = Mathf.Cos(((float)j / lot.radiusSegmentCount) * 360 * Mathf.Deg2Rad);

                float noiseValue = noise.Evaluate(new Vector3(x, y, z)) * lot.noiseStrength;
                float _size = lot.size * (1 + noiseValue);

                // angle + (wantedTunnelArc / tunnelRadius), wantedTunnelArc = cos(\theta) * size
                float _angle = lot.angle1 + Mathf.Cos(innerAngle) * _size * Mathf.Rad2Deg;
                if (i > lot.lengthSegmentCount / 2)
                {
                    _angle += 180;
                }

                float absPercent = lot.curve.Evaluate(Mathf.Abs(fixedPercent));
                float arc = Mathf.Tan(lot.angle2 * Mathf.Deg2Rad) * mw.mapMesh.mapSize + lot.size + enableArc * lot.percent +
                    (Mathf.Sin(innerAngle) * _size) * (absPercent < lot.minPercent ? lot.minPercent : absPercent) +
                    Mathf.Tan(lot.angle2 * Mathf.Deg2Rad) * mw.mapMesh.mapSize * fixedPercent;

                _angle = _angle % 360;
                if (_angle < 0)
                    _angle += 360;

                float distance = mw.mapMesh.GetDistance(mw, arc / curArc, _angle / 360) * Mathf.Abs(fixedPercent);

                float new_percent = ((absPercent < lot.minPercent ? lot.minPercent : absPercent) - Mathf.Abs(fixedPercent)) / (lot.minPercent + 0.001f);
                Vector3 point1 = mw.mapMesh.GetPointOnSurface(mw, arc / mw.curveRadius, _angle * Mathf.Deg2Rad, distance)
                    + new Vector3(0, -Mathf.Sin(lot.angle1 * Mathf.Deg2Rad), Mathf.Cos(lot.angle1 * Mathf.Deg2Rad)) * lot.minPercent * new_percent * -Mathf.Sign(fixedPercent) * _size * Mathf.Cos(innerAngle);


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
