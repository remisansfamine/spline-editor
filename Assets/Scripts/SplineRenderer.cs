using System;
using System.ComponentModel;
using UnityEditor;
using UnityEngine;

[RequireComponent(typeof(LineRenderer), typeof(SplineController))]
public class SplineRenderer : MonoBehaviour
{
    private LineRenderer lineRdr = null;
    private SplineController controller = null;

    [SerializeField] private float positionDistance = 10f;
    private float step = 0f;

    private void Awake()
    {
        lineRdr = GetComponent<LineRenderer>();
        controller = GetComponent<SplineController>();
    }

    // Start is called before the first frame update
    void Update()
    {
        step = 1f / positionDistance;
        lineRdr.positionCount = Mathf.RoundToInt(step);

        for (float quantity = 0f; quantity < 1f; quantity += positionDistance)
        {
            int PointId = (int)(lineRdr.positionCount * quantity);

            Vector3 LinePoint = controller.EvaluateFromPolynomial(quantity);

            lineRdr.SetPosition(PointId, LinePoint);
        }
    }
}
