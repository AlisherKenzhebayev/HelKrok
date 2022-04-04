using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    //**********
    //  PUBLIC
    //**********

    [Header("References")]
    [Tooltip("Camera follow transform")]
    public Transform playerCamera;
    public Transform groundCheck;
    public Transform grappleSpawn;
    public GameObject chainLinkPrefab;
    public float distanceSpawnLinks = 0.5f;
    public GameObject uiPrefab;

    [Header("Debug Materials")]
    public Material debugGoodStateMaterial;
    public Material debugQuoStateMaterial;
    public Material debugBadStateMaterial;
    public GameObject debugObject;

    [Header("Rotation")]
    [Tooltip("Rotation speed X for moving the camera")]
    public float horizontalRotationSpeed = 2.2f;
    [Tooltip("Rotation speed Y for moving the camera")]
    public float verticalRotationSpeed = 2.2f;

    [Header("Basic Movement")]
    [Tooltip("Ground speed")]
    public float groundSpeed = 10f;
    [Tooltip("Jump force")]
    public float jumpForce = 300f;
    [Tooltip("Super Jump force")]
    public float superJumpForce = 600f;
    [Tooltip("Superjump timing window (s)")]
    public float superJumpAllow = 0.2f;
    [Tooltip("Backmove dampening")]
    [Range(0f, 1f)]
    public float backwardsSpeedCoef = 0.15f;

    [Header("Grapple Movement")]
    [Tooltip("Grapple force")]
    public float grappleForce = 2000f;
    [Tooltip("Grapple speed time")]
    public float grappleMaxTime = 3f;
    [Tooltip("Speed curve")]
    public AnimationCurve speedCurve;

    [Header("Air Movement")]
    [Tooltip("Air strafe force")]
    public float airStrafeForce = 100f;
    [Tooltip("Max air speed")]
    public float maxAirSpeed = 5f;
    [Tooltip("Air force effect curve")]
    public AnimationCurve airForceEffectCurve;

    //**********
    //  PRIVATE
    //**********

    private List <GameObject>  links;

    private bool isTethered = false;
    private bool hasJumpInput = false;

    private float timeGrappledSince;
    private float timeSuperJumpSince;
    private float tetherLength;
    private Vector3 tetherPoint;

    private ICommand jumpCommand;
    private ICommand timedJumpCommand;
    private ICommand moveCommand;
    private ICommand airStrafeCommand;

    private LineRenderer lr;
    private Rigidbody rb;
    private float m_CameraVerticalAngle = 0f;
    
    private InputManager inputManager;
    private float xMouseInput;
    private float yMouseInput;
    private Vector3 worldspaceMoveInput;

    private MeshRenderer debugRenderer;

    // Start is called before the first frame update
    void Start()
    {
        links = new List<GameObject>();

        Instantiate(uiPrefab, new Vector3(0, 0, 0), Quaternion.identity);

        CursorMagick();

        inputManager = new InputManager();

        rb = this.GetComponent<Rigidbody>();
        if (rb == null)
        {
            Debug.LogError("Error - no RigidBody component");
        }

        lr = this.GetComponent<LineRenderer>();
        if (lr == null)
        {
            Debug.LogError("Error - no LineRenderer component");
        }

        debugRenderer = debugObject.GetComponent<MeshRenderer>();
        if (debugRenderer == null)
        {
            Debug.LogError("Error - no MeshRenderer component");
        }

        // Define commands here
        jumpCommand = new JumpCommand(rb).SetJumpForce(jumpForce);
        moveCommand = new MoveCommand(rb).SetGroundSpeed(groundSpeed).SetBackwardsSpeedCoef(backwardsSpeedCoef);
        timedJumpCommand = new TimedJumpCommand(rb).SetJumpForce(superJumpForce);
        airStrafeCommand = new AirStrafeCommand(rb).SetAirStrafeForce(airStrafeForce).SetMaxAirSpeed(maxAirSpeed).SetAirForceEffectCurve(airForceEffectCurve);
    }

    // Update is called once per frame
    void Update()
    {
        HandleGrappleInput();
        DebugGrappleWithMaterial();

        VisualizeGrapple(false);

        HandleCameraInput();

        HandleInput();
    }

    private void VisualizeGrapple(bool simpleVis)
    {
        if (isTethered)
        {
            if (simpleVis)
            {
                lr.SetPosition(0, grappleSpawn.position);
                lr.SetPosition(1, tetherPoint);
            }
            else
            {
                // Spawn in chain links
                float totalDist = Vector3.Distance(grappleSpawn.position, tetherPoint);
                Vector3 directionToGrapple = Vector3.Normalize(tetherPoint - grappleSpawn.position);
                float numberOfSpawn = Mathf.RoundToInt(totalDist / distanceSpawnLinks);
                totalDist -= totalDist % distanceSpawnLinks;
                float distance = totalDist / numberOfSpawn;
                float distValue = 0;
                for (int i = 0; i < Math.Max(links.Count, numberOfSpawn); i++)
                {
                    if (i >= numberOfSpawn)
                    {
                        if(links[i] != null) { 
                            GameObject.Destroy(links[i]);
                            links[i] = null;
                        }
                        continue;
                    }

                    //We increase our lerpValue
                    distValue += distance;
                    float lerpValue = precisionFloat(distValue / totalDist);
                    //Get the position
                    Vector3 placePosition = Vector3.Lerp(grappleSpawn.position, grappleSpawn.position + directionToGrapple * totalDist, lerpValue);
                    if (links.Count > i && links[i] != null)
                    {
                        links[i].transform.position = placePosition;
                        links[i].transform.rotation = Quaternion.Lerp(playerCamera.transform.rotation, grappleSpawn.rotation, lerpValue);
                        //links[i].transform.localScale = chainLinkPrefab.transform.lossyScale;
                    }
                    else
                    {
                        //Instantiate the object
                        links.Add(Instantiate(chainLinkPrefab, placePosition, playerCamera.transform.rotation, transform.parent));
                    }
                }
            }
        }
        else
        {
            if (!simpleVis)
            {
                for (int i = 0; i < links.Count; i++)
                {
                    GameObject.Destroy(links[i]);
                }
                links.Clear();
            }
        }
    }

    private void FixedUpdate()
    {
        // Update grapple timer up to grappleMaxTime
        if (isTethered) {
            timeGrappledSince = Mathf.Clamp(timeGrappledSince + Time.fixedDeltaTime, 0, grappleMaxTime);
        }
        // Update timer superjump each update
        timeSuperJumpSince = Mathf.Clamp(timeSuperJumpSince - Time.fixedDeltaTime, -1f, superJumpAllow);

        ApplyMovePhysics();

        ApplyJumpPhysics();
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(playerCamera.position, playerCamera.forward);

        Gizmos.color = Color.green;
        Gizmos.DrawRay(this.transform.position, worldspaceMoveInput);

        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(tetherPoint, 0.5f);
    }

    //************
    //  INPUTS
    //************

    private void HandleInput()
    {
        // Handle keyboard movement
        worldspaceMoveInput = transform.TransformVector(inputManager.GetMoveInput());    

        // Handle jump keys
        if (inputManager.GetJumping())
        {
            if (!hasJumpInput)
            {
                BeginJump();
            }
        }
    }

    private void HandleGrappleInput()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            if (!isTethered)
            {
                BeginGrapple();
            }
        }

        if (Input.GetKeyUp(KeyCode.Mouse0))
        {
            if (isTethered)
            {
                EndGrapple();
            }
        }
    }

    private void HandleCameraInput()
    {
        // Horizontal mouse inputs
        {
            xMouseInput = inputManager.getMouseHorizontal();
            transform.Rotate(new Vector3(0f, xMouseInput * horizontalRotationSpeed, 0f), Space.Self);
        }

        // Vertical mouse inputs
        {
            yMouseInput = inputManager.getMouseVertical();
            m_CameraVerticalAngle -= yMouseInput * verticalRotationSpeed;

            m_CameraVerticalAngle = Mathf.Clamp(m_CameraVerticalAngle, -89f, 89f);

            playerCamera.transform.localEulerAngles = new Vector3(m_CameraVerticalAngle, 0f, 0f);
        }
    }

    //************
    //  PHYSICS
    //************

    private void ApplyMovePhysics() {
        if (this.isTethered)
        {
            ApplyGrapplePhysics();
        }
        else
        {
            // Not attached to a tether
            if (isGrounded())
            {
                // Handle Movement on ground
                moveCommand.execute(worldspaceMoveInput);
            }
            else
            {
                // Air strafing
                airStrafeCommand.execute(worldspaceMoveInput);
            }
        }
    }

    private void ApplyJumpPhysics()
    {
        if (hasJumpInput)
        {
            if (superJumpAvailable())
            {
                timedJumpCommand.execute(worldspaceMoveInput);
                EndJump();
                return;
            }
            if (isGrounded())
            {
                jumpCommand.execute(worldspaceMoveInput);
                EndJump();
                return;
            }
        }
        EndJump();
        return;
    }

    private void ApplyGrapplePhysics()
    {
        Vector3 directionToGrapple = Vector3.Normalize(tetherPoint - transform.position);
        float currentDistanceToGrapple = Vector3.Distance(tetherPoint, transform.position);
        float speedTowardsGrapplePoint = precisionFloat(Vector3.Dot(rb.velocity, directionToGrapple));

        //Detach grapple if >90 angle
        {
            //Debug.Log(Vector3.Dot(playerCamera.transform.forward.normalized, directionToGrapple));
            if (Vector3.Dot(playerCamera.transform.forward.normalized, directionToGrapple) < 0) {
                EndGrapple();
            }
        }

        if (currentDistanceToGrapple > tetherLength)
        {
            if (speedTowardsGrapplePoint < 0)
            {
                // If moves away from the grapple, cancel out that portion of the velocity
                rb.position = tetherPoint - directionToGrapple * tetherLength;
                rb.velocity -= speedTowardsGrapplePoint * directionToGrapple;
            }
        }

        tetherLength = currentDistanceToGrapple;

        float curveMod = speedCurve.Evaluate(precisionFloat(timeGrappledSince / grappleMaxTime));
        Debug.Log(curveMod);
        rb.AddForce(directionToGrapple * grappleForce * curveMod, ForceMode.Force);
    }

    //************
    //  HELPER
    //************

    private float precisionFloat(float fValue)
    {
        return Mathf.Round(fValue * 1000f) / 1000f;
    }

    private void DebugGrappleWithMaterial()
    {
        if (superJumpAvailable()) {
            debugRenderer.material = debugQuoStateMaterial;
            return;
        }

        if (this.isTethered)
        {
            debugRenderer.material = debugGoodStateMaterial;
        }
        else
        {
            debugRenderer.material = debugBadStateMaterial;
        }
    }

    private bool isGrounded()
    {
        if (Physics.Raycast(groundCheck.position, -transform.up, 0.5f))
        {
            return true;
        }
        return false;
    }

    private bool superJumpAvailable()
    {
        return timeSuperJumpSince >= 0f && isGrounded();
    }

    private void CursorMagick()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void BeginGrapple()
    {
        if (Physics.Raycast(playerCamera.position, playerCamera.forward, out RaycastHit hit, Mathf.Infinity))
        {
            isTethered = true;
            timeGrappledSince = 0f;
            timeSuperJumpSince = superJumpAllow;
            tetherPoint = hit.point;
            tetherLength = Vector3.Distance(tetherPoint, playerCamera.position);
            lr.positionCount = 2;
        }
    }

    void EndGrapple()
    {
        isTethered = false;
        lr.positionCount = 0;
    }

    void BeginJump()
    {
        hasJumpInput = true;
    }

    void EndJump()
    {
        hasJumpInput = false;
    }
}
