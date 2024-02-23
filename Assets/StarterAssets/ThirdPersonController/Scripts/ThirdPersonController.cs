﻿using System;
using DG.Tweening;
using UnityEngine;
 using Random = UnityEngine.Random;
#if ENABLE_INPUT_SYSTEM 
using UnityEngine.InputSystem;
#endif


/* Note: animations are called via the controller for both the character and capsule using animator null checks
 */

namespace StarterAssets
{
    [RequireComponent(typeof(CharacterController))]
#if ENABLE_INPUT_SYSTEM 
    [RequireComponent(typeof(PlayerInput))]
#endif
    public class ThirdPersonController : MonoBehaviour
    {
        [Header("Player")]
        [Tooltip("Move speed of the character in m/s")]
        public float MoveSpeed = 2.0f;

        [Tooltip("Sprint speed of the character in m/s")]
        public float SprintSpeed = 5.335f;

        [Tooltip("How fast the character turns to face movement direction")]
        [Range(0.0f, 0.3f)]
        public float RotationSmoothTime = 0.12f;

        [Tooltip("Acceleration and deceleration")]
        public float SpeedChangeRate = 10.0f;

        public AudioClip LandingAudioClip;
        public AudioClip[] FootstepAudioClips;
        [Range(0, 1)] public float FootstepAudioVolume = 0.5f;

        [Space(10)]
        [Tooltip("The height the player can jump")]
        public float JumpHeight = 1.2f;

        [Tooltip("The character uses its own gravity value. The engine default is -9.81f")]
        public float Gravity = -15.0f;

        [Space(10)]
        [Tooltip("Time required to pass before being able to jump again. Set to 0f to instantly jump again")]
        public float JumpTimeout = 0.50f;

        [Tooltip("Time required to pass before entering the fall state. Useful for walking down stairs")]
        public float FallTimeout = 0.15f;

        [Header("Player Grounded")]
        [Tooltip("If the character is grounded or not. Not part of the CharacterController built in grounded check")]
        public bool Grounded = true;
        
        
        [Header("Player Interaction with Box")]
        public bool isInteracting = false;
        public bool beginInteract = false;
        public bool canMove = true;
        //detect if player can interact with box
        [SerializeField] private bool closeToBox;

        [Tooltip("Useful for rough ground")]
        public float GroundedOffset = -0.14f;

        [Tooltip("The radius of the grounded check. Should match the radius of the CharacterController")]
        public float GroundedRadius = 0.28f;

        [Tooltip("What layers the character uses as ground")]
        public LayerMask GroundLayers;

        [Header("Cinemachine")]
        [Tooltip("The follow target set in the Cinemachine Virtual Camera that the camera will follow")]
        public GameObject CinemachineCameraTarget;

        [Tooltip("How far in degrees can you move the camera up")]
        public float TopClamp = 70.0f;

        [Tooltip("How far in degrees can you move the camera down")]
        public float BottomClamp = -30.0f;

        [Tooltip("Additional degress to override the camera. Useful for fine tuning camera position when locked")]
        public float CameraAngleOverride = 0.0f;

        [Tooltip("For locking the camera position on all axis")]
        public bool LockCameraPosition = false;

        // cinemachine
        private float _cinemachineTargetYaw;
        private float _cinemachineTargetPitch;

        // player
        private float _speed;
        private float _animationBlend;
        private float _targetRotation = 0.0f;
        private float _rotationVelocity;
        private float _verticalVelocity;
        private float _terminalVelocity = 53.0f;

        // timeout deltatime
        private float _jumpTimeoutDelta;
        private float _fallTimeoutDelta;

        // animation IDs
        private int _animIDSpeed;
        private int _animIDGrounded;
        private int _animIDJump;
        private int _animIDFreeFall;
        private int _animIDMotionSpeed;
        private int _animIDPushPose;
        private int _animIDPushAnimation;
        private int _animIDPullAnimation;

#if ENABLE_INPUT_SYSTEM 
        private PlayerInput _playerInput;
#endif
        private Animator _animator;
        private CharacterController _controller;
        private StarterAssetsInputs _input;
        private GameObject _mainCamera;

        private const float _threshold = 0.01f;

        [SerializeField ]private Vector2 normalizeInput;

        private bool _hasAnimator;

        private bool IsCurrentDeviceMouse
        {
            get
            {
#if ENABLE_INPUT_SYSTEM
                return _playerInput.currentControlScheme == "KeyboardMouse";
#else
				return false;
#endif
            }
        }


