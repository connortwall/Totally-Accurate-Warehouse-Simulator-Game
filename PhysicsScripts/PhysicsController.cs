using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

[Serializable]
public class PhysicsController : PseudoMonobehaviour
{
    public Transform selfTransform;
    [FormerlySerializedAs("objRefTransform")] public Transform refToFollowWhenNotMoving;
    [FormerlySerializedAs("refTransform")] public Transform refToFollowWhenMoving;

    public float force = 10.0f;
    public float forceFactor = 5.0f;
    
    public float torque = 2000.0f;
    public float torqueFactor = 5.0f;

    // // --- STANDING --- //
    // public bool SupportingStanding = false;
    // public Transform standRefTransform;
    // public float standForce;
    // public float standTorque;
    
    // // --- MOVING --- //
    public bool FollowingMoving = false;
    // public Transform moveRefTransform;
    // public float moveForce;
    // public float moveTorque;
    
    // // --- JUMPING --- //
    // public bool FollowingJumping = false;
    // public Transform jumpRefTransform;
    // public float jumpForce;
    // public float jumpTorque;
    
    // // --- CRANCHING --- //
    public bool FollowingCranching = false;
    // public Transform cranchRefTransform;
    // public float cranchForce;
    // public float cranchTorque;
    
    private Rigidbody _rb;

    public override void Awake()
    {
        _rb = selfTransform.gameObject.GetComponent<Rigidbody>();
    }
    
    public override void FixedUpdate()
    {
        if (PuppetController.IsDead)
            return;
        
        // if (SupportingStanding && PuppetController.IsStanding)
        // {
        //     FixedUpdateStand();
        // }
        //
        // if (FollowingMoving && PuppetController.IsMoving)
        // {
        //     FixedUpdateMove();
        // }
        //
        // if (FollowingJumping && PuppetController.IsJumping)
        // {
        //     FixedUpdateJump();
        // }
        //
        // if (FollowingCranching && PuppetController.IsCranching)
        // {
        //     FixedUpdateCranch();
        // }

        Transform selectedTransform = PuppetController.IsMoving switch
        {
            true => refToFollowWhenMoving,
            _ => refToFollowWhenNotMoving
        };
        
        // selectedTransform = PuppetController.IsJumping switch
        // {
        //     true => objRefTransform,
        //     _ => selectedTransform
        // };
        
        Vector3 forceDirection = selectedTransform.position - selfTransform.position;
        
        float positionForce = FollowingMoving switch
        {
            true => force,
            _ => 0f
        };
        
        float factoredForce = PuppetController.IsMoving switch
        {
            true => forceFactor * positionForce,
            _ => positionForce
        };
        
        if (force > 0)
        {
            _rb.AddForce(factoredForce * forceDirection, ForceMode.Acceleration);
        }
        
        float angle = Vector3.SignedAngle(selectedTransform.forward, selfTransform.forward, Vector3.down) / 10.0f;
            
        float angleFactor = PuppetController.IsMoving switch
        {
            true => torqueFactor * torque,
            _ => FollowingMoving switch
            {
                true => torque,
                _ => 0.0f
            }
        };
        
        if (torque > 0 || (PuppetController.IsMoving && FollowingMoving))
        {
            _rb.AddTorque(new Vector3(0, Mathf.Sign(angle) * angleFactor, 0), ForceMode.Acceleration);
        }

        // if (jumpRelated && PuppetController.IsJumping)
        // {
        //     _rb.AddForce(force * Vector3.up, ForceMode.Acceleration);
        //     
        //     // TODO: remove static setter
        //     // PuppetController.IsJumping = false;
        // }
        //
        // if (cranchRelated && PuppetController.IsCranching)
        // {
        //     _rb.AddForce(force * Vector3.down, ForceMode.Acceleration);
        //     
        //     // TODO: remove static setter
        //     // PuppetController.IsCranching = false;
        // }
    }
    
    // private void FixedUpdateStand()
    // {
    //     PhysicalMotion(standRefTransform, force, torque);
    // }
    //
    // private void FixedUpdateMove()
    // {
    //     PhysicalMotion(moveRefTransform, force, torque);
    // }
    //
    // private void FixedUpdateJump()
    // {
    //     PhysicalMotion(jumpRefTransform, force, torque);
    // }
    //
    // private void FixedUpdateCranch()
    // {
    //     PhysicalMotion(cranchRefTransform, force, torque);
    // }

    private void PhysicalMotion(Transform selectedTransform, float force, float torque)
    {
        Vector3 forceDirection = selectedTransform.position - selfTransform.position;
        _rb.AddForce(force * forceDirection.normalized, ForceMode.Acceleration);
        
        float angle = Vector3.SignedAngle(selectedTransform.forward, selfTransform.forward, Vector3.down);
        _rb.AddTorque(torque * new Vector3(0, Mathf.Sign(angle), 0), ForceMode.Acceleration);
    }

    public void AddForce(float force)
    {
        _rb.AddForce(force * Vector3.down, ForceMode.Acceleration);
    }
}
