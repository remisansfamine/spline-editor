using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "HermitianSpline", menuName = "Splines/HermitianSpline", order = 1)]
public class HermitianSpline : SplineDescriptor
{
    private static readonly Matrix4x4 characteristicMatrix = new Matrix4x4(new Vector4( 2f,-2f, 1f, 1f),
                                                                           new Vector4(-3f, 3f,-2f,-1f),
                                                                           new Vector4( 0f, 0f, 1f, 0f),
                                                                           new Vector4( 1f, 0f, 0f, 0f));

    public override bool IsPointAKnot(int pointID) => pointID % 2 == 0;

    public override (float t, int startingPoint) GetLocalParameters(float u, int inputCount)
    {
        int knotCount = inputCount / 2;
        int validKnotCount = knotCount - 1;
        float knotQuantity = u * validKnotCount;
        int startingKnot = Mathf.Clamp(Mathf.FloorToInt(knotQuantity), 0, validKnotCount - 1);

        int startingPoint = startingKnot * 2;
        float t = knotQuantity - startingKnot;

        return (t, startingPoint);
    }

    public Vector3 LocalEvaluateFromPolynomial(float t, List<Vector3> intervalPoints)
    {
        Vector3 pointA = intervalPoints[0];
        Vector3 derivA = intervalPoints[1] - pointA;

        Vector3 pointB = intervalPoints[2];
        Vector3 derivB = intervalPoints[3] - pointB;

        float tSqr = t * t;
        float tCube = tSqr * t;

        float a = 2f * tCube - 3f * tSqr + 1f;
        float b = -2f * tCube + 3f * tSqr;
        float c = tCube - 2f * tSqr + t;
        float d = tCube - tSqr;

        return a * pointA + b * pointB + c * derivA + d * derivB;
    }

    public override Vector3 EvaluateFromPolynomial(float u, List<Vector3> inputPoints)
    {
        (float t, int startingPoint) = GetLocalParameters(u, inputPoints.Count);

        List<Vector3> intervalPoints = inputPoints.GetRange(startingPoint, 4);

        return LocalEvaluateFromPolynomial(t, intervalPoints);
    }

    public override Vector4 GetTimeVector(float t)
    {
        float tSqr = t * t;
        float tCube = tSqr * t;

        return new Vector4(tCube, tSqr, t, 1f);
    }
    public override Matrix4x4 GetCharacteristicMatrix() => characteristicMatrix;

    public override Matrix4x4 GetGeometryMatrix(List<Vector3> inputPoints)
    {
        Vector3 pointA = inputPoints[0];
        Vector3 derivA = inputPoints[1] - pointA;

        Vector3 pointB = inputPoints[2];
        Vector3 derivB = inputPoints[3] - pointB;

        return new Matrix4x4(new Vector4(pointA.x, pointA.y, pointA.z, 0f), 
                             new Vector4(pointB.x, pointB.y, pointB.z, 0f),
                             new Vector4(derivA.x, derivA.y, derivA.z, 0f),
                             new Vector4(derivB.x, derivB.y, derivB.z, 1f));
    }

    public void MoveVelocityAlong(int knotID, Vector3 position, List<Vector3> inputPoints)
    {
        Vector3 lastPosition = inputPoints[knotID];

        Vector3 velocity = inputPoints[knotID + 1];

        inputPoints[knotID + 1] = position + (velocity - lastPosition);
    }

    public override void SetInputPoint(int pointID, Vector3 position, List<Vector3> inputPoints)
    {
        if (IsPointAKnot(pointID))
            MoveVelocityAlong(pointID, position, inputPoints);

        base.SetInputPoint(pointID, position, inputPoints);
    }

    public override int InsertPoint(int pointID, List<Vector3> inputPoints)
    {
        List<Vector3> newPoints = new List<Vector3>() { inputPoints[pointID], inputPoints[pointID + 1] };

        inputPoints.InsertRange(pointID, newPoints);

        base.InsertPoint(pointID, inputPoints);

        return pointID;
    }
}
