using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    private StateMachine movementSM;

    public StandingState Standing { get; private set; }
    public CrouchingState Crouching { get; private set; }
    public JumpingState JumpingState { get; private set; }
    public SlidingState Sliding { get; private set; }
    public VaultingState Vaulting { get; private set; }
    public ClimbingState Climbing { get; private set; }
    public DodgingState Dodging { get; private set; }
    public WallRunningState WallRunning { get; private set; }

    [SerializeField] private float runningSpeed = 10.0f;
    public float RunningSpeed { get => runningSpeed; }

    [SerializeField] private float crouchingSpeed = 5.0f;
    public float CrouchingSpeed { get => crouchingSpeed; }

    [SerializeField] private float standingColliderHeight = 2.0f;
    public float StandingColliderHeight { get => standingColliderHeight; }

    [SerializeField] private float crouchingColliderHeight = 1.0f;
    public float CrouchingColliderHeight { get => crouchingColliderHeight; }

    [SerializeField] private float jumpSpeed = 15.0f;
    public float JumpSpeed { get => jumpSpeed; }

    [SerializeField] private float gravity = -9.8f;
    public float Gravity { get => gravity; }

    [SerializeField] private float slidingSpeed = 10.0f;
    public float SlidingSpeed { get => slidingSpeed; }

    [SerializeField] private float slidingTime = 1.0f;
    public float SlidingTime { get => slidingTime; }

    [SerializeField] private float obstacleCheckDistance = 0.8f;
    public float ObstacleCheckDistance { get => obstacleCheckDistance; }

    [SerializeField] private float vaultingObstacleMaxDepth = 0.5f;
    public float VaultingObstacleMaxDepth { get => vaultingObstacleMaxDepth; }

    [SerializeField] private float standToClimbObstacleMaxHeight = 3.5f;
    public float StandToClimbObstacleMaxHeight { get => standToClimbObstacleMaxHeight; }

    [SerializeField] private float jumpToClimbObstacleMaxHeight = 2.5f;
    public float JumpToClimbObstacleMaxHeight { get => jumpToClimbObstacleMaxHeight; }

    [SerializeField] private float dodgingSpeed = 15.0f;
    public float DodgingSpeed { get => dodgingSpeed; }

    [SerializeField] private float dodgingTime = 0.25f;
    public float DodgingTime { get => dodgingTime; }

    [SerializeField] private float wallCheckDistance = 0.3f;
    public float WallCheckDistance { get => wallCheckDistance; }

    [SerializeField] private float wallRunningGravity = -2.45f;
    public float WallRunningGravity { get => wallRunningGravity; }

    [SerializeField] private float vaultingSpeed = 10.0f;
    public float VaultingSpeed { get => vaultingSpeed; }

    [SerializeField] private float climbingSpeed = 10.0f;
    public float ClimbingSpeed { get => climbingSpeed; }

    [SerializeField] private float standingCamLocalHeight = 0.7f;
    [SerializeField] private float crouchingCamLocalHeight = 0.35f;

    public CharacterController CharacterController { get; private set; }
    public ControllerColliderHit Contact { get; private set; }

    private Camera cam;

    // Start is called before the first frame update
    void Start()
    {
        cam = GetComponentInChildren<Camera>();
        
        CharacterController = GetComponent<CharacterController>();

        movementSM = new StateMachine();

        Standing = new StandingState(this, movementSM);
        Crouching = new CrouchingState(this, movementSM);
        JumpingState = new JumpingState(this, movementSM);
        Sliding = new SlidingState(this, movementSM);
        Vaulting = new VaultingState(this, movementSM);
        Climbing = new ClimbingState(this, movementSM);
        Dodging = new DodgingState(this, movementSM);
        WallRunning = new WallRunningState(this, movementSM);

        movementSM.Initialize(Standing);
    }

    // Update is called once per frame
    void Update()
    {
        movementSM.CurrentState.HandleInput();
      
        movementSM.CurrentState.LogicUpdate();
    }

    private void FixedUpdate()
    {
        movementSM.CurrentState.PhysicsUpdate();
    }

    public void SetCameraZRotation(float angle)
    {
        Vector3 euler = cam.transform.localEulerAngles;
        cam.transform.localEulerAngles = new Vector3(euler.x, euler.y, angle);
    }

    public void SetCrouchingHeight()
    {
        CharacterController.height = CrouchingColliderHeight;
        cam.transform.localPosition = new Vector3(0, crouchingCamLocalHeight, 0);
    }

    public void SetStandingHeight()
    {
        CharacterController.height = StandingColliderHeight;
        cam.transform.localPosition = new Vector3(0, standingCamLocalHeight, 0);
    }

    public bool IsCloseToWall(out Vector3 wallNormal, out bool? wallToLeft)
    {
        wallNormal = default;
        wallToLeft = null;

        RaycastHit hit;
        if (Physics.Raycast(transform.position, -transform.right, out hit, CharacterController.radius + WallCheckDistance) )
        {
            if (hit.transform.GetComponent<BoxCollider>() != null &&
                hit.transform.rotation.eulerAngles.x == 0 &&
                hit.transform.rotation.eulerAngles.z == 0)
            {
                wallToLeft = true;
                wallNormal = hit.normal;
                return true;
            }
        }

        if (Physics.Raycast(transform.position, transform.right, out hit, CharacterController.radius + WallCheckDistance))
        {
            if (hit.transform.GetComponent<BoxCollider>() != null &&
                hit.transform.rotation.eulerAngles.x == 0 &&
                hit.transform.rotation.eulerAngles.z == 0)
            {
                wallToLeft = false;
                wallNormal = hit.normal;
                return true;
            }
        }

        return false;
    }

    public bool IsCloseToClimbingObstacle(out BoxCollider obstacle)
    {
        obstacle = null;

        int step = cam.pixelHeight / 4;
        Vector3 point, origin;
        RaycastHit hit;
        Ray ray;
        BoxCollider collider; 
        int mask = ~(1 << 8); 
        for (int i = 0; i < cam.pixelHeight; i += step)
        {
            point = new Vector3(cam.pixelWidth / 2, i, 0);
            ray = cam.ScreenPointToRay(point);
            if (Physics.Raycast(ray, out hit))
            {
                collider = hit.transform.GetComponent<BoxCollider>();

                if (collider == null)
                    continue;

                if (hit.transform.rotation.eulerAngles.x == 0 && hit.transform.rotation.eulerAngles.z == 0)
                {
                    if (hit.distance <= ObstacleCheckDistance)
                    {
                        origin = transform.position;
                        origin.y = collider.bounds.max.y + CrouchingColliderHeight / 2 + 0.1f;

                        if (!Physics.CheckCapsule(transform.position + Vector3.up * 0.1f, origin, CharacterController.radius, mask))
                        {
                            origin = transform.position + 2 * CharacterController.radius * transform.forward;
                            origin.y = collider.bounds.max.y + CrouchingColliderHeight / 2 + 0.1f;

                            if (!Physics.CheckSphere(origin, CharacterController.radius, mask))
                            {
                                obstacle = collider;
                                return true;
                            }
                        }
                    }
                }
            }
        }

        return false;
    }

    void OnGUI()
    {
        int size = 12;
        float posX = cam.pixelWidth / 2 - size / 4;
        float posY = cam.pixelHeight / 2 - size / 2;
        GUI.Label(new Rect(posX, posY, size, size), "*");
    }

    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        Contact = hit;
    }
}
