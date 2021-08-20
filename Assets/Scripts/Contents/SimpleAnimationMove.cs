using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleAnimationMove : MonoBehaviour
{

    public Transform[] targets;
    public float speed;

    private int index;

    private float t;

    public void Setting(float delay)
    {
        index = 1;
        if (targets.Length > 0)
            transform.position = targets[0].position;
        print(delay + ":" + Vector3.Distance(transform.position, targets[index].position) / (speed * Time.deltaTime));
        t = delay - Vector3.Distance(transform.position, targets[index].position) / (speed * Time.deltaTime);
    }

    // Update is called once per frame
    void Update()
    {
        if (t > 0)
        {
            t -= Time.deltaTime;
        }
        else
        {

            if (targets.Length > 0 && index < targets.Length)
            {
                Vector3 dir = GetDirToTarget();

                if (speed * Time.deltaTime > Vector3.Distance(transform.position, targets[index].position))
                {
                    transform.position += dir * Vector3.Distance(transform.position, targets[index].position);
                    index++;
                }
                else
                    transform.position += dir * speed * Time.deltaTime;

            }
        }

    }

    private Vector3 GetDirToTarget()
    {
        return (targets[index].position - transform.position).normalized;
    }
}
