using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HermitianSpline : SplineType
{
    public override Matrix4x4 GetConstantMatrix()
    {
        return new Matrix4x4(new Vector4(2f, -2f, 1f, 1f), new Vector4(-3f, 3f, -2f, -1f), new Vector4(0f, 0f, 1f, 0f), new Vector4(1f, 0f, 0f, 0f));
    }

}
