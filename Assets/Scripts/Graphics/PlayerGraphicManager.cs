using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGraphicManager : MonoBehaviour
{
    private RBCGraphicManager graphicManager;

    void Awake()
    {
        graphicManager = gameObject.GetComponentInChildren<RBCGraphicManager>();
    }

    public void Damaged()
    {
        if (!graphicManager.checkMaterialName("RBC_Base (Instance)") && !(graphicManager.checkMaterialName("RBC_Base(Clone)")))
        {
            graphicManager.ChangeToNormalMat();
        }
        graphicManager.ChangeToDamageMat();
        graphicManager.AnimateDamageShader(); // TODO: 몇 초 동안 지속 후 다시 원래 매터리얼로 돌아오게 만들기
        graphicManager.RenderDamageParticle();
    }
}
