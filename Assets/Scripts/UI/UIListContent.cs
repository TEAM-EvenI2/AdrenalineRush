using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class UIListContent<T> : MonoBehaviour
{
    protected T item;
    protected bool init;


    public virtual void Init()
    {
        init = true;
    }

    public virtual void Setting(T item)
    {
        this.item = item;
        if (!init)
        {
            Init();
        }
    }
    
}