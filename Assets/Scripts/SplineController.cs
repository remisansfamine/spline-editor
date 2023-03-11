using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SplineController : MonoBehaviour
{
    [SerializeField] private List<Vector3> inputPoints = new List<Vector3>();
    [SerializeField] private float continuityAccuracy = 0.01f;
    private float totalLength = 0f;
    [SerializeField] private List<float> cumulativeDistances = new List<float>();
    [SerializeField] private bool useContinuityApproximation = true;

    public SplineDescriptor SplineFormula = null;

    private float GetUFromLength(float length)
    {
        if (length < 0f)
            return 0f;

        totalLength = cumulativeDistances.Last();

        int lastID = cumulativeDistances.Count - 1;
        for (int di = 0; di < lastID; di++)
        {
            if (length > cumulativeDistances[di])
                continue;

            float OneOverLastID = 1f / lastID;

            float from = di * OneOverLastID;
            float to = from + OneOverLastID;

            return Utils.Remap(length, cumulativeDistances[di], cumulativeDistances[di + 1], from, to);
        }

        return 1f;
    }

    private float GetRemappedU(float u) => GetUFromLength(totalLength * u);

    public Vector3 EvaluateFromPolynomial(float u)
    {
        if (!SplineFormula)
            return Vector3.zero;

        if (useContinuityApproximation)
            return SplineFormula.EvaluateFromPolynomial(GetRemappedU(u), inputPoints);

        return SplineFormula.EvaluateFromPolynomial(u, inputPoints);
    }

    public Vector3 EvaluateFromMatrix(float u)
    {
        if (!SplineFormula)
            return Vector3.zero;

        if (useContinuityApproximation)
            return SplineFormula.EvaluateFromMatrix(GetRemappedU(u), inputPoints);

        return SplineFormula.EvaluateFromMatrix(u, inputPoints);
    }

    public bool IsPointAKnot(int PointID) => SplineFormula ? SplineFormula.IsPointAKnot(PointID) : false;

    private void ComputeDistances()
    {
        cumulativeDistances.Clear();

        if (!useContinuityApproximation || !SplineFormula)
            return;

        Vector3 previousPoint = Vector3.zero;

        float distance = 0f;
        for (float quantity = 0f; quantity < 1f; quantity += continuityAccuracy)
        {
            Vector3 currentPoint = SplineFormula.EvaluateFromMatrix(quantity, inputPoints);

            float currentDistance = 0f;

            if (quantity > 0f)
                currentDistance = Vector3.Distance(previousPoint, currentPoint);

            distance += currentDistance;

            cumulativeDistances.Add(distance);

            previousPoint = currentPoint;
        }
    }

    public virtual void SetInputPoint(int PointID, Vector3 Position)
    {
        if (!SplineFormula)
            return;

        SplineFormula.SetInputPoint(PointID, Position, inputPoints);

        ComputeDistances();
    }

    public virtual Vector3 GetInputPoint(int PointID) => inputPoints[PointID];
    public virtual int GetInputPointCount() => inputPoints.Count;
}
