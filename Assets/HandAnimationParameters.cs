using UnityEngine;

public class HandAnimationParameters : MonoBehaviour
{
    public PlayerController controller;
    public Animator animator;


    // Update is called once per frame
    void Update()
    {
        animator.SetBool("Grappling", controller.IsTethered);        
    }
}
