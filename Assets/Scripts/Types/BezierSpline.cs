using System.Collections.Generic;
using System.Runtime.ExceptionServices;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "BezierSpline", menuName = "Splines/BezierSpline", order = 1)]
public class BezierSpline : SplineDescriptor
{
    Matrix4x4 M = new Matrix4x4(new Vector4(-1f, 3f,-3f, 1f),
                                new Vector4( 3f,-6f, 3f, 0f),
                                new Vector4(-3f, 3f, 0f, 0f),
                                new Vector4( 1f, 0f, 0f, 0f));

    public override void GetT(float u, int inputCount, out float t, out int startingPoint)
    {
        int knotCount = inputCount / 2;
        float knotQuantity = u * (knotCount - 1);
        int startingKnot = Mathf.FloorToInt(knotQuantity);

        startingPoint = startingKnot * 2;

        t = knotQuantity - startingKnot;
    }

    public override bool IsPointAKnot(int PointID) => true;

    private static float Factorial(int input)
    {
        int fac = 1;

        for (int i = 1; i <= input; i++)
            fac *= i;

        return fac;
    }

    private static float Binomial(int n, int i)
    {
        float facN = Factorial(n);
        float facI = Factorial(i);
        float facNMinusI = Factorial(n - i);

        return facN / (facI * facNMinusI);
    }

    private static float Bernstein(int n, int i, float t)
    {
        float tPowi = Mathf.Pow(t, i);
        float tnMinusi = Mathf.Pow(1 - t, n - i);

        return Binomial(n, i) * tPowi * tnMinusi;
    }

    public override Vector3 EvaluateFromPolynomial(float u, List<Vector3> inputPoints)
    {
        GetT(u, inputPoints.Count, out float t, out int startingPoint);

        Vector3 Result = Vector3.zero;

        for (int i = 0; i < 4; i++)
        {
            float BernsteinPolynomial = Bernstein(4, i, t);
            Result += BernsteinPolynomial * inputPoints[startingPoint + i];
        }

        return Result;
    }

    public override Vector3 EvaluateFromMatrix(float u, List<Vector3> inputPoints)
    {
        GetT(u, inputPoints.Count, out float t, out int startingPoint);

        List<Vector3> intervallePoints = inputPoints.GetRange(startingPoint, 4);

        return GetGeometryMatrix(intervallePoints) * M * GetTimeVector(t);
    }

    public Vector4 GetTimeVector(float time)
    {
        float timeSqr = time * time;
        float timeCube = timeSqr * time;
        return new Vector4(timeCube, timeSqr, time, 1f);
    }

    public Matrix4x4 GetGeometryMatrix(List<Vector3> inputPoints)
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
