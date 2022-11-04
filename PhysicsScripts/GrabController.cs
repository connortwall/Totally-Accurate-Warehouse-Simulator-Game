using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GrabController : MonoBehaviour
{
    private Rigidbody _rb;
    
    public Transform selfTransform;
    public Transform refTransform;
    public Transform walkRefTransform;
    public Transform forceImpTransform;
    
    public float force = 2000.0f;
    public float torque = 0.0f;
    public float throwForce = 100.0f;
    public float throwVelocityThreshold = 1.0f;

    private GrabbableItem _grabbable;
    private bool _isLoaded = false;
    
    void Awake()
    {
        _rb = gameObject.GetComponent<Rigidbody>();
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
            _rb.AddTorque(new Vector3(Mathf.Sign(angle) * torque, 0, 0), ForceMode.Acceleration);
        }
    }
    
    private void OnTriggerStay(Collider collider)
    {
        if (PuppetController.IsGrabbing)
        {
            GrabItem(collider);
        }

        // _grabbable.IsGrabbed = true;
        // collision.transform.parent = transform;
        // collision.gameObject.GetComponent<Joint>().connectedBody = _rb;
    }

    private void OnTriggerExit(Collider other)
    {
        // ReleaseItem();
    }

    // --- GRAB ITEM --- //
    public void GrabItem(Collider collider)
    {
        if (_isLoaded)
            return;

        _grabbable = collider.gameObject.GetComponent<GrabbableItem>();
        if (_grabbable == null)
            return ;

        _isLoaded = true;
        _grabbable.OnGrab(_rb);
    }

    // --- RELEASE ITEM --- //
    public void ReleaseItem()
    {
        if (!_isLoaded)
            return ;

        if (!_grabbable.IsGrabbed)
        {
            _isLoaded = false;
            _grabbable = null;
            return;
        }
        
        Vector3 throwDirection = (_rb.velocity.magnitude < throwVelocityThreshold) switch
        {
            true => Vector3.zero,
            _ => _rb.velocity.normalized
        };
        
        Debug.Log("throw direction = " + throwDirection);

        _grabbable.OnRelease(throwForce * throwDirection);
        _grabbable = null;
        
        _isLoaded = false;
    }
}
