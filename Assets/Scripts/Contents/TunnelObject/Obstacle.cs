using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle : MapItem
{

    public DeformableObject[] deformableObjects;

    public override void Setting(MeshWrapper mw, float curveRotation, float ringRotation, float distanceFromCenter)
    {

        base.Setting(mw, curveRotation, ringRotation, distanceFromCenter);
        for (int i = 0; i < deformableObjects.Length; i++)
        {
            deformableObjects[i].Transformation(mw);
        }
    }
}
