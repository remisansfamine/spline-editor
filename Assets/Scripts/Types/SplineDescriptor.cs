using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SplineDescriptor : ScriptableObject
{
    public void GetT(float u, int inputCount, out float t, out int startingPoint)
    {
        int IntervalleCount = inputCount / 4;
        int StartingKnot = (int)(IntervalleCount * u);
        startingPoint = StartingKnot * 4;

        t = u - StartingKnot / IntervalleCount;
    }

    public virtual Vector3 EvaluateFromPolynomial(float u, List<Vector3> inputPoints) => Vector3.zero;
}
