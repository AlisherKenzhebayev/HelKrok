using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterController : MonoBehaviour
{
    //**********
    //  PUBLIC
    //**********

    [Header("References")]
    [Tooltip("Camera follow transform")]
    public Transform playerCamera;
    public Transform groundCheck;

    [Header("Debug Materials")]
    public Material debugGoodStateMaterial;
    public Material debugBadStateMaterial;
    public GameObject debugObject;

    [Header("Rotation")]
    [Tooltip("Rotation speed X for moving the camera")]
    public float horizontalRotationSpeed = 200f;
    [Tooltip("Rotation speed Y for moving the camera")]
    public float verticalRotationSpeed = 200f;

    [Header("Movement")]
    [Tooltip("Ground speed")]
    public float groundSpeed = 10f;
    [Tooltip("Max air speed")]
    public float maxAirSpeed = 5f;
    [Tooltip("Jump force")]
    public float jumpForce = 300f;
    [Tooltip("Air strafe force")]
    public float airStrafeForce = 0.5f;
    
    [SerializeField]
    private bool tethered = false;
    private float tetherLength;
    private Vector3 tetherPoint;

    //**********
    //  PRIVATE
    //**********

    private Rigidbody rb;
    private float xMouseInput;
    private float yMouseInput;
    private float m_CameraVerticalAngle = 0f;
    private Vector3 characterVelocity;

    private MeshRenderer debugRenderer;

    // Start is called before the first frame update
    void Start()
    {
        rb = this.GetComponent<Rigidbody>();
        if (rb == null) {
            Debug.LogError("Error - no RigidBody component");
        }

        debugRenderer = debugObject.GetComponent<MeshRenderer>();
        if (rb == null)
        {
            Debug.LogError("Error - no MeshRenderer component");
        }
    }

    // Update is called once per frame
    void Update()
    {
        HandleGrappleInput();

        HandleCameraInput();

        if (tethered)
        {
            debugRenderer.material = debugGoodStateMaterial;
            ApplyGrapplePhysics();
        }
        else
        {
            debugRenderer.material = debugBadStateMaterial;
        }
            HandleInput();
        
    }

    private void HandleGrappleInput()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            if (!tethered)
            {
                BeginGrapple();
            }
        }

        if (Input.GetKeyUp(KeyCode.Mouse0))
        {
            if (tethered)
            {
                EndGrapple();
            }
        }
    }

    private void HandleCameraInput()
    {
        // Horizontal mouse inputs
        {
            xMouseInput = Input.GetAxis(GameConstants.k_MouseAxisNameHorizontal);
            transform.Rotate(new Vector3(0f, xMouseInput * horizontalRotationSpeed, 0f), Space.Self);
        }

        // Vertical mouse inputs
        {
            yMouseInput = Input.GetAxis(GameConstants.k_MouseAxisNameVertical);
            m_CameraVerticalAngle -= yMouseInput * verticalRotationSpeed;

            m_CameraVerticalAngle = Mathf.Clamp(m_CameraVerticalAngle, -89f, 89f);

            playerCamera.transform.localEulerAngles = new Vector3(m_CameraVerticalAngle, 0f, 0f);
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(playerCamera.position, playerCamera.forward);
    }

    void BeginGrapple()
    {
        if (Physics.Raycast(playerCamera.position, playerCamera.forward, out RaycastHit hit, Mathf.Infinity))
        {
            tethered = true;
            tetherPoint = hit.point;
            tetherLength = Vector3.Distance(tetherPoint, playerCamera.position);
        }
    }

    void EndGrapple()
    {
        tethered = false;
    }

    void ApplyGrapplePhysics()
    {
        Vector3 directionToGrapple = Vector3.Normalize(tetherPoint - transform.position);
        float currentDistanceToGrapple = Vector3.Distance(tetherPoint, transform.position);

        float speedTowardsGrapplePoint = Mathf.Round(Vector3.Dot(rb.velocity, directionToGrapple) * 100f) / 100f;

        if (speedTowardsGrapplePoint < 0)
        {
            if (currentDistanceToGrapple > tetherLength)
            {
                rb.velocity -= speedTowardsGrapplePoint * directionToGrapple;
                rb.position = tetherPoint - directionToGrapple * tetherLength;
            }
        }
    }

    private void HandleInput()
    {
        // Handle keyboard movement
        {
            Vector3 worldspaceMoveInput = transform.TransformVector(GetMoveInput());

            if (isGrounded())
            { 
                // Handle Movement on ground
                characterVelocity = worldspaceMoveInput * groundSpeed;
                characterVelocity.y = rb.velocity.y;

                rb.velocity = characterVelocity;
            } else {
                // TODO: Handle Movement in air (Air-strafing) 
                AirMovement(worldspaceMoveInput);

                //rb.AddForce(worldspaceMoveInput * airStrafeForce, ForceMode.VelocityChange);
            }
        }

        // Handle jump keys
        {
            if (isGrounded() && Input.GetButtonDown(GameConstants.k_ButtonNameJump)) {
                rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
            }
        }
    }

    //************
    //  PRIVATE
    //************

    private bool isGrounded()
    {
        if (Physics.Raycast(groundCheck.position, -transform.up, 0.5f)) {
            return true;
        }
        return false;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="vector3"></param>
    void AirMovement(Vector3 vector3)
    {
        // project the velocity onto the movevector
        Vector3 projVel = Vector3.Project(GetComponent<Rigidbody>().velocity, vector3);

        // check if the movevector is moving towards or away from the projected velocity
        bool isAway = Vector3.Dot(vector3, projVel) <= 0f;

        // only apply force if moving away from velocity or velocity is below MaxAirSpeed
        if (projVel.magnitude < maxAirSpeed || isAway)
        {
            // calculate the ideal movement force
            Vector3 vc = vector3.normalized * airStrafeForce;

            // cap it if it would accelerate beyond MaxAirSpeed directly.
            if (!isAway)
            {
                vc = Vector3.ClampMagnitude(vc, maxAirSpeed - projVel.magnitude);
            }
            else
            {
                vc = Vector3.ClampMagnitude(vc, maxAirSpeed + projVel.magnitude);
            }

            // Apply the force
            GetComponent<Rigidbody>().AddForce(vc, ForceMode.VelocityChange);

        }
        else { 
        }
    }


    public Vector3 GetMoveInput()
    {
        Vector3 move = new Vector3(Input.GetAxisRaw(GameConstants.k_AxisNameHorizontal), 0f, Input.GetAxisRaw(GameConstants.k_AxisNameVertical));

        // constrain move input to a maximum magnitude of 1, otherwise diagonal movement might exceed the max move speed defined
        move = Vector3.ClampMagnitude(move, 1);

        return move;
    }
}
