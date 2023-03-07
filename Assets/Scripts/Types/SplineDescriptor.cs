using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SplineDescriptor : ScriptableObject
{
    public virtual void GetT(float u, int inputCount, out float t, out int startingPoint)
    {
        t = 0f;
        startingPoint = 0;
    }

    public virtual Vector3 EvaluateFromPolynomial(float u, List<Vector3> inputPoints) => Vector3.zero;
    public virtual Vector3 EvaluateFromMatrix(float u, List<Vector3> inputPoints) => Vector3.zero;

    public virtual bool IsPointAKnot(int PointID) => false;
}