        private void Awake()
        {
            // Initialize Box interaction variables
            canMove = true;
            beginInteract = false;
            isInteracting = false;
            closeToBox = false;
            // get a reference to our main camera
            if (_mainCamera == null)
            {
                _mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
            }
        }

        private void Start()
        {
            _cinemachineTargetYaw = CinemachineCameraTarget.transform.rotation.eulerAngles.y;
            
            _hasAnimator = TryGetComponent(out _animator);
            _controller = GetComponent<CharacterController>();
            _input = GetComponent<StarterAssetsInputs>();
#if ENABLE_INPUT_SYSTEM 
            _playerInput = GetComponent<PlayerInput>();
#else
			Debug.LogError( "Starter Assets package is missing dependencies. Please use Tools/Starter Assets/Reinstall Dependencies to fix it");
#endif

            AssignAnimationIDs();

            // reset our timeouts on start
            _jumpTimeoutDelta = JumpTimeout;
            _fallTimeoutDelta = FallTimeout;

            EventBus.AddListener<bool>(EventTypes.DisableInteraction, ChangeInteractStatus);
        }

        private void Update()
        {
            _hasAnimator = TryGetComponent(out _animator);

            JumpAndGravity();
            GroundedCheck();
            if(canMove)
                Move();
            InteractWithBox();
            
            //Vector2Int GridPos = GridSystem.Instance.WorldToGridPosition(transform.position);
            //Debug.Log("Player on Grid: " + GridPos);
        }

        private void LateUpdate()
        {
            CameraRotation();
        }

        private void AssignAnimationIDs()
        {
            _animIDSpeed = Animator.StringToHash("Speed");
            _animIDGrounded = Animator.StringToHash("Grounded");
            _animIDJump = Animator.StringToHash("Jump");
            _animIDFreeFall = Animator.StringToHash("FreeFall");
            _animIDMotionSpeed = Animator.StringToHash("MotionSpeed");
            _animIDPushPose = Animator.StringToHash("Interact");
            _animIDPushAnimation = Animator.StringToHash("isPushing");
            _animIDPullAnimation = Animator.StringToHash("isPulling");
        }

        private void GroundedCheck()
        {
            // set sphere position, with offset
            Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y - GroundedOffset,
                transform.position.z);
            Grounded = Physics.CheckSphere(spherePosition, GroundedRadius, GroundLayers,
                QueryTriggerInteraction.Ignore);

            // update animator if using character
            if (_hasAnimator)
            {
                _animator.SetBool(_animIDGrounded, Grounded);
            }
        }

        private void CameraRotation()
        {
            // if there is an input and camera position is not fixed
            if (_input.look.sqrMagnitude >= _threshold && !LockCameraPosition)
            {
                //Don't multiply mouse input by Time.deltaTime;
                float deltaTimeMultiplier = IsCurrentDeviceMouse ? 1.0f : Time.deltaTime;

                _cinemachineTargetYaw += _input.look.x * deltaTimeMultiplier;
                _cinemachineTargetPitch += _input.look.y * deltaTimeMultiplier;
            }

            // clamp our rotations so our values are limited 360 degrees
            _cinemachineTargetYaw = ClampAngle(_cinemachineTargetYaw, float.MinValue, float.MaxValue);
            _cinemachineTargetPitch = ClampAngle(_cinemachineTargetPitch, BottomClamp, TopClamp);

            // Cinemachine will follow this target
            CinemachineCameraTarget.transform.rotation = Quaternion.Euler(_cinemachineTargetPitch + CameraAngleOverride,
                _cinemachineTargetYaw, 0.0f);
        }

        private void Move()
        {
            
            // set target speed based on move speed, sprint speed and if sprint is pressed
            float targetSpeed = _input.sprint ? SprintSpeed : MoveSpeed;

            // a simplistic acceleration and deceleration designed to be easy to remove, replace, or iterate upon

            // note: Vector2's == operator uses approximation so is not floating point error prone, and is cheaper than magnitude
            // if there is no input, set the target speed to 0
            if (_input.move == Vector2.zero) targetSpeed = 0.0f;

            // a reference to the players current horizontal velocity
            float currentHorizontalSpeed = new Vector3(_controller.velocity.x, 0.0f, _controller.velocity.z).magnitude;

            float speedOffset = 0.1f;
            float inputMagnitude = _input.analogMovement ? _input.move.magnitude : 1f;

            // accelerate or decelerate to target speed
            if (currentHorizontalSpeed < targetSpeed - speedOffset ||
                currentHorizontalSpeed > targetSpeed + speedOffset)
            {
                // creates curved result rather than a linear one giving a more organic speed change
                // note T in Lerp is clamped, so we don't need to clamp our speed
                _speed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed * inputMagnitude,
                    Time.deltaTime * SpeedChangeRate);

                // round speed to 3 decimal places
                _speed = Mathf.Round(_speed * 1000f) / 1000f;
            }
            else
            {
                _speed = targetSpeed;
            }

