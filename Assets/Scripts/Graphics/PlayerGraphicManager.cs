using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGraphicManager : MonoBehaviour
{
    private RBCMaterialManager materialManager;

    void Awake()
    {
        materialManager = gameObject.GetComponentInChildren<RBCMaterialManager>();
    }

    public void Damaged()
    {
        if (!materialManager.checkMaterialName("RBC_Base (Instance)") && !(materialManager.checkMaterialName("RBC_Base(Clone)")))
        {
            materialManager.ChangeToNormalMat();
        }
        materialManager.ChangeToDamageMat();
        materialManager.AnimateDamageShader(); // TODO: 몇 초 동안 지속 후 다시 원래 매터리얼로 돌아오게 만들기
    }
}
