using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private Transform refTransform;
    
    private void Update()
    {
        transform.position = refTransform.position + transform.position;
        transform.rotation = Quaternion.Euler(transform.eulerAngles);
    }
}
