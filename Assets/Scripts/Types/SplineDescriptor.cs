using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SplineDescriptor : ScriptableObject
{
    public virtual (float t, int startingPoint) GetLocalParameters(float u, int inputCount) => (0f, 0);

    public virtual Vector4 GetTimeVector(float time) => throw new NotImplementedException();
    public virtual Matrix4x4 GetCharacteristicMatrix() => throw new NotImplementedException();
    public virtual Matrix4x4 GetGeometryMatrix(List<Vector3> inputPoints) => throw new NotImplementedException();

    public virtual Vector3 EvaluateFromPolynomial(float u, List<Vector3> inputPoints) => Vector3.zero;
    public virtual Vector3 EvaluateFromMatrix(float u, List<Vector3> inputPoints)
    {
        (float t, int startingPoint) = GetLocalParameters(u, inputPoints.Count);

        List<Vector3> intervallePoints = inputPoints.GetRange(startingPoint, 4);

        return GetGeometryMatrix(intervallePoints) * GetCharacteristicMatrix() * GetTimeVector(t);
    }

    public virtual bool IsPointAKnot(int pointID) => false;

    public virtual void SetInputPoint(int pointID, Vector3 position, List<Vector3> inputPoints) => inputPoints[pointID] = position;
    public virtual void InsertPoint(int pointID, List<Vector3> inputPoints) { }
}
