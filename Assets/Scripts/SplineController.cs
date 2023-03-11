using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class SplineController : MonoBehaviour
{
    [SerializeField] private List<Vector3> inputPoints = new List<Vector3>();
    [SerializeField] private float continuityAccuracy = 0.01f;
    private float totalLength = 0f;
    [SerializeField] private List<float> cumulativeDistances = new List<float>();
    [SerializeField] private bool useContinuityApproximation = true;

    [SerializeField] private SplineDescriptor splineFormula = null;

    public SplineDescriptor SplineFormula => splineFormula;

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

            float oneOverLastID = 1f / lastID;

            float from = di * oneOverLastID;
            float to = from + oneOverLastID;

            return Utils.Remap(length, cumulativeDistances[di], cumulativeDistances[di + 1], from, to);
        }

        return 1f;
    }

    private float GetRemappedU(float u) => GetUFromLength(totalLength * u);

    public Vector3 EvaluateFromPolynomial(float u)
    {
        if (!splineFormula)
            return Vector3.zero;

        if (useContinuityApproximation)
            return splineFormula.EvaluateFromPolynomial(GetRemappedU(u), inputPoints);

        return splineFormula.EvaluateFromPolynomial(u, inputPoints);
    }

    public Vector3 EvaluateFromMatrix(float u)
    {
        if (!splineFormula)
            return Vector3.zero;

        if (useContinuityApproximation)
            return splineFormula.EvaluateFromMatrix(GetRemappedU(u), inputPoints);

        return splineFormula.EvaluateFromMatrix(u, inputPoints);
    }

    public bool IsPointAKnot(int pointID) => splineFormula ? splineFormula.IsPointAKnot(pointID) : false;

    private void ComputeDistances()
    {
        cumulativeDistances.Clear();

        if (!useContinuityApproximation || !splineFormula)
            return;

        Vector3 previousPoint = Vector3.zero;

        float distance = 0f;
        for (float quantity = 0f; quantity < 1f; quantity += continuityAccuracy)
        {
            Vector3 currentPoint = splineFormula.EvaluateFromMatrix(quantity, inputPoints);

            float currentDistance = 0f;

            if (quantity > 0f)
                currentDistance = Vector3.Distance(previousPoint, currentPoint);

            distance += currentDistance;

            cumulativeDistances.Add(distance);

            previousPoint = currentPoint;
        }
    }

    public void SetInputPoint(int pointID, Vector3 position)
    {
        if (!splineFormula)
            return;

        splineFormula.SetInputPoint(pointID, position, inputPoints);

        ComputeDistances();
    }

    public void InsertPoint(int pointID)
    {
        if (!splineFormula || !splineFormula.IsPointAKnot(pointID))
            return;

        splineFormula.InsertPoint(pointID, inputPoints);

        ComputeDistances();
    }

    public Vector3 GetInputPoint(int pointID) => inputPoints[pointID];
    public int GetInputPointCount() => inputPoints.Count;

    private void OnValidate()
    {
        ComputeDistances();
    }
}
