using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public abstract class BaseScene : MonoBehaviour
{
    public Define.Scene SceneType { get; protected set; } = Define.Scene.Unknown;
    public UIManager baseUIManager;
    public bool SettingFinish { get; protected set; }

    void Awake()
    {
        Init();
    }

    protected virtual void Init()
    {
    }

    public abstract void Clear();
}
