using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class Define
{

    public enum BuffType
    {
        Magnet,
        Size,
        Speed
    }

    public class BuffStruct
    {
        public int id;
        public BuffType type;
        public float originTime;
        public float time;
        public float coolTime;

        public BuffStruct(int id, BuffType type,float time, float coolTime)
        {
            this.type = type;
            this.originTime = time;
            this.time = time;
            this.id = id;
            this.coolTime = coolTime;
        }
    }
    public class MagnetBuffStruct : BuffStruct
    {
        public float range;

        public MagnetBuffStruct(int id, float time, float coolTime, float range) :
            base(id, BuffType.Magnet, time, coolTime)
        {
            this.range = range;
        }
    }
    public class SpeedBuffStruct : BuffStruct
    {
        public float speed;
        public float originSpeed;
        public bool invincibility;

        public SpeedBuffStruct(int id, float time, float coolTime, float speed, bool invincibility) :
            base(id, BuffType.Speed, time, coolTime)
        {
            this.speed = speed;
            this.invincibility = invincibility;
        }

        public SpeedBuffStruct(int id, float time, float coolTime, float speed, float originSpeed, bool invincibility) :
            base(id, BuffType.Speed, time, coolTime)
        {
            this.speed = speed;
            this.originSpeed = originSpeed;
            this.invincibility = invincibility;
        }
    }
    public class SizeBuffStruct : BuffStruct
    {
        public float sizeFactor;
        public Vector3 originalSize;
        public Vector3 collisionOriginalSize;
        public SizeBuffStruct(int id, float time, float coolTime, float sizeFactor) :
            base(id, BuffType.Size, time, coolTime)
        {
            this.sizeFactor = sizeFactor;
        }
        public SizeBuffStruct(int id, float time, float coolTime, float sizeFactor, Vector3 originalSize, Vector3 collisionOriginalSize) :
            base(id, BuffType.Size, time, coolTime)
        {
            this.sizeFactor = sizeFactor;
            this.originalSize = originalSize;
            this.collisionOriginalSize = collisionOriginalSize;
        }
    }

    public class BuffTextStruct
    {
        public string explain;
        public string name;

        public BuffTextStruct(string explain, string name)
        {
            this.explain = explain;
            this.name = name;
        }
    }
}
