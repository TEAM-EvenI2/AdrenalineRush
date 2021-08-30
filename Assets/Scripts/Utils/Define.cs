using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Define 
{

    public enum Scene
    {
        Unknown,
        Intro,
        Lobby,
        Game,
    }

    public enum MapMeshType
    {
        Tunnel,
        Plane,
        ReverseTunnel
    }

    [System.Serializable]
    public struct SmoothDampStruct<T>
    {
        public float smoothTime;
        [HideInInspector]public T smoothVelocity;
    }

    public class MapItemGenerateInfo
    {
        public MapItem prefab;

        public float percent;
        public float curveArc;
        public float angle;

        // for Long Obstacle
        public float angleInTunnel;
        public float size;
        public float middleSizePercent;

    }
}
