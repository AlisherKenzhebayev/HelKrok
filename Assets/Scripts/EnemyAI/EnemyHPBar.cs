using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHPBar : MonoBehaviour
{
    [SerializeField]
    private EnemyDT enemyScript;
    private float health;
    private float maxHealth;
    private Transform player;
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        health = enemyScript.currentHealth;
        maxHealth = enemyScript.currentHealth;
    }

    // Update is called once per frame
    void Update()
    {
        health = enemyScript.currentHealth;
        transform.localScale = new Vector3(health / maxHealth, 1, 1);
        transform.LookAt(player);
    }

}
