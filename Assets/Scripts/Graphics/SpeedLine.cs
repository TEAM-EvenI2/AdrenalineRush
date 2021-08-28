using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedLine : MonoBehaviour
{
    public ParticleSystem particle;
    private ParticleSystem speedLine;

    // Start is called before the first frame update
    void Awake()
    {
        speedLine = Instantiate(particle);
    }

    // Update is called once per frame
    void Update()
    {
        speedLine.transform.position = transform.position - transform.forward * 0.3f;
    }
}
