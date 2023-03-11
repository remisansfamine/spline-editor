using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SplineController))]
public class SplineSceneEditor : Editor
{
    [SerializeField] private float positionDistance = 0.01f;

    private bool canInsert = true;

    private Color GetPointColor(SplineController controller, int PointID, bool isInserting)
    {
        if (!controller.IsPointAKnot(PointID))
            return Color.grey;

        if (isInserting)
            return Color.blue;

        return Color.yellow;
    }

    void ProcessInput(SplineController controller)
    {
        bool isInserting = Event.current.control;

        if (!isInserting)
            canInsert = true;

        for (int PointID = 0; PointID < controller.GetInputPointCount(); PointID++)
        {
            Handles.color = GetPointColor(controller, PointID, isInserting);

            Vector3 HandlePosition = Handles.FreeMoveHandle(controller.GetInputPoint(PointID), Quaternion.identity, 1f, Vector3.zero, Handles.SphereHandleCap);

            if (!GUI.changed)
                continue;

            GUI.changed = false;

            if (!isInserting)
            {
                controller.SetInputPoint(PointID, HandlePosition); 
            }
            else if (canInsert)
            {
                canInsert = false;
                controller.InsertPoint(PointID);
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
                for (int PointId = 0; PointId < controller.GetInputPointCount(); PointId += 2)
                {
                    Vector3 Position = controller.GetInputPoint(PointId + 0);
                    Vector3 Velocity = controller.GetInputPoint(PointId + 1);
                    Handles.DrawLine(Position, Velocity);
                }
                break;

            case BezierSpline bezier:
                for (int PointId = 0; PointId < controller.GetInputPointCount(); PointId += 3)
                {
                    Vector3 Position = controller.GetInputPoint(PointId + 0);

                    if (PointId > 0)
                    {
                        Vector3 PrevVelocity = controller.GetInputPoint(PointId - 1);
                        Handles.DrawLine(Position, PrevVelocity);
                    }

                    if (PointId < controller.GetInputPointCount() - 1)
                    {
                        Vector3 NextVelocity = controller.GetInputPoint(PointId + 1);
                        Handles.DrawLine(Position, NextVelocity);
                    }
                }
                break;

            case BSpline bspline:
                for (int PointId = 0; PointId < controller.GetInputPointCount() - 1; PointId++)
                {
                    Vector3 PositionA = controller.GetInputPoint(PointId + 0);
                    Vector3 PositionB = controller.GetInputPoint(PointId + 1);
                    Handles.DrawLine(PositionA, PositionB);
                }
                break;
        }
    }

    private void Display(SplineController controller)
    {
        DisplayLinks(controller);

        Handles.color = Color.white;

        Vector3 StartPoint = Vector3.zero;

        for (float quantity = 0f; quantity <= 1f; quantity += positionDistance)
        {
            Vector3 EndPoint = controller.EvaluateFromMatrix(quantity);

            if (quantity > 0f)
                Handles.DrawLine(StartPoint, EndPoint);

            StartPoint = EndPoint;
        }
    }

    private void OnSceneGUI()
    {
        SplineController controller = target as SplineController;

        ProcessInput(controller);

        Display(controller);
    }
}
