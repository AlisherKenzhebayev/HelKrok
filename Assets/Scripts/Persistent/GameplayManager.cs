using System;
using UnityEngine;

public class GameplayManager : MonoBehaviour
{
    private static Transform spawnTransform;

    private static GameplayManager gameManager;

    private static GameObject player;
    private static PlayerController playerController;
    private static Inventory playerInventory;
    private static Rigidbody playerRigidbody;

    private static GameObject inventoryCanvas;

    private static bool isShowingWinScreen;

    public static GameplayManager instance {
        get {
            gameManager = FindObjectOfType<GameplayManager>();
            if (!gameManager)
            {
                Debug.LogError("There needs to be one active GameManager script on a GameObject in your scene.");
            }
            else {
                gameManager.Init();

                DontDestroyOnLoad(gameManager);
            }

            return gameManager;
        }
    }

    void Init()
    {
        return;
    }

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        if (player == null)
        {
            Debug.LogError("Error - no Player tag exists");
        }

        playerController = player.GetComponent<PlayerController>();
        if (playerController == null)
        {
            Debug.LogError("Error - no PlayerController component exists");
        }

        playerInventory = player.GetComponentInChildren<Inventory>();
        if (playerInventory == null)
        {
            Debug.LogError("Error - no Inventory child component exists");
        }

        playerRigidbody = player.GetComponent<Rigidbody>();
        if (playerController == null)
        {
            Debug.LogError("Error - no RigidBody component exists");
        }

        inventoryCanvas = GameObject.FindGameObjectWithTag("inventoryUI");
        if (inventoryCanvas == null)
        {
            Debug.LogError("Error - no inventoryUI tag exists");
        }

        ResetCheckpoint();
    }

    private void Update()
    {
        if (InputManager.GetResetKeyDown())
        {
            ResetCheckpoint();
        }

        if (InputManager.GetRestartKeyDown())
        {
            SceneLoaderManager.LoadBuildIndexed(0);
        }

        UpdateInventoryCanvas();

        UpdateInventory();

        CursorLogic();
    }

    private void UpdateInventory()
    {
        if (playerInventory == null) {
            return;
        }

        if (InputManager.GetInventoryKeyNext())
        {
            playerInventory.ChangeNextAbility();
        }

        if (InputManager.GetInventoryKeyPrev())
        {
            playerInventory.ChangePrevAbility();
        }
    }

    private void UpdateInventoryCanvas()
    {
        if (inventoryCanvas == null) {
            Debug.LogWarning("There's no inventory canvas present in the scene");
            return;
        }

        if (InputManager.GetInventoryKeyDown())
        {
            var invCanvas = inventoryCanvas.GetComponent<Canvas>();
            invCanvas.enabled ^= true;
        }

        if (inventoryCanvas.GetComponent<Canvas>().enabled)
        {
            InputManager.CameraLockOn();
        }
        else
        {
            InputManager.CameraLockOff();
        }
    }

    private void CursorLogic()
    {
        bool checkCursorShow = isShowingWinScreen;

        // Process the checks for inventory canvas
        if (inventoryCanvas != null) {
            var invCanvas = inventoryCanvas.GetComponent<Canvas>();
            checkCursorShow |= invCanvas.enabled;
        }

        if (checkCursorShow)
        {
            CursorShow();
        }
        else
        {
            CursorHide();
        }
    }

    public static void ShowWinScreen()
    {
        isShowingWinScreen = true;
    }

    private static void CursorHide()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private static void CursorShow()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public static void ResetCheckpoint()
    {
        isShowingWinScreen = false;

        if (spawnTransform == null) {
            return;
        }

        playerRigidbody.velocity = Vector3.zero;
        player.transform.position = spawnTransform.position;
        player.transform.rotation = spawnTransform.rotation;
    }

    public static void ChangeSpawnPoint(Transform _transform)
    {
        spawnTransform = _transform;
    }
}
