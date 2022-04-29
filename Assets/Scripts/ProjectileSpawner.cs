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

    private float currentCooldown;

    // Start is called before the first frame update
    void Update()
    {
        StartSpawning();
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
            StartCoroutine(spawnCoroutine());
        }
    }

    IEnumerator spawnCoroutine() {
        for (int i = 0; i < numberToSpawn; i++)
        {
            yield return new WaitForSeconds(timerToSpawn);
            Instantiate(projectileToSpawn, this.transform);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.magenta;

        Gizmos.DrawSphere(this.transform.position, radius: 0.1f);
        Gizmos.DrawRay(this.transform.position, this.transform.forward);
    }
}
