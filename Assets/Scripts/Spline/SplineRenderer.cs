using System;
using System.ComponentModel;
using UnityEditor;
using UnityEngine;

[RequireComponent(typeof(LineRenderer), typeof(SplineController)), ExecuteInEditMode]
public class SplineRenderer : MonoBehaviour
{
    private LineRenderer lineRdr = null;
    private SplineController controller = null;

    [SerializeField] private float precision = 0.01f;

    private void Awake()
    {
        lineRdr = GetComponent<LineRenderer>();
        controller = GetComponent<SplineController>();
    }

    private void OnEnable()
    {
        controller.OnSplineUpdated.AddListener(UpdateLineRenderer);
        UpdateLineRenderer(false);
    }

    private void OnDisable()
    {
        controller.OnSplineUpdated.RemoveListener(UpdateLineRenderer);
    }

    void UpdateLineRenderer(bool calledOnValidate)
    {
        float step = 1f / precision;
        lineRdr.positionCount = Mathf.RoundToInt(step) + 1;

        int pointID = 0;
        for (float quantity = 0f; quantity <= 1f; quantity += precision)
        {
            Vector3 LinePoint = controller.EvaluatePosition(quantity);

            lineRdr.SetPosition(pointID, LinePoint);
            pointID++;
        }
    }
}