            _animationBlend = Mathf.Lerp(_animationBlend, targetSpeed, Time.deltaTime * SpeedChangeRate);
            if (_animationBlend < 0.01f) _animationBlend = 0f;

            // normalise input direction
            Vector3 inputDirection = new Vector3(_input.move.x, 0.0f, _input.move.y).normalized;
            //Debug.Log("Input Dir :" + inputDirection);
            // note: Vector2's != operator uses approximation so is not floating point error prone, and is cheaper than magnitude
            // if there is a move input rotate player when the player is moving
            if (_input.move != Vector2.zero)
            {
                _targetRotation = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg +
                                  _mainCamera.transform.eulerAngles.y;
               
                float rotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, _targetRotation, ref _rotationVelocity,
                    RotationSmoothTime);

                // rotate to face input direction relative to camera position
                transform.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);
                //Debug.Log("Target Rotate:" + transform.rotation.eulerAngles.normalized);
            }


            Vector3 targetDirection = Quaternion.Euler(0.0f, _targetRotation, 0.0f) * Vector3.forward;

            // move the player
            _controller.Move(targetDirection.normalized * (_speed * Time.deltaTime) +
                             new Vector3(0.0f, _verticalVelocity, 0.0f) * Time.deltaTime);
            //_controller.Move(targetDirection.normalized * (_speed * Time.deltaTime));                 

            // update animator if using character
            if (_hasAnimator)
            {
                _animator.SetFloat(_animIDSpeed, _animationBlend);
                _animator.SetFloat(_animIDMotionSpeed, inputMagnitude);
            }
        }

        private void JumpAndGravity()
        {
            if (Grounded)
            {
                // reset the fall timeout timer
                _fallTimeoutDelta = FallTimeout;

                // update animator if using character
                if (_hasAnimator)
                {
                    _animator.SetBool(_animIDJump, false);
                    _animator.SetBool(_animIDFreeFall, false);
                }

                // stop our velocity dropping infinitely when grounded
                if (_verticalVelocity < 0.0f)
                {
                    _verticalVelocity = -2f;
                }

                // Jump
                if (_input.jump && _jumpTimeoutDelta <= 0.0f)
                {
                    // the square root of H * -2 * G = how much velocity needed to reach desired height
                    _verticalVelocity = Mathf.Sqrt(JumpHeight * -2f * Gravity);

                    // update animator if using character
                    if (_hasAnimator)
                    {
                        _animator.SetBool(_animIDJump, true);
                    }
                }

                // jump timeout
                if (_jumpTimeoutDelta >= 0.0f)
                {
                    _jumpTimeoutDelta -= Time.deltaTime;
                }
            }
            else
            {
                // reset the jump timeout timer
                _jumpTimeoutDelta = JumpTimeout;

                // fall timeout
                if (_fallTimeoutDelta >= 0.0f)
                {
                    _fallTimeoutDelta -= Time.deltaTime;
                }
                else
                {
                    // update animator if using character
                    if (_hasAnimator)
                    {
                        _animator.SetBool(_animIDFreeFall, true);
                    }
                }

                // if we are not grounded, do not jump
                _input.jump = false;
            }

            // apply gravity over time if under terminal (multiply by delta time twice to linearly speed up over time)
            if (_verticalVelocity < _terminalVelocity)
            {
                _verticalVelocity += Gravity * Time.deltaTime;
            }

            //_controller.Move(new Vector3(0.0f, _verticalVelocity, 0.0f) * Time.deltaTime);
        }
        
        private static float ClampAngle(float lfAngle, float lfMin, float lfMax)
        {
            if (lfAngle < -360f) lfAngle += 360f;
            if (lfAngle > 360f) lfAngle -= 360f;
            return Mathf.Clamp(lfAngle, lfMin, lfMax);
        }

        private void OnDrawGizmosSelected()
        {
            Color transparentGreen = new Color(0.0f, 1.0f, 0.0f, 0.35f);
            Color transparentRed = new Color(1.0f, 0.0f, 0.0f, 0.35f);

            if (Grounded) Gizmos.color = transparentGreen;
            else Gizmos.color = transparentRed;

            // when selected, draw a gizmo in the position of, and matching radius of, the grounded collider
            Gizmos.DrawSphere(
                new Vector3(transform.position.x, transform.position.y - GroundedOffset, transform.position.z),
                GroundedRadius);
        }

        private void OnFootstep(AnimationEvent animationEvent)
        {
            if (animationEvent.animatorClipInfo.weight > 0.5f)
            {
                if (FootstepAudioClips.Length > 0)
                {
                    var index = Random.Range(0, FootstepAudioClips.Length);
                    AudioSource.PlayClipAtPoint(FootstepAudioClips[index], transform.TransformPoint(_controller.center), FootstepAudioVolume);
                }
            }
        }

        private void OnLand(AnimationEvent animationEvent)
        {
            if (animationEvent.animatorClipInfo.weight > 0.5f)
            {
                AudioSource.PlayClipAtPoint(LandingAudioClip, transform.TransformPoint(_controller.center), FootstepAudioVolume);
            }
        }

        private void InteractWithBox()
        {
            if (!closeToBox||!Grounded||GameManager.Instance.camRoot.isRotating)// if mouse is not close to box or in the air or cam is rotating
            {
                return;
            }
            
            Vector2Int playerGridPos = GridSystem.Instance.WorldToGridPosition(transform.position);
            Box attachedBox = GameManager.Instance.GetPlayerAttachedBox();
            Vector2Int attachBoxGridPos = GridSystem.Instance.WorldToGridPosition(attachedBox.transform.position);
            //Debug.Log("Box Grid loc :" + attachBoxGridPos);
            Vector2 playerBoxDir = (attachBoxGridPos - playerGridPos);
            //Debug.Log("Player -> Box Dir :" + playerBoxDir);
            int CamRotateAngle = Convert.ToInt32(GameManager.Instance.camRoot.transform.rotation.eulerAngles.y);
            
            if (Input.GetKeyDown(KeyCode.F)&&!beginInteract)//enter interact mode
            {
                Debug.Log("Begin Interact");
                Debug.Log("Player Gird Pos :" + playerGridPos);
                Debug.Log("World Pos :" + GridSystem.Instance.GridToWorldPosition(playerGridPos));
                Quaternion adjustRot = Quaternion.FromToRotation(transform.forward, new Vector3(playerBoxDir.x,0,playerBoxDir.y));
                Vector3 adjustLoc = GridSystem.Instance.GridToWorldPosition(playerGridPos);
                adjustLoc += new Vector3(playerBoxDir.x,0,playerBoxDir.y) * (GridSystem.Instance.cellSize/2 - _controller.radius);
                //adjust player facing
                transform.DORotate(transform.rotation.eulerAngles + adjustRot.eulerAngles, 1);
                transform.DOMove(adjustLoc, 1);
                ChangeInteractStatus(true);
                //hide interaction hint
                EventBus.Broadcast(EventTypes.ShowInteractHint,transform.position + new Vector3(0,2,0),false);
            }else if (Input.GetKeyDown(KeyCode.F) && beginInteract)//exit interact mode
            {
                ChangeInteractStatus(false);
            }

            if (beginInteract)
            {
                //Debug.Log("Cam Rotate Offset -> " + CamRotateAngle);
                normalizeInput = RotateVector(_input.move, CamRotateAngle);
                //Debug.Log("Player origin Input:" + _input.move);
                //Debug.Log("Player normalize Input:" + normalizeInput);
                if (playerBoxDir == normalizeInput)//push
                {
                    if (!isInteracting)
                    {
                        ChangePushStatus(true);
                        // TODO: Actual Movement
                        Direction boxPushDir = ComputeDirBasedOnVector(normalizeInput);
                        Vector3 playerMoveDir = new Vector3(normalizeInput.x, 0, normalizeInput.y);
                        //Box Push
                        if (attachedBox.Move(boxPushDir))// if box able to move
                        {
                             //Player Move
                             transform.DOMove(transform.position + playerMoveDir, 1).OnComplete(() =>
                             {
                                 ResetInteractingStatus();
                             });
                        }
                        else
                        {
                            ChangePushStatus(false);
                        }
                        
                        Debug.Log("Push:" +boxPushDir);
                    }
                    
                }else if (-1 * playerBoxDir == normalizeInput)//pull
                {
                    
                    if (!isInteracting)
                    {
                        ChangePullStatus(true);
                        // TODO: Actual Movement
                        Direction boxPullDir = ComputeDirBasedOnVector(normalizeInput);
                        Vector3 playerMoveDir = new Vector3(normalizeInput.x, 0, normalizeInput.y);
                        //Box Push
                        RaycastHit hit;
                        Ray castingRay = new Ray(transform.position,new Vector3(normalizeInput.x,0,normalizeInput.y));
                        if (!Physics.Raycast(castingRay, out hit,maxDistance:GridSystem.Instance.cellSize))// if player has enough space to back
                        {
                            attachedBox.Move(boxPullDir);
                            //Player Move
                            transform.DOMove(transform.position + playerMoveDir, 1).OnComplete(() =>
                            {
                                ResetInteractingStatus();
                            });
                        }
                        else
                        {
                            ChangePullStatus(false);
                        }
                    }
                }
            }
        }

        private void ResetInteractingStatus()
        {
            Debug.Log("Finish");
            isInteracting = false;
            _animator.SetBool(_animIDPushAnimation, false);
            _animator.SetBool(_animIDPullAnimation, false);
        }
        private void ChangeInteractStatus(bool canInteract)// change anim status of interact
        {
            
            beginInteract = canInteract;
            canMove = !canInteract;
            _animator.SetBool(_animIDPushPose,beginInteract);
        }

        private void ChangePushStatus(bool pushing)//change push anim status
        {
            isInteracting = pushing;
            _animator.SetBool(_animIDPushAnimation, pushing);
        }

        private void ChangePullStatus(bool pulling)//change pull anim status
        {
            isInteracting = pulling;
            _animator.SetBool(_animIDPullAnimation, pulling);
        }
        
        private void OnTriggerEnter(Collider other)
        {
            
            if (other.CompareTag("Box"))
            {
                if (isInteracting)
                {
                    return;
                }
                RaycastHit hit;
                Vector3 playerFacing = transform.forward;
                Ray castingRay = new Ray(transform.position, playerFacing);
                if (Physics.Raycast(castingRay, out hit))
                {
                    closeToBox = true;
                    EventBus.Broadcast(EventTypes.RegisterPlayerInteractBox,other.GetComponent<Box>());
                    EventBus.Broadcast(EventTypes.ShowInteractHint,transform.position + new Vector3(0,2,0),true);
                    //TODO Show Interaction UI button
                }
                
                
            }
        }

        private void OnTriggerExit(Collider other)//reset when player is not touching box anymore
        {
            Debug.LogWarning("Away From Box");
            if (!isInteracting)
            {
                closeToBox = false;
                beginInteract = false;
                EventBus.Broadcast(EventTypes.ClearPlayerInteractBox);
                EventBus.Broadcast(EventTypes.ShowInteractHint,transform.position + new Vector3(0,2,0),false);
            }
            
        }

        private Direction ComputeDirBasedOnVector(Vector2 dir)
        {
            if (dir.x != 0 && Mathf.Abs(dir.x) > Mathf.Abs(dir.y))
            {
                if (dir.x < 0)
                    return Direction.N;
                
                return Direction.S;
            }
            else if (dir.y != 0 && Mathf.Abs(dir.y) > Mathf.Abs(dir.x))
            {
                if (dir.y < 0)
                    return Direction.W;
                
                return Direction.E;
            }
            else
            {
                return Direction.None;
            }
        }
        public Vector2 RotateVector(Vector2 v, float angle)
        {
            float radian = angle*Mathf.Deg2Rad;
            float _x = v.x*Mathf.Cos(radian) + v.y*Mathf.Sin(radian);
            float _y = -v.x*Mathf.Sin(radian) + v.y*Mathf.Cos(radian);
            return new Vector2(_x,_y);
        }
        
        void OnDrawGizmos() {
            Gizmos.color = new Color(1, 0, 0, 0.5F);
            
            Ray debugRay = new Ray(transform.position + new Vector3(0,2,0), new Vector3(0,0,-1));
            Gizmos.DrawRay(debugRay);
            Gizmos.DrawCube(transform.position + new Vector3(0,0,-1),new Vector3(0.5f,0.5f,0.5f));
        }
    }
    
    
}