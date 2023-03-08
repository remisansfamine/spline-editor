using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SplineController : MonoBehaviour
{
    public List<Vector3> InputPoint = new List<Vector3>();

    public SplineDescriptor SplineFormula = null;

    public Vector3 EvaluateFromPolynomial(float u) => SplineFormula ? SplineFormula.EvaluateFromPolynomial(u, InputPoint) : Vector3.zero;

    public Vector3 EvaluateFromMatrix(float u) => SplineFormula ? SplineFormula.EvaluateFromMatrix(u, InputPoint) : Vector3.zero;

    public bool IsPointAKnot(int PointID) => SplineFormula ? SplineFormula.IsPointAKnot(PointID) : false;
}
