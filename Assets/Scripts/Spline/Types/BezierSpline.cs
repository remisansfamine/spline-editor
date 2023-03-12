using System.Collections.Generic;
using System.Runtime.ExceptionServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

[CreateAssetMenu(fileName = "BezierSpline", menuName = "Splines/BezierSpline", order = 1)]
public class BezierSpline : MultiModeSpline
{
    enum ETangentType
    {
        SEPARATED,
        COLLINEAR,
        MIRRORED,
        NONE
    }

    [SerializeField] private ETangentType tangentType = ETangentType.COLLINEAR;

    [SerializeField] private int controlPointsCount = 4;

    private static readonly Matrix4x4 characteristicMatrix = new Matrix4x4(new Vector4(-1f, 3f,-3f, 1f),
                                                                           new Vector4( 3f,-6f, 3f, 0f),
                                                                           new Vector4(-3f, 3f, 0f, 0f),
                                                                           new Vector4( 1f, 0f, 0f, 0f));

    public override (float t, int startingPoint) GetLocalParameters(float u, int inputCount)
    {
        int knotCount = (inputCount + 2) / 3;
        int validKnotCount = knotCount - 1;
        float knotQuantity = u * validKnotCount;
        int startingKnot = Mathf.Clamp(Mathf.FloorToInt(knotQuantity), 0, validKnotCount - 1);

        int startingPoint = startingKnot * 3;
        float t = knotQuantity - startingKnot;

        return (t, startingPoint);
    }

    public override bool IsPointAKnot(int pointID) => (pointID) % (controlPointsCount - 1) == 0;

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
        (float t, int startingPoint) = GetLocalParameters(u, inputPoints.Count);

        List<Vector3> intervalPoints = inputPoints.GetRange(startingPoint, controlPointsCount);

        return LocalEvaluateFromPolynomial(t, intervalPoints);
    }

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
    public void MoveTangentAlong(int knotID, Vector3 position, List<Vector3> inputPoints)
    {
        Vector3 lastPosition = inputPoints[knotID];

        if (knotID > 0)
        {
            Vector3 prevTangent = inputPoints[knotID - 1];

            inputPoints[knotID - 1] = position + (prevTangent - lastPosition);
        }

        if (knotID < inputPoints.Count - 1)
        {
            Vector3 nextTangent = inputPoints[knotID + 1];

            inputPoints[knotID + 1] = position + (nextTangent - lastPosition);
        }
    }

    private Vector3 GetMirrorTangentPosition(Vector3 knotPosition, Vector3 currentTangentPosition) => 2f * knotPosition - currentTangentPosition;

    private Vector3 GetCollinearTangentPosition(Vector3 knotPosition, Vector3 currentTangentPosition, Vector3 lastTwinTangentPosition) => knotPosition + Vector3.Normalize(knotPosition - currentTangentPosition) * Vector3.Distance(knotPosition, lastTwinTangentPosition);

    private Vector3 GetTwinTangentPosition(ETangentType modifierType, Vector3 knotPosition, Vector3 currentTangentPosition, Vector3 lastTwinTangentPosition)
    {
        return modifierType == ETangentType.MIRRORED ? GetMirrorTangentPosition(knotPosition, currentTangentPosition) : GetCollinearTangentPosition(knotPosition, currentTangentPosition, lastTwinTangentPosition);
    }

    public void SetTangentTwin(int pointID, Vector3 position, List<Vector3> inputPoints)
    {
        // Give the sign to the nearest knot
        int sign = 2 * (pointID % (controlPointsCount - 1)) - 3;

        int knotID = pointID + sign;

        if (knotID < 0 || knotID > inputPoints.Count - 1)
            return;

        Vector3 knotPosition = inputPoints[knotID];

        int otherTangentID = pointID + sign * 2;

        if (otherTangentID < 0 || otherTangentID > inputPoints.Count - 1)
            return;

        inputPoints[otherTangentID] = GetTwinTangentPosition(tangentType, knotPosition, position, inputPoints[otherTangentID]);
    }

    public override void SetInputPoint(int pointID, Vector3 position, List<Vector3> inputPoints)
    {
        if (IsPointAKnot(pointID))
        {
            MoveTangentAlong(pointID, position, inputPoints);
            base.SetInputPoint(pointID, position, inputPoints);
            return;
        }

        base.SetInputPoint(pointID, position, inputPoints);

        if (tangentType != ETangentType.SEPARATED)
            SetTangentTwin(pointID, position, inputPoints);
    }

    public override int InsertPoint(int pointID, List<Vector3> inputPoints)
    {
        List<Vector3> newPoints = new List<Vector3>();

        int offset = 0;
        
        if (pointID == 0)
        {
            offset++;
            newPoints.Add(inputPoints[pointID - 1]);
        }

        newPoints.Add(inputPoints[pointID]);

        if (pointID < inputPoints.Count - 1)
        {
            offset++;
            newPoints.Add(inputPoints[pointID + 1]);
        }

        inputPoints.InsertRange(pointID + offset, newPoints);

        base.InsertPoint(pointID, inputPoints);

        return pointID + offset;
    }
}
