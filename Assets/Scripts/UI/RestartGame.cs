using UnityEngine;

public class RestartGame : MonoBehaviour
{
    public void RestartLevel()
    {
        SceneLoaderManager.LoadBuildIndexed(0);
    }
}
