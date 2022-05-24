using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileSpawner : MonoBehaviour
{
    [SerializeField]
    public GameObject projectileToSpawn;

    [SerializeField]
    public Transform spawnTransform;

    [SerializeField]
    public float timerToSpawn = 2f;
    [SerializeField]
    public float numberToSpawn = 2f;

    [SerializeField]
    public bool loop = false;

    [SerializeField]
    public float timerCooldown = 2f;

    public GameObject originHitbox;

    private float currentCooldown;
    private bool isSpawning = false;

    Coroutine spawnCor = null;

    private void FixedUpdate()
    {
        currentCooldown = Mathf.Max(currentCooldown - Time.fixedDeltaTime, 0);
    }

    private void Update()
    {
        if(isSpawning)
            RunSpawning();    
    }

    public void RunSpawning()
    {
        if (currentCooldown <= 0)
        {
            Debug.Log("ProjectileSpawner - CD ok!");

            if (timerCooldown == float.PositiveInfinity)
            {
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
            Transform transformToSpawn = this.transform;
            if(spawnTransform != null) {
                transformToSpawn = spawnTransform;
            }

            GameObject obj = Instantiate(projectileToSpawn, transformToSpawn, false);
            obj.transform.SetParent(null);
            
            var dd = obj.gameObject.GetComponentInChildren<DamageDealer>();
            dd.originHitbox = originHitbox;

            yield return new WaitForSeconds(timerToSpawn);
        }
        Debug.Log("ProjectileSpawner - spawning");
    }

    internal void StopFiring()
    {
        isSpawning = false;
    }

    internal void StartFiring()
    {
        isSpawning = true;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.magenta;

        Gizmos.DrawSphere(this.transform.position, radius: 0.02f);
        Gizmos.DrawRay(this.transform.position, this.transform.forward);
    }
}
