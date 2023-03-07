using System.Collections.Generic;
using System.Runtime.ExceptionServices;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "BezierSpline", menuName = "Splines/BezierSpline", order = 1)]
public class BezierSpline : SplineDescriptor
{
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
}
