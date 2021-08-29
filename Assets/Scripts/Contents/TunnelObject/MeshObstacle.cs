using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshObstacle : MapItem
{
    public float angle;
    public override void Setting(MapMeshWrapper mw, float curverPecent, float ringPercent, float distanceFromCenter)
    {
        Generate(mw, curverPecent, ringPercent * 360);
    }

    public virtual void Generate(MapMeshWrapper mw, float percent, float angle)
    {
        transform.SetParent(mw.transform, false);
        transform.localPosition = Vector3.zero;
    }
    
}
