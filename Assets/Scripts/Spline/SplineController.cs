using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class SplineController : MonoBehaviour
{
    [SerializeField] private List<Vector3> inputPoints = new List<Vector3>();
    [SerializeField] private float continuityAccuracy = 0.01f;
    public float totalLength { get; private set; } = 0f;
    [SerializeField] private List<float> cumulativeDistances = new List<float>();
    [SerializeField] private bool useContinuityApproximation = true;

    [SerializeField] private SplineDescriptor splineFormula = null;

    public UnityEvent<bool> OnSplineUpdated = new UnityEvent<bool>();

    public SplineDescriptor SplineFormula => splineFormula;

    [SerializeField] private bool isTransformBounded = true;

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

    public Vector3 EvaluatePosition(float u)
    {
        if (!splineFormula)
            return Vector3.zero;

        float correctU = useContinuityApproximation ? GetRemappedU(u) : u;

        Vector3 pointLocalPosition = splineFormula.EvaluatePosition(correctU, inputPoints);

        if (!isTransformBounded)
            return pointLocalPosition;

        return transform.TransformPoint(pointLocalPosition);
    }

    public Vector3 EvaluateTangent(float u)
    {
        if (!splineFormula)
            return Vector3.zero;

        float correctU = useContinuityApproximation ? GetRemappedU(u) : u;

        Vector3 pointLocalPosition = splineFormula.EvaluateTangent(correctU, inputPoints);

        if (!isTransformBounded)
            return pointLocalPosition;

        return transform.TransformVector(pointLocalPosition);
    }

    public bool IsPointAKnot(int pointID) => splineFormula ? splineFormula.IsPointAKnot(pointID) : false;

    private void ComputeDistances()
    {
        cumulativeDistances.Clear();

        if (!splineFormula)
            return;

        Vector3 previousPoint = Vector3.zero;

        float distance = 0f;
        for (float quantity = 0f; quantity < 1f; quantity += continuityAccuracy)
        {
            Vector3 currentPoint = splineFormula.EvaluatePosition(quantity, inputPoints);

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

        Vector3 pointLocalPosition = position;

        if (isTransformBounded)
            pointLocalPosition = transform.InverseTransformPoint(pointLocalPosition);

        splineFormula.SetInputPoint(pointID, pointLocalPosition, inputPoints);

        SetDirty();
    }

    public int InsertPoint(int pointID)
    {
        if (!splineFormula || !splineFormula.IsPointAKnot(pointID))
            return -1;

        int insertedPointID = splineFormula.InsertPoint(pointID, inputPoints);

        SetDirty();

        return insertedPointID;
    }

    public Vector3 GetInputPoint(int pointID)
    {
        Vector3 pointLocalPosition = inputPoints[pointID];

        if (!isTransformBounded)
            return pointLocalPosition;

        return transform.TransformPoint(pointLocalPosition);
    }

    public int GetInputPointCount() => inputPoints.Count;

    private void OnValidate()
    {
        SetDirty(true);

    }

    private void SetDirty(bool calledOnValidate = false)
    {
        ComputeDistances();
        OnSplineUpdated.Invoke(calledOnValidate);
    }
}
