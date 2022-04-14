 using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PostGenScale : MonoBehaviour
{
    public int levelScale = 4;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(LateStart());   
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator LateStart()
    {
        yield return new WaitForEndOfFrame();
        transform.localScale = new Vector3(levelScale, levelScale, levelScale);
    }
}
