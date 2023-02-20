using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SplineController : MonoBehaviour
{
    public List<Vector3> InputPoint = new List<Vector3>();

    Matrix4x4 M = new Matrix4x4(new Vector4(2f,-2f, 1f, 1f), new Vector4(-3f, 3f,-2f,-1f),
                                new Vector4(0f, 0f, 1f, 0f), new Vector4(1f, 0f, 0f, 0f));

    public int PositionCount => InputPoint.Count / 2;

    public Vector3 EvaluateFromPolynomial(float t)
    {
        float tSqr = t * t;
        float tCube = tSqr * t;

        Vector3 PointA = InputPoint[0];
        Vector3 PointB = InputPoint[2];

        Vector3 DerivA = InputPoint[1] - PointA;
        Vector3 DerivB = InputPoint[3] - PointB;

        return (2f * tCube - 3f * tSqr + 1f) * PointA + (-2f * tCube + 3f * tSqr) * PointB + (tCube - 2f * tSqr + t) * DerivA + (tCube - tSqr) * DerivB;
    }

    public Vector3 EvaluateFromMatrix(float time)
    {
        return Matrix4x4.Transpose(GetGeometryMatrix()) * Matrix4x4.Transpose(M) * GetTimeVector(time);
    }

    public Vector4 GetTimeVector(float time)
    {
        float timeSqr = time * time;
        float timeCube = timeSqr * time;
        return new Vector4(timeCube, timeSqr, time, 1f);
    }

    public Matrix4x4 GetGeometryMatrix()
    {
        Vector3 PointA = InputPoint[0];
        Vector3 PointB = InputPoint[2];

        Vector3 DerivA = InputPoint[1] - PointA;
        Vector3 DerivB = InputPoint[3] - PointB;

        return new Matrix4x4(new Vector4(PointA.x, PointA.y, PointA.z, 0f), new Vector4(PointB.x, PointB.y, PointB.z, 0f),
                             new Vector4(DerivA.x, DerivA.y, DerivA.z, 0f), new Vector4(DerivB.x, DerivB.y, DerivB.z, 1f));
    }

}
