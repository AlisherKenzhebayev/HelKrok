using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrappleInteractable : MonoBehaviour, IInteractable
{
    [Tooltip("Replacement material (visual feedback)")]
    [SerializeField]
    private Material shaderMaterial;
    internal GameObject player;
    internal PlayerController playerController;
    internal Renderer[] rendererRef;
    internal Material[] originalMaterial;

    internal bool isGrappled = false;
    internal bool isFocused = false;

    // Start is called before the first frame update
    internal virtual void Start()
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
    internal virtual void Update()
    {
        if (isFocused) {
            ShowMaterial();
        }
        if (isGrappled || !isFocused) {
            ResetMaterial();
        }
        isFocused = false;
    }

    internal virtual void ShowMaterial()
    {
        if (true)
        {
            for (int i = 0; i < this.rendererRef.Length; i++)
            {
                this.rendererRef[i].material = shaderMaterial;
            }
        }
    }

    public virtual void InteractStart()
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

    public void InteractStop()
    {
        isGrappled = false;
    }
}
