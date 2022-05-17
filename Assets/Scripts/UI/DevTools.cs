using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DevTools : MonoBehaviour
{
    Text versionText;
    // Start is called before the first frame update
    void Start()
    {
        versionText = GetComponent<Text>();
        versionText.text =  Application.version;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
