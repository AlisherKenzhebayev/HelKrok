using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour, ISaveable
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

    [Header("Camera Rotation/Sensitivity")]
    [Tooltip("Rotation speed X for moving the camera")]
    [SerializeField] 
    private float horizontalRotationSpeed = 2.2f;
    [Tooltip("Rotation speed Y for moving the camera")]
    [SerializeField]
    private float verticalRotationSpeed = 2.2f;

    [Header("Basic Movement")]
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
    [Tooltip("Grapple overlap geometry time")]
    [SerializeField]
    private float grappleMaxOverlapTime = 3f;
    [Tooltip("Grapple shake amount")]
    [SerializeField]
    private float shakeValue = 3f;
    [Tooltip("LayerMask of colliders that are checked for collision availability")]
    [SerializeField]
    public LayerMask grappleLayers;

    [Header("Movement while grappled")]
    [Tooltip("Grapple move force")]
    [SerializeField]
    private float grappleMoveForce = 100f;
    [Tooltip("Grapple move force curve")]
    [SerializeField]
    private AnimationCurve grappleMoveForceEffectCurve = null;

    [Header("Air Movement")]
    [Tooltip("Air strafe force")]
    [SerializeField]
    private float airStrafeForce = 100f;
    [Tooltip("Max air speed")]
    [SerializeField] 
    private float maxAirSpeed = 5f;
    [Tooltip("Air force curve (maxSpeed)")]
    [SerializeField] 
    private AnimationCurve airForceEffectCurve = null;

    [Header("Energy costs")]
    [SerializeField]
    private float grappleCost = 5f;
    [SerializeField]
    private float grappleEnergyCost = .2f;
    [SerializeField]
    private float grappleCostCont = 0.0f;

    [Header ("Debug")]
    [SerializeField]
    private GameObject grappleEndPrefab = null;


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

    private bool isTethered = false;
    private bool isAction = false;
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

    private Rigidbody rb;
    private float m_CameraVerticalAngle = 0f;

    private Vector3 worldspaceMoveInput;
    private Vector3 localMoveInput;

    private DamageTaker damageTaker;
    private EnergyDepleter energyDepleter;
    private TriggerGrappleClosenessCheck triggerClosenessCheck;
    private int grappleLayerMask;

    private GravityCustom gravityCustom;
    private AirFrictionCustom airFrictionCustom;
    private GrappleVisualizer grappleVisualizer;

    private GameObject grappleEndpoint;
    
    // Start is called before the first frame update
    void Start()
    {
        grappleLayerMask = grappleLayers.value;

        physicsCommands = new List<ICommand>();
        
        rb = this.GetComponent<Rigidbody>();
        if (rb == null)
        {
            Debug.LogError("Error - no RigidBody component");
        }

        gravityCustom = this.GetComponent<GravityCustom>();
        if (gravityCustom == null)
        {
            Debug.LogError("Error - no GravityCustom component");
        }

        grappleVisualizer = this.GetComponent<GrappleVisualizer>();
        if (grappleVisualizer == null)
        {
            Debug.LogError("Error - no GrappleVisualizer component");
        }

        airFrictionCustom = this.GetComponent<AirFrictionCustom>();
        if (airFrictionCustom == null)
        {
            Debug.LogError("Error - no AirFrictionCustom component");
        }

        energyDepleter = this.GetComponentInChildren<EnergyDepleter>();
        if (energyDepleter == null)
        {
            Debug.LogError("Error - no EnergyDepleter child component");
        }

        damageTaker = this.GetComponentInChildren<DamageTaker>();
        if (damageTaker == null)
        {
            Debug.LogError("Error - no DamageTaker child component");
        }

        triggerClosenessCheck = this.GetComponentInChildren<TriggerGrappleClosenessCheck>();
        if (triggerClosenessCheck == null)
        {
            Debug.LogError("Error - no TriggerGrappleClosenessCheck child component");
        }
    }

    // Update is called once per frame
    void Update()
    {
        TestGrapple();

        HandleMouseInput();

        grappleVisualizer.UpdateGrappleVisualization(
            isTethered, 
            tetherPoint, 
            playerCamera.transform, 
            grappleMaxOverlapTime, 
            timeGrappleOverlapGeometry, 
            timeGrappledSince);

        HandleCameraInput();

        HandleInput();
    }

    private void FixedUpdate()
    {
        physicsCommands.Clear();
        
        ApplyPhysicsDrag();

        UpdateGrapplePosition();
        UpdateContinuedEnergy();

        UpdateTimers();

        ApplyMovePhysics();

        ApplyJumpPhysics();

        // Fire up the commands 
        foreach (ICommand command in physicsCommands)
        {
            command.Execute();
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
        localMoveInput = InputManager.GetMoveInput();
        worldspaceMoveInput = transform.TransformVector(localMoveInput);
        
        // Handle jump keys
        if (InputManager.GetJumpButtonDown())
        {
            if (!hasJumpInput)
            {
                BeginJump();
            }
        }
    }

    private void HandleMouseInput()
    {
        // Action
        if (InputManager.GetActionButtonDown())
        {
            if (!isAction)
            {
                BeginAction();
            }
        }

        if (InputManager.GetActionButtonUp())
        {
            if (isAction)
            {
                EndAction();
            }
        }

        // Grapple
        if (InputManager.GetGrappleButtonDown() 
            && energyDepleter.HasEnough(grappleCost))
        {
            if (!isTethered)
            {
                BeginGrapple();
            }
        }

        if (InputManager.GetGrappleButtonUp())
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
            float xMouseInput = InputManager.getMouseHorizontal();
            this.transform.Rotate(new Vector3(0f, xMouseInput * horizontalRotationSpeed, 0f), Space.Self);
        }

        // Vertical mouse inputs
        {
            float yMouseInput = InputManager.getMouseVertical();
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
            if (!triggerClosenessCheck.HasGameObject(grappleEndpoint))
            {
                // Is grappled to something valid
                ApplyGrapplePhysics();

                // TODO: handle grapple movement
                physicsCommands.Add(new GrappleMoveCommand(rb, transform.TransformVector(GrappleMoveCommand.FilterYZ(localMoveInput)))
                    .SetForceEffectCurve(grappleMoveForceEffectCurve)
                    .SetMoveForce(grappleMoveForce)
                    .SetLookVector(this.playerCamera.forward));
            }
            else {
                ApplyGrapplePhysics(0.1f);
            }
        }
        else
        {
            // Not attached to anything
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

    private void ApplyGrapplePhysics(float _movementMul = 1.0f)
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
        //Debug.Log("Curve GrappleForce " + curveMod);
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
    /// Raycast checks and visualizes if the grapple point is available (not guaranteed)
    /// </summary>
    private void TestGrapple()
    {
        if (isTethered) {
            return;
        }

        if (Physics.Raycast(playerCamera.position, playerCamera.forward, out RaycastHit hit, energyDepleter.GetEnergy(), grappleLayerMask)
            && hit.collider.gameObject.TryGetComponent<IInteractable>(out IInteractable interactable))
        {
            interactable.Visualize();
        }
    }

    /// <summary>
    /// Applies both the gravity and air drag components
    /// </summary>
    private void ApplyPhysicsDrag()
    {
        float frac = precisionFloat(timeGrappledSince / grappleMaxTime);

        gravityCustom.UpdateFrac(frac);
        airFrictionCustom.UpdateFrac(frac);
    }

    private void UpdateGrapplePosition()
    {
        if (isTethered)
        {
            tetherPoint = tetherObject.transform.position + tetherOffset;
            
            grappleEndpoint.transform.position = tetherPoint;
        }
    }

    private void UpdateContinuedEnergy()
    {
        if (isTethered)
        {
            // TODO: Simulate continuous grapple if tethered and close
            energyDepleter.Use(grappleCostCont, 0);
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

    void BeginGrapple()
    {
        if (Physics.Raycast(playerCamera.position, playerCamera.forward, out RaycastHit hit, energyDepleter.GetEnergy(), grappleLayerMask) 
            && hit.collider.gameObject.TryGetComponent<IInteractable>(out grappleInteractable))
        {
            energyDepleter.Use(grappleCost, grappleEnergyCost);
            grappleInteractable.InteractStart();

            isTethered = true;
            timeGrappledSince = 0f;
            timeGrappleOverlapGeometry = 0f;
            timeSuperJumpSince = superJumpAllow;
            tetherObject = hit.collider.gameObject;
            tetherOffset = hit.point - tetherObject.transform.position;
            tetherPoint = tetherObject.transform.position + tetherOffset;
            tetherLength = Vector3.Distance(tetherPoint, playerCamera.position);

            grappleEndpoint = Instantiate(grappleEndPrefab, tetherPoint, Quaternion.identity);
            grappleEndpoint.transform.SetParent(null);
        }
    }

    void EndGrapple()
    {
        grappleInteractable.InteractStop();

        isTethered = false;
        tetherObject = null;

        timeGrappledSince = 0f;
        timeGrappleOverlapGeometry = 0f;

        Destroy(grappleEndpoint);
        grappleEndpoint = null;
    }

    void BeginAction() {
        isAction = true;
        EventManager.TriggerEvent("PlayerActionButton", new Dictionary<string, object> { { "amount", true } });
    }

    void EndAction()
    {
        isAction = false;
        EventManager.TriggerEvent("PlayerActionButton", new Dictionary<string, object> { { "amount", false } });
    }

    public bool IsTethered
    {
        get {
            return isTethered;
        }
    }

    void BeginJump()
    {
        hasJumpInput = true;
    }

    void EndJump()
    {
        hasJumpInput = false;
    }

    public void ResetToCheckpoint(Vector3 _position, Quaternion _rotation) {
        rb.velocity = Vector3.zero;
        this.transform.position = _position;
        this.transform.rotation = _rotation;
    }

    //************
    //  SAVE / LOAD
    //************

    public void PopulateSaveData(SaveData a_saveData)
    {
        SaveData.PlayerData _playerData = new SaveData.PlayerData();

        _playerData.m_Position = new float[3];
        _playerData.m_Position[0] = transform.position.x;
        _playerData.m_Position[1] = transform.position.y;
        _playerData.m_Position[2] = transform.position.z;

        _playerData.m_Rotation = new float[4];
        _playerData.m_Rotation[0] = transform.rotation.x;
        _playerData.m_Rotation[1] = transform.rotation.y;
        _playerData.m_Rotation[2] = transform.rotation.z;
        _playerData.m_Rotation[3] = transform.rotation.w;
        
        _playerData.m_FracHealth = damageTaker.FracHealth;
        
        a_saveData.playerData = _playerData;
    }

    public void LoadFromSaveData(SaveData a_saveData)
    {
        this.transform.position = new Vector3(
            a_saveData.playerData.m_Position[0],
            a_saveData.playerData.m_Position[1],
            a_saveData.playerData.m_Position[2]);

        this.transform.rotation = new Quaternion(
         a_saveData.playerData.m_Rotation[0],
         a_saveData.playerData.m_Rotation[1],
         a_saveData.playerData.m_Rotation[2],
         a_saveData.playerData.m_Rotation[3]);

        this.damageTaker.FracHealth = a_saveData.playerData.m_FracHealth;
    }
}
