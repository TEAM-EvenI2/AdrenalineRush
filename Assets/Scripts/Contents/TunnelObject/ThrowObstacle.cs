using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowObstacle : MapItem
{
    public SimpleAnimationMove sam;

	public override void Setting(MapMeshWrapper mw, float curveRotation, float ringRotation, float distanceFromCenter)
    {
        base.Setting(mw, curveRotation, ringRotation, distanceFromCenter);

        float deltaToRotation = 360f / (2f * Mathf.PI * mw.curveRadius);

        float delta = Managers.Instance.GetScene<GameScene>().player.curVelocity * Time.deltaTime;

        sam.Setting(curveRotation / (delta * deltaToRotation));
    }
}
