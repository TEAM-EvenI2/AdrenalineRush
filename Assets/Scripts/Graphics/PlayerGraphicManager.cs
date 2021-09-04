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
        StartCoroutine(CoDamaged());
    }

    public void Die()
    {
        StartCoroutine(CoDie());
    }

    IEnumerator CoDamaged()
    {
        graphicManager.ChangeToDamageMat();
        graphicManager.AnimateDamageShader();
        graphicManager.RenderDamageParticle();
        yield return new WaitForSeconds(1f);
        graphicManager.ChangeToNormalMat();
    }

    IEnumerator CoDie()
    {
        graphicManager.ChangeToDamageMat();
        graphicManager.AnimateDamageShader();
        graphicManager.RenderDieParticle();
        yield return new WaitForSeconds(1f);
        graphicManager.ChangeToNormalMat();
    }
}
