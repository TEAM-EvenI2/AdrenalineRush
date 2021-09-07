using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedLine : MonoBehaviour
{
    public ParticleSystem particle;
    private ParticleSystem speedLine;
    public float distance = 5f;

    // Start is called before the first frame update
    void Awake()
    {
        speedLine = Instantiate(particle);
        speedLine.transform.position = new Vector3(transform.position.x + distance, transform.position.y, transform.position.z);
        speedLine.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
