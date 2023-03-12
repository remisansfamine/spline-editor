using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SplineController))]
public class SplineSceneEditor : Editor
{
    [SerializeField] private float positionDistance = 0.01f;

    private int? insertedPoint = null;

    private Color GetPointColor(SplineController controller, int pointID, bool isInserting)
    {
        if (!controller.IsPointAKnot(pointID))
            return Color.grey;

        if (isInserting)
            return Color.blue;

        return Color.yellow;
    }

    void ProcessInput(SplineController controller)
    {
        Event currEvent = Event.current;

        bool isInserting = currEvent.control;

        if (!isInserting)
            insertedPoint = null;

        for (int pointID = 0; pointID < controller.GetInputPointCount(); pointID++)
        {
            if (pointID == insertedPoint)
                continue;

            Handles.color = GetPointColor(controller, pointID, isInserting);

            Vector3 handlePosition = Handles.FreeMoveHandle(controller.GetInputPoint(pointID), Quaternion.identity, 1f, Vector3.zero, Handles.SphereHandleCap);

            if (!GUI.changed)
                continue;

            GUI.changed = false;

            if (insertedPoint.HasValue)
                controller.SetInputPoint(insertedPoint.Value, handlePosition);
            else
            {
                if (!isInserting)
                    controller.SetInputPoint(pointID, handlePosition);
                else
                {
                    controller.InsertPoint(pointID);
                    insertedPoint = pointID; 
                }
            }

            EditorUtility.SetDirty(target);
        }
    }

    private void DisplayLinks(SplineController controller)
    {
        Handles.color = Color.red;

        switch (controller.SplineFormula)
        {
            case HermitianSpline hermitian:
                for (int pointID = 0; pointID < controller.GetInputPointCount(); pointID += 2)
                {
                    Vector3 position = controller.GetInputPoint(pointID + 0);
                    Vector3 velocity = controller.GetInputPoint(pointID + 1);
                    Handles.DrawLine(position, velocity);
                }
                break;

            case BezierSpline bezier:
                for (int PointId = 0; PointId < controller.GetInputPointCount(); PointId += 3)
                {
                    Vector3 position = controller.GetInputPoint(PointId + 0);

                    if (PointId > 0)
                    {
                        Vector3 prevTangent = controller.GetInputPoint(PointId - 1);
                        Handles.DrawLine(position, prevTangent);
                    }

                    if (PointId < controller.GetInputPointCount() - 1)
                    {
                        Vector3 nextTangent = controller.GetInputPoint(PointId + 1);
                        Handles.DrawLine(position, nextTangent);
                    }
                }
                break;

            case BSpline bspline:
                for (int PointId = 0; PointId < controller.GetInputPointCount() - 1; PointId++)
                {
                    Vector3 positionA = controller.GetInputPoint(PointId + 0);
                    Vector3 positionB = controller.GetInputPoint(PointId + 1);
                    Handles.DrawLine(positionA, positionB);
                }
                break;
        }
    }

    private void Display(SplineController controller)
    {
        DisplayLinks(controller);

        Handles.color = Color.white;

        Vector3 startPoint = Vector3.zero;

        for (float quantity = 0f; quantity <= 1f; quantity += positionDistance)
        {
            Vector3 endPoint = controller.EvaluateFromMatrix(quantity);

            if (quantity > 0f)
                Handles.DrawLine(startPoint, endPoint);

            startPoint = endPoint;
        }
    }

    private void OnSceneGUI()
    {
        SplineController controller = target as SplineController;

        ProcessInput(controller);

        Display(controller);
    }
}
