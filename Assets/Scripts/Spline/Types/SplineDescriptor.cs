using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SplineDescriptor : ScriptableObject
{
    public virtual (float t, int startingPoint) GetLocalParameters(float u, int inputCount) => (0f, 0);

    public virtual Vector4 GetTimeVector(float time) => throw new NotImplementedException();
    public virtual Matrix4x4 GetPositionCharacteristicMatrix() => throw new NotImplementedException();
    public virtual Matrix4x4 GetTangentCharacteristicMatrix() => throw new NotImplementedException();
    public virtual Matrix4x4 GetGeometryMatrix(List<Vector3> inputPoints) => throw new NotImplementedException();

    public abstract Vector3 EvaluatePosition(float u, List<Vector3> inputPoints);
    public abstract Vector3 EvaluateTangent(float u, List<Vector3> inputPoints);

    public virtual bool IsPointAKnot(int pointID) => false;

    public virtual void SetInputPoint(int pointID, Vector3 position, List<Vector3> inputPoints) => inputPoints[pointID] = position;
    public virtual int InsertPoint(int pointID, List<Vector3> inputPoints) => -1;
}
