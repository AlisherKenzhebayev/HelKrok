using UnityEngine;

public class RestartGame : MonoBehaviour
{
    public void RestartLevel()
    {
        FindObjectOfType<SceneLoaderManager>().LoadBuildIndexed(0);
    }
}
