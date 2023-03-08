using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SplineController))]
public class SplineSceneEditor : Editor
{
    [SerializeField] private float positionDistance = 0.001f;

    void Input(SplineController controller)
    {
        for (int PointId = 0; PointId < controller.GetInputPointCount(); PointId++)
        {
            Handles.color = controller.IsPointAKnot(PointId) ? Color.yellow : Color.grey;

            Vector3 HandlePosition = Handles.FreeMoveHandle(controller.GetInputPoint(PointId), Quaternion.identity, 1f, Vector3.zero, Handles.SphereHandleCap);

            if (!GUI.changed)
                continue;

            GUI.changed = false;
            controller.SetInputPoint(PointId, HandlePosition); 
            EditorUtility.SetDirty(target);
        }
    }

    void DisplayLinks(SplineController controller)
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

    void Display(SplineController controller)
    {
        DisplayLinks(controller);

        Handles.color = Color.white;

        Vector3 StartPoint = controller.EvaluateFromMatrix(0f);

        for (float quantity = positionDistance; quantity < 1f; quantity += positionDistance)
        {
            Vector3 EndPoint = controller.EvaluateFromMatrix(quantity);
            Handles.DrawLine(StartPoint, EndPoint);

            StartPoint = EndPoint;
            quantity += positionDistance;
        }
    }

    void OnSceneGUI()
    {
        SplineController controller = target as SplineController;

        Input(controller);
        Display(controller);
    }
}
