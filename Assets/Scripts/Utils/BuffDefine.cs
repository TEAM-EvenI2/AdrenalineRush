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

        public BuffStruct(int id, BuffType type,float time)
        {
            this.type = type;
            this.originTime = time;
            this.time = time;
            this.id = id;
        }
    }
    public class MagnetBuffStruct : BuffStruct
    {
        public float range;
        public float power;

        public MagnetBuffStruct(int id, float time, float range, float power) :
            base(id, BuffType.Magnet, time)
        {
            this.range = range;
            this.power = power;
        }
    }
    public class SpeedBuffStruct : BuffStruct
    {
        public float distance;
        public float originSpeed;

        public SpeedBuffStruct(int id, float time, float distance, float originSpeed) :
            base(id, BuffType.Speed, time)
        {
            this.distance = distance;
            this.originSpeed = originSpeed;
        }
    }
    public class SizeBuffStruct : BuffStruct
    {
        public float sizeFactor;
        public Vector3 originalSize;
        public SizeBuffStruct(int id, float time, float sizeFactor, Vector3 originalSize) :
            base(id, BuffType.Size, time)
        {
            this.sizeFactor = sizeFactor;
            this.originalSize = originalSize;
        }
    }
}
