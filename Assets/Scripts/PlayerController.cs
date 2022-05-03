using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    //**********
    //  PUBLIC
    //**********

    [Header("References")]
    [Tooltip("Camera follow transform")]
    [SerializeField]
    private Transform playerCamera = null;
    [SerializeField]
    private Transform groundCheck = null;
    [SerializeField]
    private Transform grappleSpawn = null;
    [SerializeField] 
    private GameObject chainLinkPrefab = null;
    [SerializeField]
    private float distanceSpawnLinks = 0.5f;

    [Header("Debug Materials")]
    [SerializeField]
    private Material debugGoodStateMaterial = null;
    [SerializeField]
    private Material debugQuoStateMaterial = null;
    [SerializeField]
    private Material debugBadStateMaterial = null;

    [SerializeField]
    private GameObject debugObject = null;

    [Header("Camera Rotation/Sensitivity")]
    [Tooltip("Rotation speed X for moving the camera")]
    [SerializeField] 
    private float horizontalRotationSpeed = 2.2f;
    [Tooltip("Rotation speed Y for moving the camera")]
    [SerializeField]
    private float verticalRotationSpeed = 2.2f;

    [Header("Basic Movement")]
    [Tooltip("Gravity acceleration")]
    [SerializeField]
    private float gravityAcceleration = 10f;
    [Tooltip("Ground speed")]
    [SerializeField]
    private float groundSpeed = 10f;
    [Tooltip("Jump force")]
    [SerializeField]
    private float jumpForce = 300f;
    [Tooltip("Super Jump force")]
    [SerializeField]
    private float superJumpForce = 600f;
    [Tooltip("Superjump timing window (s)")]
    [SerializeField]
    private float superJumpAllow = 0.2f;
    [Tooltip("Backmove dampening")]
    [Range(0f, 1f)]
    [SerializeField]
    private float backwardsSpeedCoef = 0.15f;

    [Header("Grapple Movement")]
    [Tooltip("Grapple force")]
    [SerializeField]
    private float grappleForce = 2000f;
    [Tooltip("Grapple speed time")]
    [SerializeField] 
    private float grappleMaxTime = 3f;
    [Tooltip("Speed curve")]
    [SerializeField] 
    private AnimationCurve speedCurve = null;
    [Tooltip("Gravity curve")]
    [SerializeField]
    private AnimationCurve gravityCurve = null;
    [Tooltip("Grapple overlap geometry time")]
    [SerializeField]
    private float grappleMaxOverlapTime = 3f;
    [Tooltip("Grapple shake amount")]
    [SerializeField]
    private float shakeValue = 3f;


    [Header("Air Movement")]
    [Tooltip("Air strafe force")]
    [SerializeField]
    private float airStrafeForce = 100f;
    [Tooltip("Max air speed")]
    [SerializeField] 
    private float maxAirSpeed = 5f;
    [Tooltip("Air force curve (dependency of time)")]
    [SerializeField] 
    private AnimationCurve airForceEffectCurve = null;

    [SerializeField]
    private float grappleCost = 5f;

    //**********
    //  PUBLIC
    //**********

    public float getEnergy()
    {
        return energyDepleter.GetEnergy();
    }

    //**********
    //  PRIVATE
    //**********

    private List <GameObject>  links;

    private bool isTethered = false;
    private bool hasJumpInput = false;

    // Grapple information
    private float timeGrappledSince;
    private float timeGrappleOverlapGeometry;
    private float timeSuperJumpSince;
    private float tetherLength;
    private Vector3 tetherPoint;
    private Vector3 tetherOffset;
    private GameObject tetherObject;
    private IInteractable grappleInteractable;

    private List<ICommand> physicsCommands;

    private LineRenderer lr;
    private Rigidbody rb;
    private float m_CameraVerticalAngle = 0f;

    private DamageTaker health;
    private InputManager inputManager;
    private Vector3 worldspaceMoveInput;

    private MeshRenderer debugRenderer;
    private EnergyDepleter energyDepleter;

    // Start is called before the first frame update
    void Start()
    {
        physicsCommands = new List<ICommand>();
        links = new List<GameObject>();
        inputManager = new InputManager();

        CursorMagick();
        
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

        energyDepleter = this.GetComponentInChildren<EnergyDepleter>();
        if(energyDepleter == null)
        {
            Debug.LogError("Error - no EnergyDepleter child component");
        }
    }

    // Update is called once per frame
    void Update()
    {
        TestGrapple();

        HandleGrappleInput();
        DebugGrappleWithMaterial();

        VisualizeGrapple();

        HandleCameraInput();

        HandleInput();
    }

    private void FixedUpdate()
    {
        ApplyGravity();

        physicsCommands.Clear();

        UpdateGrapplePosition();
        UpdateGrappleEnergy();

        UpdateTimers();

        ApplyMovePhysics();

        ApplyJumpPhysics();

        // Fire up the commands 
        foreach (ICommand command in physicsCommands)
        {
            command.execute();
        }
    }
    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(playerCamera.position, playerCamera.forward);

        Gizmos.color = Color.green;
        Gizmos.DrawRay(this.transform.position, worldspaceMoveInput);

        Gizmos.color = Color.blue;
        Gizmos.DrawRay(this.transform.position, Vector3.Project(this.GetComponent<Rigidbody>().velocity, worldspaceMoveInput));

        Gizmos.color = Color.cyan;
        Gizmos.DrawSphere(tetherPoint, 0.1f);
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
        if (Input.GetKeyDown(KeyCode.Mouse0) 
            && energyDepleter.HasEnough(grappleCost))
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
            float xMouseInput = inputManager.getMouseHorizontal();
            this.transform.Rotate(new Vector3(0f, xMouseInput * horizontalRotationSpeed, 0f), Space.Self);
        }

        // Vertical mouse inputs
        {
            float yMouseInput = inputManager.getMouseVertical();
            m_CameraVerticalAngle -= yMouseInput * verticalRotationSpeed;

            m_CameraVerticalAngle = Mathf.Clamp(m_CameraVerticalAngle, -89f, 89f);

            playerCamera.localEulerAngles = new Vector3(m_CameraVerticalAngle, 0f, 0f);
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
                physicsCommands.Add(new MoveCommand(rb, worldspaceMoveInput)
                    .SetGroundSpeed(groundSpeed)
                    .SetBackwardsSpeedCoef(backwardsSpeedCoef));
            }
            else
            {
                // Air strafing
                physicsCommands.Add(new AirStrafeCommand(rb, worldspaceMoveInput)
                    .SetAirStrafeForce(airStrafeForce)
                    .SetMaxAirSpeed(maxAirSpeed)
                    .SetAirForceEffectCurve(airForceEffectCurve));
            }
        }
    }

    private void ApplyJumpPhysics()
    {
        if (hasJumpInput)
        {
            if (superJumpAvailable())
            {
                physicsCommands.Add(new TimedJumpCommand(rb, worldspaceMoveInput)
                    .SetJumpForce(superJumpForce));
                EndJump();
                return;
            }
            if (isGrounded())
            {
                physicsCommands.Add(new JumpCommand(rb, worldspaceMoveInput)
                    .SetJumpForce(jumpForce));
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
        //Debug.Log("Curve GrappleForce" + curveMod);
        rb.AddForce(directionToGrapple * grappleForce * curveMod, ForceMode.Force);
    }

    //************
    //  HELPER
    //************

    private float SmoothStop(float t) {
        return 1f - Mathf.Pow(t - 1f, 2);
    }

    private float SmoothStart(float t)
    {
        return Mathf.Pow(t, 2);
    }

    private float MapCos(float t) {
        return 1 - Mathf.Cos(t * Mathf.PI);
    }

    /// <summary>
    /// Checks and visualizes if the grapple point is available
    /// </summary>
    private void TestGrapple()
    {
        if (isTethered) {
            return;
        }

        if (Physics.Raycast(playerCamera.position, playerCamera.forward, out RaycastHit hit, energyDepleter.GetEnergy())
            && hit.collider.gameObject.TryGetComponent<IInteractable>(out IInteractable interactable))
        {
            interactable.Visualize();
        }
    }

    private void VisualizeGrapple()
    {
        if (isTethered)
        {
            // Spawn in chain links
            float totalDist = Vector3.Distance(grappleSpawn.position, tetherPoint);
            Vector3 directionToGrapple = Vector3.Normalize(tetherPoint - grappleSpawn.position);
            float numberOfSpawn = Mathf.RoundToInt(totalDist / distanceSpawnLinks);
            totalDist -= totalDist % distanceSpawnLinks;
            float distValue = 0;
            for (int i = 0; i < Mathf.Max(links.Count, numberOfSpawn); i++)
            {
                if (i >= numberOfSpawn)
                {
                    if (links[i] != null)
                    {
                        GameObject.Destroy(links[i]);
                        links[i] = null;
                    }
                    continue;
                }

                if (totalDist == 0.0f)
                {
                    totalDist += 0.01f;
                }

                //We get our lerpValue
                float lerpValue = precisionFloat(distValue / totalDist);

                //Get the position
                Vector3 placePosition = Vector3.Lerp(grappleSpawn.position, grappleSpawn.position + directionToGrapple * totalDist, lerpValue);
                if (links.Count > i && links[i] != null)
                {
                    links[i].transform.position = placePosition;
                    Quaternion rotation = Quaternion.LookRotation(directionToGrapple);
                    rotation = Quaternion.Lerp(rotation*chainLinkPrefab.transform.rotation, rotation, lerpValue);
                        
                    // Add shake when overlaps with something
                    Quaternion shake = Quaternion.Euler(0, Random.Range(-shakeValue, shakeValue), Random.Range(-shakeValue, shakeValue));
                    Quaternion localRotation = Quaternion.Lerp(Quaternion.Euler(0, 0, 0), shake, precisionFloat(timeGrappleOverlapGeometry / grappleMaxOverlapTime));
                        
                    if(i % 2 == 0) 
                    {
                        localRotation *= Quaternion.Euler(0, 0, 90);
                    }

                    float maxRenderDistance = 50f;

                    Renderer rnd = links[i].GetComponent<Renderer>();
                    rnd.material.SetFloat("Distance_", Mathf.Clamp(distValue / maxRenderDistance, 0f, 1f));

                    links[i].transform.rotation = rotation;
                    links[i].transform.localRotation *= localRotation;
                    //links[i].transform.localScale = chainLinkPrefab.transform.lossyScale;
                }
                else
                {
                    //Instantiate the object
                    GameObject newLink = Instantiate(chainLinkPrefab, placePosition, playerCamera.transform.rotation);
                    newLink.transform.parent = grappleSpawn.transform;
                    links.Add(newLink);
                }

                distValue += distanceSpawnLinks;
                
            }
        }
        else
        {
            for (int i = 0; i < links.Count; i++)
            {
                GameObject.Destroy(links[i]);
            }
            links.Clear();   
        }
    }

    private void ApplyGravity()
    {
        if (isTethered)
        {
            float curveMod = gravityCurve.Evaluate(precisionFloat(timeGrappledSince / grappleMaxTime));
            rb.AddForce(-this.transform.up * rb.mass * gravityAcceleration * curveMod);
        }
        else
        {
            rb.AddForce(-this.transform.up * rb.mass * gravityAcceleration);
        }
    }

    private void UpdateGrapplePosition()
    {
        if (isTethered)
        {
            tetherPoint = tetherObject.transform.position + tetherOffset;
        }
    }

    private void UpdateGrappleEnergy()
    {
        if (isTethered)
        {
            // Simulate continuous grapple
            energyDepleter.Use(0, 0);
        }
    }


    private void UpdateTimers()
    {
        // Update grapple timer up to grappleMaxTime
        if (isTethered)
        {
            timeGrappledSince = Mathf.Clamp(timeGrappledSince + Time.fixedDeltaTime, 0, grappleMaxTime);
        }
        // Update timer superjump each update
        timeSuperJumpSince = Mathf.Clamp(timeSuperJumpSince - Time.fixedDeltaTime, -1f, superJumpAllow);

        if (isTethered)
        {
            Vector3 directionToGrapple = Vector3.Normalize(tetherPoint - transform.position);
            float currentDistanceToGrapple = Vector3.Distance(tetherPoint, transform.position) - 2f;
            if (Physics.Raycast(grappleSpawn.position, directionToGrapple, out RaycastHit hit, currentDistanceToGrapple))
            {
                timeGrappleOverlapGeometry += Time.fixedDeltaTime;
            }
            else
            {
                if (timeGrappleOverlapGeometry > 0)
                {
                    timeGrappleOverlapGeometry = Mathf.Clamp(
                        timeGrappleOverlapGeometry - Mathf.Lerp(1, 0, SmoothStart(precisionFloat(timeGrappleOverlapGeometry / grappleMaxOverlapTime))),
                        0,
                        grappleMaxOverlapTime);
                }
                else
                {
                    timeGrappleOverlapGeometry = 0f;
                }
            }
        }

        //Detach if overlaps with geometry for >overlapTime
        {
            if (timeGrappleOverlapGeometry > grappleMaxOverlapTime)
            {
                EndGrapple();
            }
        }
    }

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
        if (Physics.Raycast(playerCamera.position, playerCamera.forward, out RaycastHit hit, energyDepleter.GetEnergy()) 
            && hit.collider.gameObject.TryGetComponent<IInteractable>(out grappleInteractable))
        {
            energyDepleter.Use(grappleCost, 0.2f);
            grappleInteractable.Execute();
            isTethered = true;
            timeGrappledSince = 0f;
            timeGrappleOverlapGeometry = 0f;
            timeSuperJumpSince = superJumpAllow;
            tetherObject = hit.collider.gameObject;
            tetherOffset = hit.point - tetherObject.transform.position;
            tetherPoint = tetherObject.transform.position + tetherOffset;
            tetherLength = Vector3.Distance(tetherPoint, playerCamera.position);
            lr.positionCount = 2;
        }
    }

    void EndGrapple()
    {
        grappleInteractable.Execute();
        isTethered = false;
        tetherObject = null;
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
