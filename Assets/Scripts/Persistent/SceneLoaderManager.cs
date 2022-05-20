using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoaderManager : MonoBehaviour
{
    public enum Scene
    {
        SampleScene,
        DeathScene,
    }

    private static SceneLoaderManager sceneLoaderInstance;

    public static SceneLoaderManager instance
    {
        get
        {
            if (!sceneLoaderInstance)
            {
                sceneLoaderInstance = FindObjectOfType(typeof(SceneLoaderManager)) as SceneLoaderManager;

                if (!sceneLoaderInstance)
                {
                    Debug.LogError("There needs to be one active SceneLoaderManager script on a GameObject in your scene.");
                }
                else
                {
                    sceneLoaderInstance.Init();

                    //  Sets this to not be destroyed when reloading scene
                    DontDestroyOnLoad(sceneLoaderInstance);
                }
            }
            return sceneLoaderInstance;
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