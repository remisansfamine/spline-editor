using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SplineController)), ExecuteInEditMode]
public class PillarGenerator : MonoBehaviour
{
    [SerializeField] private GameObject pillarPrefab = null;
    [SerializeField] private float precision = 0.01f;
    [SerializeField] private float maxCastDistance = 1f;

    private SplineController splineController = null;

    void Awake()
    {
        splineController = GetComponent<SplineController>();
    }

    private void OnEnable()
    {
        splineController.OnSplineUpdated.AddListener(ImmediatePillarsUpdate);
        UnimmediatePillarsUpdate();
    }

    private void OnDisable()
    {
        ResetPillars(false);
        splineController.OnSplineUpdated.RemoveListener(ImmediatePillarsUpdate);
    }

    void ResetPillars(bool Immediate)
    {
        if (Immediate)
        {
            foreach (Transform child in transform)
                DestroyImmediate(child.gameObject);

            return;
        }

        foreach (Transform child in transform)
            Destroy(child.gameObject);
    }

    void UnimmediatePillarsUpdate()
    {
        ResetPillars(false);
        CreatePillars();
    }

    void ImmediatePillarsUpdate()
    {
        ResetPillars(true);
        CreatePillars();
    }

    void CreatePillars()
    {
        for (float quantity = 0f; quantity <= 1f; quantity += precision)
        {
            Vector3 stepPoint = splineController.EvaluateFromMatrix(quantity);

            if (!Physics.Raycast(stepPoint, Vector3.down, out RaycastHit hit, maxCastDistance))
                continue;

            if (transform.IsChildOf(hit.collider.transform))
                continue;

            Vector3 averagePoint = 0.5f * (stepPoint + hit.point);

            GameObject pillar = Instantiate(pillarPrefab, averagePoint, Quaternion.identity, transform);

            float pillarHeight = Vector3.Distance(stepPoint, hit.point) * 0.5f;

            pillar.transform.localScale = new Vector3(pillar.transform.localScale.x, pillarHeight, pillar.transform.localScale.z);
        }
    }
}
