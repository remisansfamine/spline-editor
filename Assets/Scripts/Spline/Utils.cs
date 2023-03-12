using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Utils
{
    // [from1; to1] -> [from2; to2]
    public static float Remap(this float value, float from1, float to1, float from2, float to2) => (value - from1) / (to1 - from1) * (to2 - from2) + from2;
}
