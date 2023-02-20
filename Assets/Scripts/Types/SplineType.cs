using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SplineType : ScriptableObject
{
    public abstract Matrix4x4 GetConstantMatrix();

}
