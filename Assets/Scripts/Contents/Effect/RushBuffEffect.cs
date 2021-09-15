using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RushBuffEffect : MonoBehaviour
{
    public ParticleSystem particle1;
    public ParticleSystem particle2;

    public void Setting(float speed)
    {
    }

    public void Stop()
    {

        particle1.Stop();
        particle2.Stop();

        Invoke("Disable", 1.5f);
    }

    private void Disable()
    {
        gameObject.SetActive(false);
    }

}
