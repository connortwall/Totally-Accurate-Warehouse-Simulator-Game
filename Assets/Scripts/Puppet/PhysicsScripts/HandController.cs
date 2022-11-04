using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

[Serializable]
[RequireComponent(typeof(Rigidbody))]
public class HandController
{
    public Transform selfTransform;
    public Transform refTransform;
    public Transform walkRefTransform;
    public Transform forceImpTransform;
    
    public float force = 2000.0f;
    public float torque = 0.0f;
    
    private Rigidbody _rb;
    private bool _isLoaded = false;

    public void Awake()
    {
        _rb = selfTransform.gameObject.GetComponent<Rigidbody>();
    }

    public void FixedUpdate()
    {
        Vector3 forceDirection = refTransform.position - selfTransform.position;
        
        if (!PuppetController.IsDead && PuppetController.IsSwinging && force > 0)
        {
            _rb.AddForceAtPosition(force * forceDirection, forceImpTransform.position, ForceMode.Acceleration);
        }

        float angle = Vector3.SignedAngle(selfTransform.forward, walkRefTransform.forward, Vector3.right) / 10.0f;

        if (torque > 0)
        {
            _rb.AddTorque(new Vector3(angle * torque, 0, 0), ForceMode.Acceleration);
        }
    }
    
    // private void OnCollisionStay(Collision collision)
    // {
    //     if (_isLoaded || !collision.gameObject.CompareTag("Item") || !PuppetController.IsGrabbing)
    //         return ;
    //
    //     collision.gameObject.GetComponent<Joint>().connectedBody = _rb;
    //     collision.transform.parent = selfTransform;
    // }

    public void ReleaseItem()
    {
        if (selfTransform.childCount == 0)
            return ;
        
        var jointList = selfTransform.GetComponentsInChildren<Joint>().ToList();
        jointList.ForEach(j =>
        {
            j.connectedBody = null;
            j.gameObject.transform.parent = null;
        });
        
        // selfTransform.DetachChildren();
    }
}
