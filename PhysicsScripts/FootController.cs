using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

[Serializable]
public class FootController
{
    public Transform headRefTransform;
    public Transform selfTransform;

    public void Update()
    {
        Vector3 rotEuler = new Vector3(0, headRefTransform.eulerAngles.y, 0);
        selfTransform.rotation = quaternion.Euler(rotEuler);
    }
}
