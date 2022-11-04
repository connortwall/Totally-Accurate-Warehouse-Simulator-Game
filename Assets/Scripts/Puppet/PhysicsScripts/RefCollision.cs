using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RefCollision : MonoBehaviour
{
    private bool _isCollided = false;
    private Collider _collider;
    private Vector3 _collisionPos = Vector3.zero;

    private static float distToGround;
    private static Transform groundTransform;
    
    public static bool IsGrounded => Physics.Raycast(groundTransform.position, Vector3.down, distToGround + 0.01f);
    
    private void Awake()
    {
        _collider = GetComponent<Collider>();
        
        distToGround = _collider.bounds.extents.y;
    }

    // Update is called once per frame
    private void Update()
    {
        groundTransform = transform;
        Debug.Log("Grounded = "+ IsGrounded);
    }
 
    // function Update () {
    //     if (Input.GetKeyDown(KeyCode.Space) && IsGrounded()){
    //         rigidbody.velocity.y = jumpSpeed;
    //     }
    // }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Shelf"))
        {
            // Debug.Log("Be Careful!");

            _isCollided = true;
            _collisionPos = transform.position;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("Shelf"))
        {
            transform.position = _collisionPos;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Shelf"))
        {
            // Debug.Log("collision ends");
            _isCollided = false;
        }
    }

    // private void OnCollisionEnter(Collision collision)
    // {
    //     if (collision.gameObject.CompareTag("Shelf"))
    //     {
    //         Debug.Log("Be Careful!");
    //         _isCollided = true;
    //         _collisionPos = transform.position;
    //     }
    // }

    // private void OnCollisionExit(Collision other)
    // {
    //     _isCollided = false;
    // }
}
