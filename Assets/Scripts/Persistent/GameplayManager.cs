using System;
using UnityEngine;

public class GameplayManager : MonoBehaviour
{
    private static Vector3 spawnPosition;
    private static Quaternion spawnRotation;

    private static GameplayManager gameManager;

    private static GameObject player;
    private static PlayerController playerController;
    private static Inventory playerInventory;

    private static GameObject inventoryCanvas;
    private static GameObject pauseCanvas;

    private static bool isShowingUiScreen;

    public static bool isGamePaused;

    public static GameplayManager instance {
        get {
            gameManager = FindObjectOfType(typeof(GameplayManager)) as GameplayManager;
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
        ContinueGameTime();
    }

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        if (player == null)
        {
            Debug.LogError("Error - no Player tag exists");
        }

        if (player != null)
        {
            playerController = player.GetComponent<PlayerController>();
        }
        if (playerController == null)
        {
            Debug.LogError("Error - no PlayerController component exists");
        }

        if (player != null)
        {
            playerInventory = player.GetComponentInChildren<Inventory>();
        }
        if (playerInventory == null)
        {
            Debug.LogError("Error - no Inventory child component exists");
        }

        inventoryCanvas = GameObject.FindGameObjectWithTag("inventoryUI");
        if (inventoryCanvas == null)
        {
            Debug.LogError("Error - no inventoryUI tag exists");
        }

        pauseCanvas = GameObject.FindGameObjectWithTag("pauseUI");
        if (pauseCanvas == null)
        {
            Debug.LogError("Error - no pauseUI tag exists");
        }
    }

    private void Update()
    {
        if (InputManager.GetResetKeyDown())
        {
            ResetCheckpoint();
        }

        if (InputManager.GetRestartKeyDown())
        {
            RestartGame();
        }

        UpdateInventoryCanvas();

        UpdateInventory();

        CursorLogic();
    }

    public static void PauseGameTime()
    {
        Time.timeScale = 0f;
        isGamePaused = true;
    }

    public static void ContinueGameTime()
    {
        Time.timeScale = 1f;
        isGamePaused = false;
    }

    public static void RestartGame()
    {
        SceneLoaderManager.ForceLoadEnum(SceneLoaderManager.ScenesEnum.MainLevel);
    }

    public static void MainMenu()
    {
        SceneLoaderManager.LoadEnum(SceneLoaderManager.ScenesEnum.Menu);
    }

    private void UpdateInventory()
    {
        if (playerInventory == null)
        {
            Debug.LogWarning("There's no inventory present in the scene");
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
            if (isGamePaused)
            {
                Debug.LogWarning("Game is paused, inventory open state tried changing");
                return;
            }

            var invCanvas = inventoryCanvas.GetComponent<Canvas>();
            invCanvas.enabled ^= true;
        }

        if (inventoryCanvas.GetComponent<Canvas>().enabled || pauseCanvas.GetComponent<Canvas>().enabled)
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
        bool checkCursorShow = isShowingUiScreen;

        // Process the checks for inventory canvas
        if (inventoryCanvas != null) {
            var canvas = inventoryCanvas.GetComponent<Canvas>();
            checkCursorShow |= canvas.enabled;
        }

        // Process the checks for pause canvas
        if (pauseCanvas != null)
        {
            var canvas = pauseCanvas.GetComponent<Canvas>();
            checkCursorShow |= canvas.enabled;
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

    public void ExitGame()
    {
        Application.Quit();
    }

    public static void ShowUiScreen(bool value = true)
    {
        isShowingUiScreen = value;
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
        LoadGame();
        if (playerController != null) { 
            isShowingUiScreen = false;
            playerController.ResetToCheckpoint(spawnPosition, spawnRotation);
        }
    }

    public static void ChangeSpawnPoint(Vector3 _position, Quaternion _rotation)
    {
        spawnPosition = _position;
        spawnRotation = _rotation;
        
        SaveGame();
    }

    //************
    //  SAVING / LOADING
    //************

    public static void SaveGame() {
        SaveJsonData();
    }

    public static void LoadGame()
    {
        LoadJsonData();
    }

    private static void SaveJsonData() {
        SaveData sd = new SaveData();
        GameplayManager.PopulateSaveData(sd);

        if (FileManager.WriteToFile("SaveFile.dat", sd.ToJson())){
            Debug.Log("Save successful");
        }
    }

    private static void LoadJsonData() {
        if (FileManager.LoadFromFile("SaveFile.dat", out var json)) {
            SaveData sd = new SaveData();
            sd.LoadFromJson(json);

            GameplayManager.LoadFromSaveData(sd);
            Debug.Log("Load successful");
        }
    }

    public static void PopulateSaveData(SaveData a_saveData)
    {
        a_saveData.m_CheckpointPosition = new float[3];
        a_saveData.m_CheckpointPosition[0] = spawnPosition.x;
        a_saveData.m_CheckpointPosition[1] = spawnPosition.y;
        a_saveData.m_CheckpointPosition[2] = spawnPosition.z;

        a_saveData.m_CheckpointRotation = new float[4];
        a_saveData.m_CheckpointRotation[0] = spawnRotation.x;
        a_saveData.m_CheckpointRotation[1] = spawnRotation.y;
        a_saveData.m_CheckpointRotation[2] = spawnRotation.z;
        a_saveData.m_CheckpointRotation[3] = spawnRotation.w;

        if (playerController != null) {
            playerController.PopulateSaveData(a_saveData);
        }
    }

    public static void LoadFromSaveData(SaveData a_saveData)
    {
        spawnPosition = new Vector3(
            a_saveData.m_CheckpointPosition[0],
            a_saveData.m_CheckpointPosition[1],
            a_saveData.m_CheckpointPosition[2]);

        spawnRotation = new Quaternion(
            a_saveData.m_CheckpointRotation[0],
            a_saveData.m_CheckpointRotation[1],
            a_saveData.m_CheckpointRotation[2],
            a_saveData.m_CheckpointRotation[3]);

        if (playerController != null) {
            playerController.LoadFromSaveData(a_saveData);
        }
    }
}
