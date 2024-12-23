using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public static class Core
{
    public static IEnumerator DelayFrame(Action action, int frame = 1)
    {
        for (int i = 0; i < frame; i++)
            yield return null;
        action?.Invoke();
    }
    public static float Remap(float value, float inputMin, float inputMax, float outputMin, float outputMax)
    {
        return outputMin + (value - inputMin) * (outputMax - outputMin) / (inputMax - inputMin);
    }
    public static Vector3 RotateVector(Vector3 referenceVector, Vector3 targetVector)
    {
        // referenceVector 방향을 기준으로 targetVector를 회전
        Quaternion rotation = Quaternion.LookRotation(referenceVector);
        return rotation * targetVector;
    }
}
