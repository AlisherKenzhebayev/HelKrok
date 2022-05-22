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

    public bool isSpawning = false;

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

    public void StartSpawning()
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
            
            Transform transformToSpawn = this.transform;
            if(spawnTransform != null) {
                transformToSpawn = spawnTransform;
            }

            GameObject obj = Instantiate(projectileToSpawn, transformToSpawn, false);
            obj.transform.SetParent(null);
        }
        Debug.Log("spawning");
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.magenta;

        Gizmos.DrawSphere(this.transform.position, radius: 0.02f);
        Gizmos.DrawRay(this.transform.position, this.transform.forward);
    }

    public void SwitchSpawning(bool state)
    {
        isSpawning = state;
        if (spawnCor != null && state == false)
        {
            StopCoroutine(spawnCor);
        }
    }
}
