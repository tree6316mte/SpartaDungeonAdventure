using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

public class PlayerController : MonoBehaviour
{
    [Header("Moving")]
    public float walklSpeed = 2.0f;
    public float runSpeed = 6.0f;
    public float RotationSmoothTime = 0.12f;
    public float SpeedChangeRate = 10.0f;

    // Moving
    float _currentSpeed;
    float _animationBlend;
    Vector3 _targetDirection;
    float _targetRotation = 0.0f;
    float _rotationVelocity;


    [Header("Jumping")]
    public float airSpeed = 5f;
    public float airSmooth = 6f;
    public float jumpForce = 4f;
    public float jumpTimer = 0.3f;
    public float jumpDelay = 0.15f;
    public bool stopMove { get; protected set; }

    // Jumping
    [SerializeField] float _jumpCounter;
    [SerializeField] bool _isJumping = false;


    [Header("Camera")]
    public bool isLockCamera = false;
    public float topClamp = 70.0f;
    public float bottomClamp = -30.0f;

    // Camera
    float _cinemachineTargetYaw;
    float _cinemachineTargetPitch;
    const float _threshold = 0.01f;


    [Header("Slope Check")]
    [Range(30, 80)] public float slopeLimit = 45f;


    [Header("Ground Check")]
    public float groundMinDistance = 0.25f;
    public float groundMaxDistance = 0.5f;
    public float extraGravity = -10f;
    RaycastHit _groundHit;
    float _heightReached;                       // max height that character reached in air;
    float _groundDistance;                      // used to know the distance from the ground
    [SerializeField] bool _isGrounded;
    float _jumpTimeoutDelta;


    [Header("Components")]
    #region Components
    public LayerMask groundLayer = 1 << 0;
    public GameObject cinemachineCameraTarget;
    public PlayerAnimHandler animHandler;
    GameObject _mainCamera;
    PlayerInputHandler _inputHandler;
    CapsuleCollider _collider;
    Rigidbody _rigidbody;
    PhysicMaterial _frictionPhysics, _maxFrictionPhysics, _slippyPhysics;
    #endregion

    void Start()
    {
        _mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
        _inputHandler = GetComponent<PlayerInputHandler>();
        _collider = GetComponent<CapsuleCollider>();
        _rigidbody = GetComponent<Rigidbody>();

        // slides the character through walls and edges
        _frictionPhysics = new PhysicMaterial();
        _frictionPhysics.name = "frictionPhysics";
        _frictionPhysics.staticFriction = .25f;
        _frictionPhysics.dynamicFriction = .25f;
        _frictionPhysics.frictionCombine = PhysicMaterialCombine.Multiply;

        // prevents the collider from slipping on ramps
        _maxFrictionPhysics = new PhysicMaterial();
        _maxFrictionPhysics.name = "maxFrictionPhysics";
        _maxFrictionPhysics.staticFriction = 1f;
        _maxFrictionPhysics.dynamicFriction = 1f;
        _maxFrictionPhysics.frictionCombine = PhysicMaterialCombine.Maximum;

        // air physics 
        _slippyPhysics = new PhysicMaterial();
        _slippyPhysics.name = "slippyPhysics";
        _slippyPhysics.staticFriction = 0f;
        _slippyPhysics.dynamicFriction = 0f;
        _slippyPhysics.frictionCombine = PhysicMaterialCombine.Minimum;

        // reset our timeouts on start
        _jumpTimeoutDelta = jumpTimer;
    }

    void Update()
    {
        Moving();
        Jumping();
    }

    void FixedUpdate()
    {
        CheckGround();
        CheckSlopeLimit();
        ControlJumpBehaviour();
        AirControl();
    }

    void LateUpdate()
    {
        CameraRotation();
    }

    #region Moving

