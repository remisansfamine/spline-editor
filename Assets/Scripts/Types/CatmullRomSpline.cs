
using System.Collections.Generic;
using System.Runtime.ExceptionServices;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "CatmullRomSpline", menuName = "Splines/CatmullRomSpline", order = 1)]
public class CatmullRomSpline : SplineDescriptor
{
    private static readonly Matrix4x4 characteristicMatrix = new Matrix4x4(new Vector4(-1f, 3f,-3f, 1f) * 0.5f,
                                                                           new Vector4( 2f,-5f, 4f,-1f) * 0.5f,
                                                                           new Vector4(-1f, 0f, 1f, 0f) * 0.5f,
                                                                           new Vector4( 0f, 2f, 0f, 0f) * 0.5f);

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

    public override Vector4 GetTimeVector(float time)
    {
        float timeSqr = time * time;
        float timeCube = timeSqr * time;
        return new Vector4(timeCube, timeSqr, time, 1f);
    }
    public override Matrix4x4 GetCharacteristicMatrix() => characteristicMatrix;

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
