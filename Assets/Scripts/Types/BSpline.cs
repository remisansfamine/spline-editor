using System.Collections.Generic;
using System.Runtime.ExceptionServices;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "BSpline", menuName = "Splines/BSpline", order = 1)]
public class BSpline : SplineDescriptor
{
    private static readonly Matrix4x4 CharacteristicMatrix = new Matrix4x4(new Vector4(-1f, 3f,-3f, 1f) / 6f,
                                                                           new Vector4( 3f,-6f, 3f, 0f) / 6f,
                                                                           new Vector4(-3f, 0f, 3f, 0f) / 6f,
                                                                           new Vector4( 1f, 4f, 1f, 0f) / 6f);

    public override (float t, int startingPoint) GetLocalParameters(float u, int inputCount)
    {
        int knotCount = inputCount;
        float knotQuantity = u * (knotCount - 3);
        int startingKnot = Mathf.Clamp(Mathf.FloorToInt(knotQuantity), 0, knotCount - 2);

        int startingPoint = startingKnot;
        float t = knotQuantity - startingKnot;

        return (t, startingPoint);
    }

    public override bool IsPointAKnot(int PointID) => true;

    public Vector3 LocalEvaluateFromPolynomial(float t, List<Vector3> intervalPoints)
    {
        Vector3 PointA = intervalPoints[0];
        Vector3 PointB = intervalPoints[1];
        Vector3 PointC = intervalPoints[2];
        Vector3 PointD = intervalPoints[3];

        float tSqr = t * t;
        float tCube = tSqr * t;

        float a = Mathf.Pow(1f - t, 3f);
        float b = 3f * tCube - 6f * tSqr + 4f;
        float c = 3f * (-tCube + tSqr + t) + 1f;
        float d = tCube;

        return 1f / 6f * (a * PointA + b * PointB + c * PointC + d * PointD);
    }


    public override Vector3 EvaluateFromPolynomial(float u, List<Vector3> inputPoints)
    {
        (float t, int startingPoint) = GetLocalParameters(u, inputPoints.Count);

        List<Vector3> intervalPoints = inputPoints.GetRange(startingPoint, 4);

        return LocalEvaluateFromPolynomial(t, intervalPoints);
    }

    public override Vector4 GetTimeVector(float time)
    {
        float timeSqr = time * time;
        float timeCube = timeSqr * time;
        return new Vector4(timeCube, timeSqr, time, 1f);
    }
    public override Matrix4x4 GetCharacteristicMatrix() => CharacteristicMatrix;

    public override Matrix4x4 GetGeometryMatrix(List<Vector3> inputPoints)
    {
        Vector3 PointA = inputPoints[0];
        Vector3 PointB = inputPoints[1];
        Vector3 PointC = inputPoints[2];
        Vector3 PointD = inputPoints[3];

        return new Matrix4x4(new Vector4(PointA.x, PointA.y, PointA.z, 0f),
                             new Vector4(PointB.x, PointB.y, PointB.z, 0f),
                             new Vector4(PointC.x, PointC.y, PointC.z, 0f),
                             new Vector4(PointD.x, PointD.y, PointD.z, 1f));
    }
}
