using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public class PuppetController : MonoBehaviour
{
    #region INPUT ACTIONS AND STATES

    [SerializeField] private PlayerInput input;

    // --- LEAN ACTION --- //
    private InputAction _leanAction;
    private static bool _isLeaning = false;
    public static bool IsLeaning => _isLeaning;

    // --- MOVE ACTION --- //
    private InputAction _moveAction;
    private static bool _isMoving = false;
    public static bool IsMoving => _isMoving;

    // --- GRAB ACTION --- //
    private InputAction _grabAction;
    private static bool _isGrabbing = false;
    public static bool IsGrabbing => _isGrabbing;

    public static void SetGrabbingState(bool state)
    {
        _isGrabbing = state;
    }

    // --- SWING ACTION --- //
    private InputAction _swingAction;
    private static bool _isSwinging = false;
    public static bool IsSwinging => _isSwinging;

    // --- JUMP ACTION --- //
    private InputAction _jumpAction;
    private static bool _isJumping = false;

    public static bool IsJumping => _isJumping;

    // --- CRANCH ACTION --- //
    private InputAction _cranchAction;
    private static bool _isCranching = false;
    public static bool IsCranching => _isCranching;

    #endregion

    #region ANIMATION AND STATES

    [SerializeField] private Animator animator;

    // --- WALKING ANIMATION --- //
    [SerializeField] private string walkingAnimationName = "Walking";
    private static bool _isWalkingAnimating = false;
    public static bool IsWalkingAnimating => _isWalkingAnimating;

    // --- JUMPING ANIMATION --- //
    [SerializeField] private string jumpingAnimationName = "Jumping";
    private static bool _isJumpingAnimating = false;
    public static bool IsJumpingAnimating => _isJumpingAnimating;

    // --- CRANCHING ANIMATION --- //
    [SerializeField] private string cranchingAnimationName = "Cranching";
    private static bool _isCranchingAnimating = false;
    public static bool IsCranchingAnimating => _isCranchingAnimating;

    #endregion

    #region OTHER STATES

    // --- STAND STATE --- //
    public static bool IsStanding =>
        !_isMoving && !_isJumping && !_isCranching;

    // --- DEAD STATE --- //
    private static bool _isDead = false;
    public static bool IsDead => _isDead;

    // --- GAMEOVER STATE --- //
    private static bool _isGameOver = false;
    public static bool IsGameOver => _isGameOver;

    #endregion

    #region TRANSFORMS

    // --- HEAD REF TRANSFORM --- //
    [SerializeField] private Transform headRefTransform;
    [SerializeField] private Vector3 headRefLocalPositionDefault = new Vector3(0, 1.6f, 0);

    // --- FEET REF TRANSFORM --- //
    [SerializeField] private Transform feetRefTransform;

    // -- DUMMY REF TRANSFORM --- //
    [SerializeField] private Transform refTransform;
    [SerializeField] private Transform reference;

    // --- HAND REF TRANSFORM --- //
    [SerializeField] private Transform swingRefTransform;

    // --- RIG TRANSFORM --- //
    [SerializeField] private Transform rigTransform;

    // --- BODY TRANSFORMS --- //
    [SerializeField] private Transform bodyTransform;
    [SerializeField] private Transform bodyRefTransform;

    #endregion

    #region MONOBEHAVIOURS

    private void Awake()
    {
        AwakePlayerController();
        AwakeGroundDetection();
        AwakeFoot();
        AwakeRigidbodyInit();
        AwakePhysics();
        AwakeGrab();
    }

    // private void Start()
    // {
    //     throw new NotImplementedException();
    // }

    private void Update()
    {
        UpdatePlayerController();
        UpdateRefMovement();
        UpdateFoot();
    }

    private void FixedUpdate()
    {
        FixedUpdatePhysics();
        FixedUpdateGrab();
    }

    #endregion

    #region IS_GROUNDED

    [SerializeField] private Collider groundDetectionCollider;

    private static float _distToGround;
    private static Transform _colliderTransform;

    public static bool IsGrounded => Physics.Raycast(_colliderTransform.position, Vector3.down, _distToGround + 0.01f);

    private void AwakeGroundDetection()
    {
        _distToGround = groundDetectionCollider.bounds.extents.y;
        _colliderTransform = groundDetectionCollider.transform;
    }

    // private void UpdateGroundDetection()
    // {
    //     _colliderTransform = groundDetectionCollider.transform;
    // }

    #endregion

    #region REF_MOVEMENT

    private void UpdateRefMovement()
    {
        reference.position = refTransform.position;

        Quaternion.LookRotation(new Vector3(refTransform.forward.x, 0, refTransform.forward.z), Vector3.up);
    }

    #endregion

    #region FOOT_CONTROLLER

    [SerializeField] private List<FootController> footControllers = new List<FootController>();

    private void AwakeFoot()
    {

    }

    private void UpdateFoot()
    {
        footControllers.ForEach(fc => fc.Update());
    }

    #endregion

    #region RIGIDBODY_INIT

    private List<Rigidbody> _rbs = new List<Rigidbody>();

    [SerializeField] private bool useUnifiedRigidbodyParameters = false;

    [SerializeField] private float mass = 10.0f;
    [SerializeField] private float drag = 6.0f;
    [SerializeField] private float angularDrag = 0.05f;

    private void AwakeRigidbodyInit()
    {
        if (!useUnifiedRigidbodyParameters)
            return;

        _rbs = bodyTransform.gameObject.GetComponentsInChildren<Rigidbody>().ToList();
        _rbs.ForEach(rb =>
        {
            rb.mass = mass;
            rb.drag = drag;
            rb.angularDrag = angularDrag;
        });
    }

    #endregion

    #region PLAYER_CONTROLLER

    private Vector2 _moveDirection = Vector2.zero;

    // --- MOVE ACTION PARAMETERS --- //
    [SerializeField] private float moveSpeed = 1.0f;

    // --- LEAN ACTION PARAMETERS --- //
    [SerializeField] private float leanRange = 0.8f;

    // --- SWING ACTION PARAMETERS --- //
    [SerializeField] private float swingRange = 1.8f;
    [SerializeField] private float swingHeight = 0.8f;

    private void AwakePlayerController()
    {
        // --- LEAN ACTION --- //
        _leanAction = input.actions["lean"];
        _leanAction.performed += ctx => OnHoldLeanAction();
        _leanAction.canceled += ctx => OnReleaseLeanAction();

        // --- SWING ACTION --- //
        _swingAction = input.actions["swing"];
        // _swingAction.performed += ctx => OnHoldSwingAction();
        // _swingAction.canceled += ctx => OnReleaseGrabAction();

        // --- MOVE ACTION --- //
        _moveAction = input.actions["move"];

        // --- MOVE ACTION --- //
        _grabAction = input.actions["grab"];
        // _grabAction.canceled += ctx => OnReleaseGrabAction();

        // --- JUMP ACTION --- //
        _jumpAction = input.actions["jump"];
        //_jumpAction.started += ctx => OnClickJumpAction();
        //_jumpAction.canceled += ctx => OnReleaseJumpAction();

        // --- CRANCH ACTION --- //
        _cranchAction = input.actions["cranch"];
        // _cranchAction.started += ctx => OnClickCranchAction();
        // _cranchAction.canceled += ctx => OnReleaseCranchAction();

        //_jumpAction.Disable();
        //_swingAction.Disable();
    }

    private void UpdatePlayerController()
    {
        // if (_leanAction.IsPressed())
        // {
        //     // OnHoldLeanAction();
        // }

        // if (_leanAction.WasReleasedThisFrame())
        // {
        //     OnReleaseLeanAction();
        // }

        /*
        if (_swingAction.IsPressed())
        {
            OnHoldSwingAction();
        }
        
        if (_swingAction.WasReleasedThisFrame())
        {
            OnReleaseSwingAction();
        }
        */

        if (_moveAction.IsPressed())
        {
            OnHoldMoveAction();
        }

        if (_moveAction.WasReleasedThisFrame())
        {
            OnReleaseMoveAction();
        }

        /*
        if (_grabAction.IsPressed())
        {
            OnHoldGrabAction();
        }
        */

        // if (_grabAction.WasReleasedThisFrame())
        // {
        //     OnReleaseGrabAction();
        // }

        // if (_jumpAction.WasReleasedThisFrame())
        // {
        //     OnReleaseJumpAction();
        // }

        if (_cranchAction.IsPressed())
        {
            OnClickCrouchAction();
        }

        if (_cranchAction.WasReleasedThisFrame())
        {
            OnReleaseCrouchAction();
        }
    }

    private void OnHoldLeanAction()
    {
        _isLeaning = true;

        Vector2 leanActionControllerValue = _leanAction.ReadValue<Vector2>();
        _moveDirection = leanActionControllerValue;

        Vector2 leanActionControllerValueRanged = leanRange * leanActionControllerValue;
        Vector3 leanDirection = new Vector3(leanActionControllerValueRanged.x, 0, leanActionControllerValueRanged.y);
        refTransform.rotation = Quaternion.LookRotation(leanDirection, Vector3.up);

        float leanActionControllerValueMagnitudeRanged = leanRange * leanActionControllerValue.magnitude;

        headRefTransform.localPosition = leanActionControllerValueMagnitudeRanged * rigTransform.forward + headRefLocalPositionDefault;
    }

    private void OnReleaseLeanAction()
    {
        _isLeaning = false;

        headRefTransform.localPosition = headRefLocalPositionDefault;
    }

    private void OnHoldSwingAction()
    {
        _isSwinging = true;

        Vector2 swingActionControllerValue = _swingAction.ReadValue<Vector2>();

        Vector2 swingActionControllerValueRanged = swingRange * swingActionControllerValue;
        Vector3 swingDirection = new Vector3(swingActionControllerValueRanged.x, swingHeight, swingActionControllerValueRanged.y);
        swingRefTransform.position = swingDirection + refTransform.position;
    }

    private void OnReleaseSwingAction()
    {
        _isSwinging = false;
    }

    private void OnHoldMoveAction()
    {
        if (!_isLeaning)
            return;

        _isMoving = true;

        animator.SetBool(walkingAnimationName, true);

        refTransform.position += moveSpeed * Time.deltaTime * new Vector3(_moveDirection.x, 0, _moveDirection.y);
    }

    private void OnReleaseMoveAction()
    {
        _isMoving = false;

        animator.SetBool(walkingAnimationName, false);
    }

    private void OnClickJumpAction()
    {
        if (!IsGrounded || _isJumping || _isCranching)
            return;

        Jump();
    }

    private void OnReleaseJumpAction()
    {
        // StopCoroutine(_jumpCoutine);
        // _isJumping = false;
    }

    private void OnClickCrouchAction()
    {
        if (_isCranching || _isJumping)
            return;

        _isCrouchPressed = true;
        _cranchCoroutine = StartCoroutine(CrouchWhenPressCoroutine());

        // Cranch();
    }

    private void OnReleaseCrouchAction()
    {
        // --- UNCOMMENT ONLY WHEN USING CrouchWhenPressed() METHOD --- ///
        _isCrouchPressed = false;
        // bodyRefTransform.position += Vector3.up;
    }

    /*
    private void OnHoldGrabAction()
    {
        _isGrabbing = true;
        
        Debug.Log("You can grab");
    }

    private void OnReleaseGrabAction()
    {
        grabController.ReleaseItem();
        
        _isGrabbing = false;
    }
    */

    #endregion

    #region PHYSICS_CONTROLLER

    [SerializeField] private List<PhysicsController> physicsControllers = new List<PhysicsController>();

    private void AwakePhysics()
    {
        physicsControllers.ForEach(pc =>
        {
            pc.Awake();
        });
    }

    private void FixedUpdatePhysics()
    {
        physicsControllers.ForEach(pc => pc.FixedUpdate());
    }

    #endregion

    #region GRAB_CONTROLLER

    [SerializeField] private List<HandController> handControllers;
    [SerializeField] private GrabController grabController;

    private void AwakeGrab()
    {
        // handControllers.ForEach(gc => gc.Awake());
    }

    private void FixedUpdateGrab()
    {
        // handControllers.ForEach(gc => gc.FixedUpdate());
    }

    #endregion

    #region ACTIONS

    /// <summary>
    ///  --- JUMP ACTION --- //
    /// </summary>

    private Coroutine _jumpCoroutine = null;
    public AnimationCurve jumpControlCurve;
    [SerializeField] private float jumpCurveFactor = 5.0f;
    [SerializeField] private float jumpTimeInSecond = 1.0f;

    private void Jump()
    {
        _jumpCoroutine = StartCoroutine(JumpCoroutine());
    }

    private IEnumerator JumpCoroutine()
    {
        _isJumping = true;
        Vector3 headRefTransformPosInit = refTransform.position;

        for (int i = 0; i < 50; ++i)
        {
            // Debug.Log(100 * jumpVelocityCurve.Evaluate((float)i/50));
            // bodyTransform.GetComponent<Rigidbody>().velocity = new Vector3(0, 100 * jumpVelocityCurve.Evaluate((float)i/50), 0);
            refTransform.position = new Vector3(
                refTransform.position.x,
                headRefTransformPosInit.y + jumpCurveFactor * jumpControlCurve.Evaluate((float)i / 50),
                refTransform.position.z
            );

            yield return null;
        }

        Debug.Log("Jump Done");
        refTransform.position = new Vector3(refTransform.position.x, headRefTransformPosInit.y, refTransform.position.z);

        _isJumping = false;
    }

    /// <summary>
    ///  --- CRANCH ACTION --- //
    /// </summary>

    private Coroutine _cranchCoroutine = null;
    public AnimationCurve cranchControlCurve;
    [SerializeField] private float cranchCurveFactor = 5.0f;
    // [SerializeField] private List<Rigidbody> crouchBodies;

    private void Cranch()
    {
        // _cranchCoroutine = StartCoroutine(CranchCoroutine());
        CrouchWhenPressed();
    }

    private IEnumerator CrouchCoroutine()
    {
        // if (_isCranching)
        //     yield break;

        _isCranching = true;
        Vector3 refTransformPosInit = headRefTransform.position;

        for (int i = 0; i < 50; ++i)
        {
            headRefTransform.position = new Vector3(
                headRefTransform.position.x,
                refTransformPosInit.y - cranchCurveFactor * cranchControlCurve.Evaluate((float)i / 50),
                headRefTransform.position.z);

            yield return null;
        }

        Debug.Log("Cranch Done");
        headRefTransform.position = new Vector3(headRefTransform.position.x, refTransformPosInit.y, headRefTransform.position.z);

        _isCranching = false;
    }

    private bool _isCrouchPressed = false;
    public float crouchHeight = 0.5f;
    private IEnumerator CrouchWhenPressCoroutine()
    {
        // if (_isCranching)
        //     yield break;

        _isCranching = true;
        float normalHeight = headRefLocalPositionDefault.y;
        Vector3 refTransformPosInit = headRefTransform.position;

        while (_isCrouchPressed)
        {
            headRefTransform.position = new Vector3(
                headRefTransform.position.x,
                crouchHeight,
                headRefTransform.position.z);

            yield return null;
        }

        Debug.Log("Crouch Done");
        // headRefTransform.position = new Vector3(headRefTransform.position.x, normalHeight, headRefTransform.position.z);
        headRefTransform.localPosition = headRefLocalPositionDefault;

        _isCranching = false;
    }

    #endregion

    #region UTILITIES

    IEnumerator WaitForSecondsCoroutine(int sec)
    {
        yield return new WaitForSeconds(sec);
    }

    #endregion

    #region LEGACY

    // [SerializeField] private List<Rigidbody> jumpRigidbodies = new List<Rigidbody>();
    // [SerializeField] private float jumpImpulseForce = 1000.0f;
    //
    // [SerializeField] private List<Rigidbody> cranchRigidbodies = new List<Rigidbody>();
    // [SerializeField] private float cranchImpulseForce = 1000.0f;

    [SerializeField] private float crouchForce = 400.0f;
    private void CrouchWhenPressed()
    {
        _isCranching = true;
        bodyRefTransform.position += Vector3.down;
    }

    #endregion

}
