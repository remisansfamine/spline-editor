
using System.Collections.Generic;
using System.Runtime.ExceptionServices;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "CatmullRomSpline", menuName = "Splines/CatmullRomSpline", order = 1)]
public class CatmullRomSpline : MultiModeSpline
{
    private static readonly Matrix4x4 positionCharacteristicMatrix = new Matrix4x4(new Vector4(-1f, 3f,-3f, 1f) * 0.5f,
                                                                                   new Vector4( 2f,-5f, 4f,-1f) * 0.5f,
                                                                                   new Vector4(-1f, 0f, 1f, 0f) * 0.5f,
                                                                                   new Vector4( 0f, 2f, 0f, 0f) * 0.5f);

    private static readonly Matrix4x4 tangentCharacteristicMatrix = new Matrix4x4(new Vector4( 0f,  0f, 0f, 0f) * 0.5f,
                                                                                  new Vector4(-3f,  9f,-9f, 3f) * 0.5f,
                                                                                  new Vector4( 4f,-10f, 8f,-2f) * 0.5f,
                                                                                  new Vector4(-1f,  0f, 1f, 0f) * 0.5f);

    public override (float t, int startingPoint) GetLocalParameters(float u, int inputCount)
    {
        int knotCount = inputCount;
        int validKnotCount = knotCount - 3;
        float knotQuantity = u * validKnotCount;
        int startingKnot = Mathf.Clamp(Mathf.FloorToInt(knotQuantity), 0, validKnotCount - 1);

        int startingPoint = startingKnot;
        float t = knotQuantity - startingKnot;

        return (t, startingPoint);
    }

    public override bool IsPointAKnot(int pointID) => true;

    public Vector3 LocalEvaluatePositionFromPolynomial(float t, List<Vector3> intervalPoints)
    {
        Vector3 pointA = intervalPoints[0];
        Vector3 pointB = intervalPoints[1];
        Vector3 pointC = intervalPoints[2];
        Vector3 pointD = intervalPoints[3];

        float tSqr = t * t;
        float tCube = tSqr * t;

        float a = -tCube + 2f * tSqr - t;
        float b = 3f * tCube - 5f * tSqr + 2f;
        float c = -3f * tCube + 4f * tSqr + t;
        float d = tCube - tSqr;

        return 0.5f * (a * pointA + b * pointB + c * pointC + d * pointD);
    }

    public Vector3 LocalEvaluateTangentFromPolynomial(float t, List<Vector3> intervalPoints)
    {
        Vector3 pointA = intervalPoints[0];
        Vector3 pointB = intervalPoints[1];
        Vector3 pointC = intervalPoints[2];
        Vector3 pointD = intervalPoints[3];

        float tSqr = t * t;

        float a = -3f * tSqr + 4f * t - 1;
        float b = 9f * tSqr - 10f * t;
        float c = -9f * tSqr + 8f * t + 1;
        float d = 3f * tSqr - 2f * t;

        return 0.5f * (a * pointA + b * pointB + c * pointC + d * pointD);
    }

    public override Vector3 EvaluatePositionFromPolynomial(float u, List<Vector3> inputPoints)
    {
        (float t, int startingPoint) = GetLocalParameters(u, inputPoints.Count);

        List<Vector3> intervalPoints = inputPoints.GetRange(startingPoint, 4);

        return LocalEvaluatePositionFromPolynomial(t, intervalPoints);
    }

    public override Vector3 EvaluateTangentFromPolynomial(float u, List<Vector3> inputPoints)
    {
        (float t, int startingPoint) = GetLocalParameters(u, inputPoints.Count);

        List<Vector3> intervalPoints = inputPoints.GetRange(startingPoint, 4);

        return LocalEvaluateTangentFromPolynomial(t, intervalPoints);
    }

    public override Vector4 GetTimeVector(float time)
    {
        float timeSqr = time * time;
        float timeCube = timeSqr * time;
        return new Vector4(timeCube, timeSqr, time, 1f);
    }
    public override Matrix4x4 GetPositionCharacteristicMatrix() => positionCharacteristicMatrix;
    public override Matrix4x4 GetTangentCharacteristicMatrix() => tangentCharacteristicMatrix;


    public override Matrix4x4 GetGeometryMatrix(List<Vector3> inputPoints)
    {
        Vector3 pointA = inputPoints[0];
        Vector3 pointB = inputPoints[1];
        Vector3 pointC = inputPoints[2];
        Vector3 pointD = inputPoints[3];

        return new Matrix4x4(new Vector4(pointA.x, pointA.y, pointA.z, 0f),
                             new Vector4(pointB.x, pointB.y, pointB.z, 0f),
                             new Vector4(pointC.x, pointC.y, pointC.z, 0f),
                             new Vector4(pointD.x, pointD.y, pointD.z, 1f));
    }

    public override int InsertPoint(int pointID, List<Vector3> inputPoints)
    {
        inputPoints.Insert(pointID, inputPoints[pointID]);

        base.InsertPoint(pointID, inputPoints);

        return pointID;
    }
}
