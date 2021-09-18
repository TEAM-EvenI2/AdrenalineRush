using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GraphicManager : MonoBehaviour
{
    private RBCGraphicManager rbcGraphicManager;
    public ParticleSystem p_itemCollided; // 아이템에 적용할 매터리얼

    public void Init()
    {

        int index = DataManager.instance.gameData.equippedCharaIndex;
        for(int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).gameObject.SetActive(false);
        }
        transform.GetChild(index).gameObject.SetActive(true);
        rbcGraphicManager = transform.GetChild(index).GetComponent<RBCGraphicManager>();
    }
     
    public void Damaged()
    {
        StartCoroutine(CoDamaged());
    }

    public void Die()
    {
        StartCoroutine(CoDie());
    }

    public void CollideItem(GameObject item)
    {
        StartCoroutine(CoCollideItem(item));
    }

    IEnumerator CoDamaged()
    {
        rbcGraphicManager.ChangeToDamageMat();
        rbcGraphicManager.AnimateDamageShader();
        rbcGraphicManager.RenderDamageParticle();
        yield return new WaitForSeconds(1f);
        rbcGraphicManager.ChangeToNormalMat();
    }

    IEnumerator CoDie()
    {
        rbcGraphicManager.ChangeToDamageMat();
        rbcGraphicManager.AnimateDamageShader();
        rbcGraphicManager.RenderDieParticle();
        yield return new WaitForSeconds(1f);
        rbcGraphicManager.ChangeToNormalMat();
    }

    IEnumerator CoCollideItem(GameObject item)
    {
        // RBCGraphicManager를 만든 것처럼 ItemGraphicManager를 따로 만드는게 훨씬 좋은 구조이지만, 
        // RBC와 달리 아이템은 오브젝트 수가 많고 또 게임 자체가 복잡하지 않기 때문에 GraphicManager에서 아이템 관련 그래픽 효과까지 처리.

        ParticleSystem itemParticle = Instantiate(p_itemCollided, item.transform.position, item.transform.localRotation);
        Destroy(item);
        yield return new WaitForSeconds(0.1f);
    }
}
