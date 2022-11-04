using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
// [RequireComponent(typeof(FixedJoint))]
public class GrabbableItem : Item
{
    protected Joint _joint = null;
    public float breakForce = 10000.0f;
    public float maxGrabDistance = 1000.0f;

    private bool _isGrabbed = false;
    public bool IsGrabbed => _isGrabbed;
        
    private void Awake()
    {
        base.Awake();
        
        // _joint = GetComponent<Joint>();
        // _joint.breakForce = breakForce;
    }

    private void FixedUpdate()
    {
        if (_joint != null && 
            (_joint.currentForce.magnitude > breakForce || 
             Vector3.Distance(transform.position, _joint.connectedBody.transform.position) > maxGrabDistance))
        {
            OnRelease(_joint.currentForce);
        }
    }

    public void OnGrab(Rigidbody rb)
    {
        if (_isGrabbed) 
            return ;

        if (_joint == null)
        {
            _joint = gameObject.AddComponent<FixedJoint>();
        }
        
        Debug.Log("Some item : You got me!");
        
        _joint.connectedBody = rb;
        _isGrabbed = true;
    }

    public void OnRelease(Vector3 force)
    {
        if (!_isGrabbed)
            return ;
        
        Debug.Log("Some item again : You lose me!");
        
        // _joint.connectedBody = null;
        Destroy(_joint);
        _rb.AddForce(force, ForceMode.Impulse);

        _isGrabbed = false;
    }
}
