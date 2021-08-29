using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedLine : MonoBehaviour
{
    public ParticleSystem particle;
    private ParticleSystem speedLine;
    public float distance = -2.0f;

    // Start is called before the first frame update
    void Awake()
    {
        speedLine = Instantiate(particle);
    }

    // Update is called once per frame
    void Update()
    {
        // TODO: 속도에 따라 선택적으로 파티클 적용
        speedLine.transform.position = new Vector3(transform.position.x * distance, transform.position.y, transform.position.z);
    }
}
