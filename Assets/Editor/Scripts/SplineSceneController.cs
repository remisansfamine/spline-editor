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

    void Display(SplineController controller)
    {
        Handles.color = Color.red;

        for (int PointId = 0; PointId < controller.InputPoint.Count; PointId += 2)
        {
            Vector3 Position = controller.InputPoint[PointId + 0];
            Vector3 Velocity = controller.InputPoint[PointId + 1];
            Handles.DrawLine(Position, Velocity);
        }

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
