using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle : TunnelItem
{

    public DeformableObject[] deformableObjects;

    public override void Setting(Tunnel tunnel, float curveRotation, float ringRotation, float distanceFromCenter)
    {

        base.Setting(tunnel, curveRotation, ringRotation, distanceFromCenter);
        for (int i = 0; i < deformableObjects.Length; i++)
        {
            deformableObjects[i].Transformation(tunnel);
        }
    }
}
