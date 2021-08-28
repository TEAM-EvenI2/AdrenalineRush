using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test1 : MonoBehaviour
{
    public MapSystem mapSystem;
    public int c;
    public int p;

    void Start()
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


    private void Update()
    {
        float worldRotation = 0;
        float angle = 360 / p * Mathf.Deg2Rad;
        for (int i = 0; i < mapSystem.mapCount; i++) 
        {
            float a = (mapSystem.maps[i].curveAngle / c) * Mathf.Deg2Rad;
            float r = mapSystem.maps[i].mapMesh.mapSize;
            worldRotation += mapSystem.maps[i].cumulativeRelativeRotation / 2;

            for (int j = 0; j  < c; j++)
            {
                for (int k = 0; k < p; k++)
                {
                    Transform t = transform.GetChild(i * c * p + j * p + k);

                    t.position = Utils.RotateTranslate(mapSystem.maps[i].GetPointOnSurface(a * j, angle * k, r), mapSystem.maps[i].transform.eulerAngles, mapSystem.maps[i].transform.position);
                    t.localEulerAngles =mapSystem.maps[i].transform.localEulerAngles+ new Vector3(0, 0, -a * j) * Mathf.Rad2Deg;
                    t.GetChild(0).localEulerAngles = new Vector3(angle * k, 0, 0) * Mathf.Rad2Deg;
                }
            }
        }
    }
}
