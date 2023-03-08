using System.Collections.Generic;
using System.Runtime.ExceptionServices;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "BezierSpline", menuName = "Splines/BezierSpline", order = 1)]
public class BezierSpline : SplineDescriptor
{
    [SerializeField] private int controlPointsCount = 4;

    private static readonly Matrix4x4 CharacteristicMatrix = new Matrix4x4(new Vector4(-1f, 3f,-3f, 1f),
                                                                           new Vector4( 3f,-6f, 3f, 0f),
                                                                           new Vector4(-3f, 3f, 0f, 0f),
                                                                           new Vector4( 1f, 0f, 0f, 0f));

    public override void GetLocalParameters(float u, int inputCount, out float t, out int startingPoint)
    {
        int knotCount = (inputCount + 2) / 3;
        float knotQuantity = u * (knotCount - 1);
        int startingKnot = Mathf.FloorToInt(knotQuantity);

        startingPoint = startingKnot * 3;

        t = knotQuantity - startingKnot;
    }

    public override bool IsPointAKnot(int PointID) => (PointID) % (controlPointsCount - 1) == 0;

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

    public Vector3 LocalEvaluateFromPolynomial(float t, List<Vector3> intervalPoints)
    {
        Vector3 Result = Vector3.zero;

        int nPlusOne = intervalPoints.Count;
        int n = nPlusOne - 1;

        for (int i = 0; i < nPlusOne; i++)
        {
            float BernsteinPolynomial = Bernstein(n, i, t);
            Result += BernsteinPolynomial * intervalPoints[i];
        }

        return Result;
    }

    public override Vector3 EvaluateFromPolynomial(float u, List<Vector3> inputPoints)
    {
        GetLocalParameters(u, inputPoints.Count, out float t, out int startingPoint);

        List<Vector3> intervalPoints = inputPoints.GetRange(startingPoint, controlPointsCount);

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
