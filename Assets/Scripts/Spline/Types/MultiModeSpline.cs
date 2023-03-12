using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultiModeSpline : SplineDescriptor
{
    enum ECalculationMode
    {
        POLYNOMIAL,
        MATRIX,
    }

    [SerializeField] private ECalculationMode calculationMode = ECalculationMode.MATRIX;

    public virtual Vector3 EvaluateFromPolynomial(float u, List<Vector3> inputPoints) => Vector3.zero;
    public virtual Vector3 EvaluateFromMatrix(float u, List<Vector3> inputPoints)
    {
        (float t, int startingPoint) = GetLocalParameters(u, inputPoints.Count);

        List<Vector3> intervallePoints = inputPoints.GetRange(startingPoint, 4);

        return GetGeometryMatrix(intervallePoints) * GetCharacteristicMatrix() * GetTimeVector(t);
    }

    public override Vector3 EvaluatePosition(float u, List<Vector3> inputPoints)
    {
        if (calculationMode == ECalculationMode.MATRIX)
            return EvaluateFromMatrix(u, inputPoints);

        return EvaluateFromPolynomial(u, inputPoints);
    }
}
