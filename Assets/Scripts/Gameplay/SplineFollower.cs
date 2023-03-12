using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SplineFollower : MonoBehaviour
{
    [SerializeField] private SplineController splineToFollow = null;
    [SerializeField] private float speed = 5f;
    private float avancement = 0f;

    int motionSign = 1;

    private float frameSpeed => motionSign * Time.fixedDeltaTime * speed;

    void FixedUpdate()
    {
        if (!splineToFollow)
            return;

        avancement += frameSpeed;

        Vector3 currentPosition = splineToFollow.EvaluatePosition(avancement);

        if (avancement >= 1f || avancement <= 0f)
            motionSign = -motionSign;

        Vector3 direction = splineToFollow.EvaluateTangent(avancement);

        transform.position = currentPosition;
        transform.forward = motionSign * direction;
    }
}