    void Moving()
    {
        // 목표 속도 설정
        float targetSpeed = _inputHandler.sprint ? runSpeed : walklSpeed;
        if (_inputHandler.move == Vector2.zero || stopMove) targetSpeed = 0.0f;

        // 현재 속도가 목표 속도를 향해 점점 증가
        float speedOffset = 0.1f; // 0.1f 씩 증가
        float inputMagnitude = _inputHandler.move.magnitude;
        inputMagnitude = inputMagnitude == 0 ? 1f : inputMagnitude;

        // 속도 제한
        if (_currentSpeed < targetSpeed - speedOffset ||
            _currentSpeed > targetSpeed + speedOffset)
        {
            _currentSpeed = Mathf.Lerp(_currentSpeed, targetSpeed * inputMagnitude, Time.deltaTime * SpeedChangeRate);
            _currentSpeed = Mathf.Round(_currentSpeed * 1000f) / 1000f;
        }
        else
        {
            _currentSpeed = targetSpeed;
        }

        // 애니메이션 속도 점점 증가
        _animationBlend = Mathf.Lerp(_animationBlend, targetSpeed, Time.deltaTime * SpeedChangeRate);
        if (_animationBlend < 0.01f) _animationBlend = 0f;

        // 입력된 값의 normal벡터
        Vector3 inputDirection = new Vector3(_inputHandler.move.x, 0.0f, _inputHandler.move.y).normalized;

        // 입력된 값을 카메라 벡터로 회전 및 캐릭터도 회전
        if (_inputHandler.move != Vector2.zero)
        {
            _targetRotation = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg + _mainCamera.transform.eulerAngles.y;
            float rotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, _targetRotation, ref _rotationVelocity, RotationSmoothTime);

            transform.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);
        }

        _targetDirection = Quaternion.Euler(0.0f, _targetRotation, 0.0f) * Vector3.forward;
        Vector3 dir = _targetDirection.normalized * _currentSpeed;
        dir.y = _rigidbody.velocity.y;  // y값은 velocity(변화량)의 y 값을 넣어준다.

        _rigidbody.velocity = dir;  // 연산된 속도를 velocity(변화량)에 넣어준다.
        animHandler?.PlayMove(_animationBlend, inputMagnitude);
    }
    #endregion

    #region Jumping
    void Jumping()
    {
        if (_isGrounded && GroundAngle() < slopeLimit && !_isJumping && !stopMove)
        {
            // Jump
            if (_inputHandler.jump && _jumpTimeoutDelta <= 0.0f)
            {
                _jumpCounter = jumpTimer;
                _isJumping = true;
                animHandler?.PlayJump(true);
            }
            else
            {
                _inputHandler.jump = false;
            }


            // jump timeout
            if (_jumpTimeoutDelta >= 0.0f)
            {
                _jumpTimeoutDelta -= Time.deltaTime;
            }
        }
        else
        {
            _jumpTimeoutDelta = jumpDelay;

            _inputHandler.jump = false;
        }
    }

    void ControlJumpBehaviour()
    {
        if (!_isJumping) return;

        _jumpCounter -= Time.deltaTime;
        if (_jumpCounter <= 0)
        {
            _jumpCounter = 0;
            _isJumping = false;
            animHandler?.PlayJump(false);
        }
        // apply extra force to the jump height   
        var vel = _rigidbody.velocity;
        vel.y = jumpForce;
        _rigidbody.velocity = vel;
    }

    void AirControl()
    {
        if (_isGrounded && !_isJumping) return;

        if (transform.position.y > _heightReached) _heightReached = transform.position.y;

        if (!_isGrounded)
        {
            _rigidbody.AddForce(_targetDirection * airSpeed * Time.deltaTime, ForceMode.VelocityChange);
            return;
        }

        _targetDirection.y = 0;
        _targetDirection.x = Mathf.Clamp(_targetDirection.x, -1f, 1f);
        _targetDirection.z = Mathf.Clamp(_targetDirection.z, -1f, 1f);

        Vector3 targetPosition = _rigidbody.position + (_targetDirection * airSpeed) * Time.deltaTime;
        Vector3 targetVelocity = (targetPosition - transform.position) / Time.deltaTime;

        targetVelocity.y = _rigidbody.velocity.y;
        _rigidbody.velocity = Vector3.Lerp(_rigidbody.velocity, targetVelocity, airSmooth * Time.deltaTime);
    }

    #endregion

    #region Camera Rotation
    // 3인칭 카메라 회전
    void CameraRotation()
    {
        if (_inputHandler.look.sqrMagnitude >= _threshold && !isLockCamera)
        {
            float deltaTimeMultiplier = 1.0f;

            _cinemachineTargetYaw += _inputHandler.look.x * deltaTimeMultiplier;
            _cinemachineTargetPitch += _inputHandler.look.y * deltaTimeMultiplier;
        }

        _cinemachineTargetYaw = ClampAngle(_cinemachineTargetYaw, float.MinValue, float.MaxValue);
        _cinemachineTargetPitch = ClampAngle(_cinemachineTargetPitch, bottomClamp, topClamp);

        cinemachineCameraTarget.transform.rotation = Quaternion.Euler(_cinemachineTargetPitch, _cinemachineTargetYaw, 0.0f);
    }
    float ClampAngle(float lfAngle, float lfMin, float lfMax)
    {
        if (lfAngle < -360f) lfAngle += 360f;
        if (lfAngle > 360f) lfAngle -= 360f;
        return Mathf.Clamp(lfAngle, lfMin, lfMax);
    }
    #endregion


    #region Slope Check
    void CheckSlopeLimit()
    {
        if (_inputHandler.move.sqrMagnitude < 0.1) return;

        RaycastHit hitinfo;
        var hitAngle = 0f;

        // 왼쪽, 가운데, 오른쪽 3방향의 확인을 위함
        for (int i = -1; i <= 1; i++)
        {
            var margin = transform.right * (0.2f * i);
            if (Physics.Linecast(transform.position + margin + Vector3.up * (_collider.height * 0.5f), transform.position + margin + _targetDirection.normalized * (_collider.radius + 0.2f), out hitinfo, groundLayer))
            {
                hitAngle = Vector3.Angle(Vector3.up, hitinfo.normal);

                var targetPoint = hitinfo.point + _targetDirection.normalized * _collider.radius;
                if ((hitAngle > slopeLimit) && Physics.Linecast(transform.position + margin + Vector3.up * (_collider.height * 0.5f), targetPoint, out hitinfo, groundLayer))
                {
                    hitAngle = Vector3.Angle(Vector3.up, hitinfo.normal);

                    if (hitAngle > slopeLimit && hitAngle < 85f)
                    {
                        stopMove = true;
                        return;
                    }
                }
            }
        }

        stopMove = false;
    }
    #endregion


    #region Ground Check

    void CheckGround()
    {
        CheckGroundDistance();
        ControlMaterialPhysics();

        if (_groundDistance <= groundMinDistance)
        {
            _isGrounded = true;
            animHandler.PlayFreeFall(false);
            if (!_isJumping && _groundDistance > 0.05f)
                _rigidbody.AddForce(transform.up * (extraGravity * 2 * Time.deltaTime), ForceMode.VelocityChange);

            _heightReached = transform.position.y;
        }
        else
        {
            if (_groundDistance >= groundMaxDistance)
            {
                _isGrounded = false;
                animHandler.PlayFreeFall(true);
                if (!_isJumping)
                {
                    _rigidbody.AddForce(transform.up * extraGravity * Time.deltaTime, ForceMode.VelocityChange);
                }
            }
            else if (!_isJumping)
            {
                _rigidbody.AddForce(transform.up * (extraGravity * 2 * Time.deltaTime), ForceMode.VelocityChange);
            }
        }
        animHandler.SetGrounded(_isGrounded);
    }

    void ControlMaterialPhysics()
    {
        _collider.material = (_isGrounded && GroundAngle() <= slopeLimit + 1) ? _frictionPhysics : _slippyPhysics;

        if (_isGrounded && _inputHandler.move == Vector2.zero)
            _collider.material = _maxFrictionPhysics;
        else if (_isGrounded && _inputHandler.move != Vector2.zero)
            _collider.material = _frictionPhysics;
        else
            _collider.material = _slippyPhysics;
    }

    void CheckGroundDistance()
    {
        if (_collider != null)
        {
            float radius = _collider.radius * 0.9f;
            var dist = 10f;

            Ray ray2 = new Ray(transform.position + new Vector3(0, _collider.height / 2, 0), Vector3.down);

            if (Physics.Raycast(ray2, out _groundHit, (_collider.height / 2) + dist, groundLayer) && !_groundHit.collider.isTrigger)
                dist = transform.position.y - _groundHit.point.y;

            if (dist >= groundMinDistance)
            {
                Vector3 pos = transform.position + Vector3.up * (_collider.radius);
                Ray ray = new Ray(pos, -Vector3.up);
                if (Physics.SphereCast(ray, radius, out _groundHit, _collider.radius + groundMaxDistance, groundLayer) && !_groundHit.collider.isTrigger)
                {
                    Physics.Linecast(_groundHit.point + (Vector3.up * 0.1f), _groundHit.point + Vector3.down * 0.15f, out _groundHit, groundLayer);
                    float newDist = transform.position.y - _groundHit.point.y;
                    if (dist > newDist) dist = newDist;
                }
            }
            _groundDistance = (float)System.Math.Round(dist, 2);
        }
    }

    float GroundAngle()
    {
        var groundAngle = Vector3.Angle(_groundHit.normal, Vector3.up);
        return groundAngle;
    }
    #endregion
}
