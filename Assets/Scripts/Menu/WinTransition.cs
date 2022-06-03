using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WinTransition : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField]
    private GameObject gm;

    private void OnEnable()
    {
        gm.SetActive(false);
        GetComponentInChildren<Animation>().Play("fadeInWinScreen");
        GetComponentInChildren<Animation>().PlayQueued("fadingWinScreen");
        //StartCoroutine(ExitToMenu());
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    IEnumerator ExitToMenu()
    {
        yield return new WaitForSeconds(3f);
        SceneLoaderManager.LoadEnum(SceneLoaderManager.ScenesEnum.Menu);
    }
}
