using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrappleInteractable : MonoBehaviour, IInteractable
{
    [Tooltip("Replacement material (visual feedback)")]
    [SerializeField]
    private Material shaderMaterial;
    private GameObject player;
    private PlayerController playerController;
    private Renderer[] rendererRef;
    private Material[] originalMaterial;

    private bool isGrappled = false;
    private bool isFocused = false;

    // Start is called before the first frame update
    void Start()
    {
        this.player = GameObject.FindGameObjectWithTag("Player");
        this.playerController = player.GetComponentInChildren<PlayerController>();

        this.rendererRef = this.GetComponentsInChildren<Renderer>();
        this.originalMaterial = new Material[this.rendererRef.Length];
        for(int i = 0; i < this.rendererRef.Length; i++) {
            this.originalMaterial[i] = this.rendererRef[i].material;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (isFocused) {
            ShowMaterial();
        }
        if (isGrappled || !isFocused) {
            ResetMaterial();
        }
        isFocused = false;
        isGrappled = false;
    }

    private void ShowMaterial()
    {
        if (true)
        {
            for (int i = 0; i < this.rendererRef.Length; i++)
            {
                this.rendererRef[i].material = shaderMaterial;
            }
        }
    }

    public void Execute()
    {
        isGrappled = true;
    }

    public void Visualize()
    {
        isFocused = true;
    }

    private void ResetMaterial()
    {
        for (int i = 0; i < this.rendererRef.Length; i++)
        {
            this.rendererRef[i].material = originalMaterial[i];
        }
    }
}
