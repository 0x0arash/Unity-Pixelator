using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ArashGh.Pixelator.Runtime.Utils
{
    public static class Color32Ext
    {
        public static bool IsEqualTo(this Color32 aCol, Color32 aRef)
        {
            return aCol.r == aRef.r && aCol.g == aRef.g && aCol.b == aRef.b && aCol.a == aRef.a;
        }
    }
}
