using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class Obstacle : MapItem
{

    public Transform[] deformableObjects;

    public override void Setting(MeshWrapper mw, float curverPecent, float ringPercent, float distanceFromCenter)
    {

        base.Setting(mw, curverPecent, ringPercent, distanceFromCenter);

        float curveArc = mw.curveRadius * mw.curveAngle * Mathf.Deg2Rad;
        float curveRotation = ((curverPecent * curveArc) / mw.curveRadius) * Mathf.Rad2Deg;
        float ringRotation = ringPercent * 360;

        for (int i = 0; i < deformableObjects.Length; i++)
        {
            float size = 1;
            if (mw.meshType == MapMeshType.Tunnel)
            {
                Vector3 p0 = mw.GetPointOnSurface(curveRotation * Mathf.Deg2Rad, ringRotation * Mathf.Deg2Rad,0);
                Vector3 p1 = mw.GetPointOnSurface(curveRotation * Mathf.Deg2Rad, ringRotation * Mathf.Deg2Rad, mw.mapMesh.GetDistance(mw, curverPecent, ringPercent));
                Vector3 p2 = mw.GetPointOnSurface(curveRotation * Mathf.Deg2Rad, (ringRotation + 180) * Mathf.Deg2Rad, mw.mapMesh.GetDistance(mw, curverPecent, (ringPercent + 0.5f) % 1f));
                size = Vector3.Distance(p1, p2);
            }

            deformableObjects[i].localScale = new Vector3(deformableObjects[i].localScale.x, size, deformableObjects[i].localScale.z);
            deformableObjects[i].localPosition = new Vector3(0, -size / 2, 0) ;
        }
    }
}
