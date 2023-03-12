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

    public virtual Vector3 EvaluatePositionFromPolynomial(float u, List<Vector3> inputPoints) => Vector3.zero;
    public virtual Vector3 EvaluateTangentFromPolynomial(float u, List<Vector3> inputPoints) => Vector3.zero;
    public virtual Vector3 EvaluatePositionFromMatrix(float u, List<Vector3> inputPoints)
    {
        (float t, int startingPoint) = GetLocalParameters(u, inputPoints.Count);

        List<Vector3> intervallePoints = inputPoints.GetRange(startingPoint, 4);

        return GetGeometryMatrix(intervallePoints) * GetPositionCharacteristicMatrix() * GetTimeVector(t);
    }

    public virtual Vector3 EvaluateTangentFromMatrix(float u, List<Vector3> inputPoints)
    {
        (float t, int startingPoint) = GetLocalParameters(u, inputPoints.Count);

        List<Vector3> intervallePoints = inputPoints.GetRange(startingPoint, 4);

        return GetGeometryMatrix(intervallePoints) * GetTangentCharacteristicMatrix() * GetTimeVector(t);
    }

    public override Vector3 EvaluatePosition(float u, List<Vector3> inputPoints)
    {
        if (calculationMode == ECalculationMode.MATRIX)
            return EvaluatePositionFromMatrix(u, inputPoints);

        return EvaluatePositionFromPolynomial(u, inputPoints);
    }

    public override Vector3 EvaluateTangent(float u, List<Vector3> inputPoints)
    {
        if (calculationMode == ECalculationMode.MATRIX)
            return EvaluateTangentFromMatrix(u, inputPoints);

        return EvaluateTangentFromPolynomial(u, inputPoints);
    }
}
