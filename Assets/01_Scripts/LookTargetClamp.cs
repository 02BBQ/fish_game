using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookTargetClamp : MonoBehaviour
{
    public Transform target;         // �ٶ� Ÿ��
    public float maxRotationAngle = 45f;
    void LateUpdate()
    {
        Vector3 directionToTarget = target.position - transform.position;

        Quaternion targetRotation = Quaternion.LookRotation(directionToTarget);
        Quaternion origin = transform.rotation;
        transform.rotation = targetRotation;
        float angleDifference = Quaternion.Angle(transform.parent.rotation, transform.rotation);
        if (angleDifference > maxRotationAngle)
        {
            transform.rotation = origin;
        }
    }
}
