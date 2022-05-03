using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileSpawner : MonoBehaviour
{
    [SerializeField]
    private GameObject projectileToSpawn;

    [SerializeField]
    private float timerToSpawn = 2f;
    [SerializeField]
    private float numberToSpawn = 2f;

    [SerializeField]
    private bool loop = false;

    [SerializeField]
    private float timerCooldown = 2f;

    [SerializeField]
    private Transform parentTransform;
    bool isSpawning = false;

    private float currentCooldown;

    Coroutine spawnCor;

    // Start is called before the first frame update
    void Update()
    {
        if (isSpawning)
        {
            StartSpawning();
        }
    }

    private void FixedUpdate()
    {
        currentCooldown = Mathf.Max(currentCooldown - Time.fixedDeltaTime, 0);
    }

    private void StartSpawning()
    {
        
        if (currentCooldown <= 0) {
            if (timerCooldown == float.PositiveInfinity) {
                return;
            }
            if (!loop)
            {
                timerCooldown = float.PositiveInfinity;
            }
            currentCooldown = timerCooldown;
            spawnCor = StartCoroutine(spawnCoroutine());
            
        }
    }

    IEnumerator spawnCoroutine() {
        for (int i = 0; i < numberToSpawn; i++)
        {
            yield return new WaitForSeconds(timerToSpawn);
            Instantiate(projectileToSpawn, this.transform.position, this.transform.rotation, parentTransform);
            Instantiate(projectileToSpawn, this.transform);
            
        }
        Debug.Log("spawning");
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.magenta;

        Gizmos.DrawSphere(this.transform.position, radius: 0.02f);
        Gizmos.DrawRay(this.transform.position, this.transform.forward);
    }

    public GameObject ProjectileToSpawn {
        get {
            return projectileToSpawn;
        }
        set {
            this.projectileToSpawn = value;
        }
    }

    public void SwitchSpawning(bool state)
    {
        isSpawning = state;
        if (spawnCor != null && state == false)
        {
            StopCoroutine(spawnCor);
        }
    }

    public bool isSpawningMethod()
    {
        if (isSpawning)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
