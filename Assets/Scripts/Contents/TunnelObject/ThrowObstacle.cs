using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowObstacle : MapItem
{
    public SimpleAnimationMove sam;

	public override void Setting(MeshWrapper mw, float curveRotation, float ringRotation, float distanceFromCenter)
    {
        base.Setting(mw, curveRotation, ringRotation, distanceFromCenter);

        float deltaToRotation = 360f / (2f * Mathf.PI * mw.curveRadius);

        float delta = Managers.Instance.GetScene<GameScene>().player.velocity * Time.deltaTime;

        sam.Setting(curveRotation / (delta * deltaToRotation));
    }
}
