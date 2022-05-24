using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoaderManager : MonoBehaviour
{
    public enum Scene
    {
        SampleScene,
        DeathScene,
    }

    private static SceneLoaderManager sceneLoaderManager;

    public static SceneLoaderManager instance
    {
        get
        {
            if (!sceneLoaderManager)
            {
                sceneLoaderManager = FindObjectOfType(typeof(SceneLoaderManager)) as SceneLoaderManager;

                if (!sceneLoaderManager)
                {
                    Debug.LogError("There needs to be one active SceneLoaderManager script on a GameObject in your scene.");
                }
                else
                {
                    sceneLoaderManager.Init();

                    //  Sets this to not be destroyed when reloading scene
                    DontDestroyOnLoad(sceneLoaderManager);
                }
            }
            return sceneLoaderManager;
        }
    }

    void Init()
    {
    }

    public static void LoadBuildIndexed(int index)
    {
        if (SceneManager.sceneCountInBuildSettings < index)
        {
            Debug.LogError("Not enough scenes");
            return;
        }
        SceneManager.LoadScene(index, LoadSceneMode.Single);
    }

    public static void ExitApp()
    {
        Application.Quit();
    }

    public static void LoadEnum(Scene enumScene, LoadSceneMode sceneMode = LoadSceneMode.Single)
    {
        SceneManager.LoadScene(enumScene.ToString(), sceneMode);
    }

    public static void LoadName(string nameScene, LoadSceneMode sceneMode = LoadSceneMode.Single)
    {
        SceneManager.LoadScene(nameScene, sceneMode);
    }
}