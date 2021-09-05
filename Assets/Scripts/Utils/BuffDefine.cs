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
        public BuffType type;
        public float time;

        public BuffStruct(BuffType type,float time)
        {
            this.type = type;
            this.time = time;
        }
    }
    public class MagnetBuffStruct : BuffStruct
    {
        public float range;
        public float power;

        public MagnetBuffStruct(float time, float range, float power) :
            base(BuffType.Magnet, time)
        {
            this.range = range;
            this.power = power;
        }
    }
    public class SpeedBuffStruct : BuffStruct
    {
        public float distance;

        public SpeedBuffStruct(float time, float distance) :
            base(BuffType.Speed, time)
        {
            this.distance = distance;
        }
    }
    public class SizeBuffStruct : BuffStruct
    {
        public float sizeFactor;
        public Vector3 originalSize;
        public SizeBuffStruct(float time, float sizeFactor, Vector3 originalSize) :
            base(BuffType.Size, time)
        {
            this.sizeFactor = sizeFactor;
            this.originalSize = originalSize;
        }
    }
}
