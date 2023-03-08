using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SplineController))]
public class SplineSceneEditor : Editor
{
    [SerializeField] private float positionDistance = 0.01f;

    void Input(SplineController controller)
    {
        for (int PointId = 0; PointId < controller.InputPoint.Count; PointId++)
        {
            Handles.color = controller.IsPointAKnot(PointId) ? Color.yellow : Color.grey;

            controller.InputPoint[PointId] = Handles.FreeMoveHandle(controller.InputPoint[PointId], Quaternion.identity, 1f, Vector3.zero, Handles.SphereHandleCap);
        }

        if (GUI.changed)
            EditorUtility.SetDirty(target);
    }

    void DisplayLinks(SplineController controller)
    {
        Handles.color = Color.red;

        switch (controller.SplineFormula)
        {
            case HermitianSpline hermitian:
                for (int PointId = 0; PointId < controller.InputPoint.Count; PointId += 2)
                {
                    Vector3 Position = controller.InputPoint[PointId + 0];
                    Vector3 Velocity = controller.InputPoint[PointId + 1];
                    Handles.DrawLine(Position, Velocity);
                }
                break;

            case BezierSpline bezier:
                for (int PointId = 0; PointId < controller.InputPoint.Count; PointId += 3)
                {
                    Vector3 Position = controller.InputPoint[PointId + 0];

                    if (PointId > 0)
                    {
                        Vector3 PrevVelocity = controller.InputPoint[PointId - 1];
                        Handles.DrawLine(Position, PrevVelocity);
                    }

                    if (PointId < controller.InputPoint.Count - 1)
                    {
                        Vector3 NextVelocity = controller.InputPoint[PointId + 1];
                        Handles.DrawLine(Position, NextVelocity);
                    }
                }
                break;

            case BSpline bspline:
                for (int PointId = 0; PointId < controller.InputPoint.Count - 1; PointId++)
                {
                    Vector3 PositionA = controller.InputPoint[PointId + 0];
                    Vector3 PositionB = controller.InputPoint[PointId + 1];
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
