using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectController : MonoBehaviour
{
    [System.Serializable]
    public class EffectDict
    {
        public string name;
        public float timeToDestroy;
        public Transform prefab;
    }

    public List<EffectDict> effects;
    private Dictionary<string, EffectDict> prefabDict = new Dictionary<string, EffectDict>();

    private void Awake()
    {
        foreach(EffectDict ed in effects)
        {
            prefabDict.Add(ed.name, ed);
        }
    }

    public Transform CreateEffect(string name, bool deleteAfterTime=false)
    {
        EffectDict ed = null;
        if (!prefabDict.TryGetValue(name, out ed)) 
            return null;

        Transform effect = Instantiate(ed.prefab);

        if (deleteAfterTime)
            Destroy(effect.gameObject, ed.timeToDestroy);

        return effect;

    }


}
