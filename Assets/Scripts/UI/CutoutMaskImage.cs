using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class CutoutMaskImage : Image
{
    private Material mat;

    public override Material materialForRendering
    {
        get
        {
            if (mat == null)
            {
                mat = new Material(base.materialForRendering);
                mat.SetInt("_StencilComp", (int)CompareFunction.NotEqual);
            }
            return mat;
        }
    }

}
