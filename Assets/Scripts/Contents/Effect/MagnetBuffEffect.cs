using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagnetBuffEffect : MonoBehaviour
{
    public ParticleSystem dust;
    public ParticleSystem magneticField1;

    public void Setting(float size)
    {
        ParticleSystem.ShapeModule sm = dust.shape;
        sm.scale  = Vector3.one * size * 2;

        ParticleSystem.MainModule main = dust.main;
        main.startSpeedMultiplier = -size;

        main = magneticField1.main;
        main.startSizeMultiplier = size * 2;

    }

    public void Stop()
    {

        dust.Stop();
        magneticField1.Stop();

        Invoke("Disable", 1f);
    }

    private void Disable()
    {
        gameObject.SetActive(false);
    }

}
