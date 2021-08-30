using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RBCGraphicManager : MonoBehaviour
{
    public Material rbc_base;
    public Material rbc_damaged;

    public ParticleSystem p_rbc_damaged;


    private Material material
    {
        get {return gameObject.GetComponent<Renderer>().material;}
        set {gameObject.GetComponent<Renderer>().material = value;}
    }

    void Start()
    {
        material = gameObject.GetComponent<Renderer>().material;
    }

    void Update()
    {
        Debug.Log(material.name);
    }

    public bool checkMaterialName(string matName)
    {
        return (material.name == matName);
    }

    /*****
    최선의 방식은 아니지만 일단 임시로 코드 작성함.
    *****/

    public void ChangeToDamageMat()
    {
        if (material.name == "RBC_Base (Instance)" || material.name == "RBC_Base(Clone)")
        {
            material = Instantiate(rbc_damaged);
        }
        else
        {
            Debug.LogWarning(material.name);
            Debug.LogWarning("tried to change material to RBC_Damaged when current material is not RBC_Base.");
            return;
        }
    }

    public void ChangeToNormalMat()
    {
        if (material.name == "RBC_Damaged (Instance)" || material.name == "RBC_Damaged(Clone)")
        {
            material = Instantiate(rbc_base);
        }
        else
        {
            Debug.LogWarning(material.name);
            Debug.LogWarning("tried to change material to RBC_Base when current material is not RBC_Damaged.");
            return;
        }
    }

    public void AnimateDamageShader()
    {
        if (material.name == "RBC_Damaged (Instance)" || material.name == "RBC_Damaged(Clone)")
        {
            Debug.Log("AnimateDamageShader");
            // TODO: 셰이더 색상 동적으로 변환해서 데미지 받는 효과 주기
        }
    }

    public static GameObject FindParentWithTag(GameObject childObject, string tag)
    {
        Transform t = childObject.transform;
        while (t.parent != null)
        {
        if (t.parent.tag == tag)
        {
            return t.parent.gameObject;
        }
        t = t.parent.transform;
        }
        return null; // Could not find a parent with given tag.
    }

    public void RenderDamageParticle()
    {
        GameObject rotator = FindParentWithTag(gameObject, "PlayerRotator");
        if (rotator)
        {
            ParticleSystem particle = Instantiate(p_rbc_damaged, gameObject.transform.position, rotator.transform.localRotation);
            p_rbc_damaged.Play();
        }
    }
}
