using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Cinemachine;
using UnityEngine.UI;

public class Menu : MonoBehaviour
{
    public GameObject fadeCanvas;
    public CinemachineFading camFading;
    public Color fadeInColor;
    public Color fadeOutColor;
    // Start is called before the first frame update
    void Start()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PlayGame()
    {
        print("play");
        StartCoroutine(StartGame());
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    IEnumerator StartGame()
    {
        StartCoroutine(camFading.FadeInFOV());
        for (float time = 2f; time > 0; time -= Time.deltaTime)
        {
            foreach (Transform ch in transform)
            {
                ch.gameObject.GetComponent<TextMeshProUGUI>().color = Color.Lerp(ch.gameObject.GetComponent<TextMeshProUGUI>().color, new Color(0, 0, 0, 0), 2f);
            }
            yield return null;
        }
        fadeCanvas.GetComponent<Animation>().Play("fadeout");
        yield return new WaitForSeconds(3f);
        SceneLoaderManager.LoadEnum(SceneLoaderManager.ScenesEnum.MainLevel);

    }

}
