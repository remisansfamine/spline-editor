using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SplineController : MonoBehaviour
{
    public List<Vector3> InputPoint = new List<Vector3>();

    [SerializeField] private SplineDescriptor SplineFormula = null;
    public Vector3 EvaluateFromPolynomial(float u)
    {
        return SplineFormula ? SplineFormula.EvaluateFromPolynomial(u, InputPoint) : Vector3.zero;
    }
}
