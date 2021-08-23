using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowObstacle : TunnelItem
{
    public SimpleAnimationMove sam;

	public override void Setting(Tunnel pipe, float curveRotation, float ringRotation, float distanceFromCenter)
    {
        base.Setting(pipe, curveRotation, ringRotation, distanceFromCenter);

        float deltaToRotation = 360f / (2f * Mathf.PI * pipe.CurveRadius);

        float delta = Managers.Instance.GetScene<GameScene>().player.velocity * Time.deltaTime;

        sam.Setting(curveRotation / (delta * deltaToRotation));
    }
}
