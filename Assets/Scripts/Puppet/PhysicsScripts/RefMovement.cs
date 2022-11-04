using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RefMovement : MonoBehaviour
{
    [SerializeField] private Transform refTransform;

    // Update is called once per frame
    void Update()
    {
        transform.position = refTransform.position;
        Vector3 refForward = refTransform.forward;
        
        Quaternion.LookRotation(new Vector3(refForward.x, 0, refForward.z), Vector3.up);
    }
}
