using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private PlayerInput input;
    private InputAction _leanAction;
    private InputAction _moveAction;
    private InputAction _swingAction;

    [SerializeField] private float leanRange = 0.8f;
    [SerializeField] private Transform headTransform;

    [SerializeField] private Transform refTransform;
    [SerializeField] private Transform weaponTransform;
    
    private Vector2 moveDirection = Vector2.zero;

    [SerializeField] private float swingRange = 0.5f;
    [SerializeField] private float swingHeight = 0.8f;
    [SerializeField] private float swingVelocityThreshold = 0.5f;
    [SerializeField] private float swingSpeed = 1.0f;

    private bool isSwing = false;
    private bool isMove = false;

    [SerializeField] private Animator refAnimator;

    private void Awake()
    {
        _leanAction = input.actions["Leaning"];
        _moveAction = input.actions["walk"];
        _swingAction = input.actions["Arms"];
    }

    // TODO: figure out "OnHold" and "OnRelease" state in Class InputAction 
    private void OnHoldLeanAction()
    {
        Vector2 leanActionControllerValue = _leanAction.ReadValue<Vector2>();
        moveDirection = leanActionControllerValue;

        Vector2 leanActionControllerValueRanged = leanRange * leanActionControllerValue;
        Vector3 leanDirection = new Vector3(leanActionControllerValueRanged.x, 0, leanActionControllerValueRanged.y);
        refTransform.rotation = Quaternion.LookRotation(leanDirection, Vector3.up);

        float leanActionControllerValueMagnitude = leanActionControllerValue.magnitude; // always be 1
        float leanActionControllerValueMagnitudeRanged = leanRange * leanActionControllerValue.magnitude;
        headTransform.localPosition = leanActionControllerValueMagnitudeRanged * transform.forward + new Vector3(0, 1.6f, 0);
    }

    private void OnHoldSwingAction()
    {
        Vector2 swingActionControllerValue = _swingAction.ReadValue<Vector2>();
        
        Vector2 swingActionControllerValueRanged = swingRange * swingActionControllerValue;
        Vector3 swingDirection = new Vector3(swingActionControllerValueRanged.x, swingHeight, swingActionControllerValueRanged.y);
        weaponTransform.position = swingDirection + refTransform.position;

        isSwing = true;
    }

    private void OnReleaseSwingAction()
    {
        isSwing = false;
    }

    private void OnHoldMoveAction()
    {
        isMove = true;
        refAnimator.SetBool("walking", true);

        refTransform.position += swingSpeed * Time.deltaTime * new Vector3(moveDirection.x, 0, moveDirection.y);
    }

    private void OnReleaseMoveAction()
    {
        isMove = false;
        refAnimator.SetBool("walking", false);
    }

    // game over condition varies?
    private void OnGameOver()
    {
        throw new NotImplementedException();
    }
}
