using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationFloor : MonoBehaviour
{
    Animation aniComp;
    // Start is called before the first frame update
    void Start()
    {
        float random = Random.Range(.5f, 2f);
        aniComp = GetComponent<Animation>();
        Invoke("PlayAnimation", random);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void PlayAnimation()
    {
        aniComp.Play();
    }
}
