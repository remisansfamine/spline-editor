using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "HermitianSpline", menuName = "Splines/HermitianSpline", order = 1)]
public class HermitianSpline : SplineDescriptor
{
    private static readonly Matrix4x4 CharacteristicMatrix = new Matrix4x4(new Vector4( 2f,-2f, 1f, 1f),
                                                                           new Vector4(-3f, 3f,-2f,-1f),
                                                                           new Vector4( 0f, 0f, 1f, 0f),
                                                                           new Vector4( 1f, 0f, 0f, 0f));

    public override bool IsPointAKnot(int PointID) => PointID % 2 == 0;

    public override void GetLocalParameters(float u, int inputCount, out float t, out int startingPoint)
    {
        int knotCount = inputCount / 2;
        float knotQuantity = u * (knotCount - 1);
        int startingKnot = Mathf.FloorToInt(knotQuantity);

        startingPoint = startingKnot * 2;

        t = knotQuantity - startingKnot;
    }

    public Vector3 LocalEvaluateFromPolynomial(float t, List<Vector3> intervalPoints)
    {
        Vector3 PointA = intervalPoints[0];
        Vector3 DerivA = intervalPoints[1] - PointA;

        Vector3 PointB = intervalPoints[2];
        Vector3 DerivB = intervalPoints[3] - PointB;

        float tSqr = t * t;
        float tCube = tSqr * t;

        float a = 2f * tCube - 3f * tSqr + 1f;
        float b = -2f * tCube + 3f * tSqr;
        float c = tCube - 2f * tSqr + t;
        float d = tCube - tSqr;

        return a * PointA + b * PointB + c * DerivA + d * DerivB;
    }

    public override Vector3 EvaluateFromPolynomial(float u, List<Vector3> inputPoints)
    {
        GetLocalParameters(u, inputPoints.Count, out float t, out int startingPoint);

        List<Vector3> intervalPoints = inputPoints.GetRange(startingPoint, 4);

        return LocalEvaluateFromPolynomial(t, intervalPoints);
    }

    public override Vector4 GetTimeVector(float t)
    {
        float tSqr = t * t;
        float tCube = tSqr * t;

        return new Vector4(tCube, tSqr, t, 1f);
    }
    public override Matrix4x4 GetCharacteristicMatrix() => CharacteristicMatrix;

    public override Matrix4x4 GetGeometryMatrix(List<Vector3> inputPoints)
    {
        Vector3 PointA = inputPoints[0];
        Vector3 DerivA = inputPoints[1] - PointA;

        Vector3 PointB = inputPoints[2];
        Vector3 DerivB = inputPoints[3] - PointB;

        return new Matrix4x4(new Vector4(PointA.x, PointA.y, PointA.z, 0f), 
                             new Vector4(PointB.x, PointB.y, PointB.z, 0f),
                             new Vector4(DerivA.x, DerivA.y, DerivA.z, 0f),
                             new Vector4(DerivB.x, DerivB.y, DerivB.z, 1f));
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
}
