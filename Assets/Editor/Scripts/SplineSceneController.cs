using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SplineController))]
public class SplineSceneEditor : Editor
{
    [SerializeField] private static float positionDistance = 0.01f;

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

    static void DisplayLinks(SplineController controller)
    {
        Gizmos.color = Color.red;

        switch (controller.SplineFormula)
        {
            case HermitianSpline hermitian:
                for (int PointId = 0; PointId < controller.GetInputPointCount(); PointId += 2)
                {
                    Vector3 Position = controller.GetInputPoint(PointId + 0);
                    Vector3 Velocity = controller.GetInputPoint(PointId + 1);
                    Gizmos.DrawLine(Position, Velocity);
                }
                break;

            case BezierSpline bezier:
                for (int PointId = 0; PointId < controller.GetInputPointCount(); PointId += 3)
                {
                    Vector3 Position = controller.GetInputPoint(PointId + 0);

                    if (PointId > 0)
                    {
                        Vector3 PrevVelocity = controller.GetInputPoint(PointId - 1);
                        Gizmos.DrawLine(Position, PrevVelocity);
                    }

                    if (PointId < controller.GetInputPointCount() - 1)
                    {
                        Vector3 NextVelocity = controller.GetInputPoint(PointId + 1);
                        Gizmos.DrawLine(Position, NextVelocity);
                    }
                }
                break;

            case BSpline bspline:
                for (int PointId = 0; PointId < controller.GetInputPointCount() - 1; PointId++)
                {
                    Vector3 PositionA = controller.GetInputPoint(PointId + 0);
                    Vector3 PositionB = controller.GetInputPoint(PointId + 1);
                    Gizmos.DrawLine(PositionA, PositionB);
                }
                break;
        }
    }

    static public void Display(SplineController controller)
    {
        DisplayLinks(controller);

        Gizmos.color = Color.white;

        Vector3 StartPoint = Vector3.zero;

        for (float quantity = 0f; quantity <= 1f; quantity += positionDistance)
        {
            Vector3 EndPoint = controller.EvaluateFromMatrix(quantity);
            Gizmos.DrawSphere(EndPoint, 0.1f);

            if (quantity > 0f)
                Handles.DrawLine(StartPoint, EndPoint);

            StartPoint = EndPoint;
        }
    }

    void OnSceneGUI()
    {
        SplineController controller = target as SplineController;

        Input(controller);
    }
}
