using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SplineController : MonoBehaviour
{
    [SerializeField] private List<Vector3> inputPoint = new List<Vector3>();

    public SplineDescriptor SplineFormula = null;

    public Vector3 EvaluateFromPolynomial(float u) => SplineFormula ? SplineFormula.EvaluateFromPolynomial(u, inputPoint) : Vector3.zero;

    public Vector3 EvaluateFromMatrix(float u) => SplineFormula ? SplineFormula.EvaluateFromMatrix(u, inputPoint) : Vector3.zero;

    public bool IsPointAKnot(int PointID) => SplineFormula ? SplineFormula.IsPointAKnot(PointID) : false;

    public virtual void SetInputPoint(int PointID, Vector3 Position) => SplineFormula?.SetInputPoint(PointID, Position, inputPoint);
    public virtual Vector3 GetInputPoint(int PointID) => inputPoint[PointID];
    public virtual int GetInputPointCount() => inputPoint.Count;
}
