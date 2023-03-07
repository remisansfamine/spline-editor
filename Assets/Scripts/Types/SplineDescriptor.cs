using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SplineDescriptor : ScriptableObject
{
    public void GetT(float u, int inputCount, out float t, out int startingPoint)
    {
        //int knotCount = inputCount / 2;
        //startingPoint = Mathf.Clamp(Mathf.FloorToInt(u * (knotCount - 1)), 0, knotCount - 2);

        //t = u * (knotCount - 1) - startingPoint;

        int intervalleCount = inputCount / 4;
        int startingKnot = (int)(intervalleCount * u);
        startingPoint = startingKnot * 4;

        t = (u - startingKnot / (float)intervalleCount) * 2f;

        Debug.Log($"u = {u}, t = {t}, startingpoint = {startingPoint}");
    }

    public virtual Vector3 EvaluateFromPolynomial(float u, List<Vector3> inputPoints) => Vector3.zero;
    public virtual Vector3 EvaluateFromMatrix(float u, List<Vector3> inputPoints) => Vector3.zero;

    public virtual bool IsPointAKnot(int PointID) => false;
}
