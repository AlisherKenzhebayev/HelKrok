using System.Collections.Generic;
using UnityEngine;

public class DamageTaker : MonoBehaviour
{
    [SerializeField]
    [Tooltip("Same as maxHealth, works with that assumption")]
    internal int currentHealth = 100;
    internal int maxHealth;

    private void Start()
    {
        maxHealth = currentHealth;
    }

    internal virtual void OnEnable()
    {
        EventManager.StartListening("takeDamage" + this.gameObject.GetInstanceID(), OnTakeDamage);
    }

    internal virtual void OnDisable()
    {
        EventManager.StopListening("takeDamage" + this.gameObject.GetInstanceID(), OnTakeDamage);
    }

    internal virtual void OnTakeDamage(Dictionary<string, object> obj)
    {
        FindObjectOfType<AudioManager>().Play("Hit");

        DoTakeDamage(obj);
    }

    internal virtual void DoTakeDamage(Dictionary<string, object> obj)
    {
        if (this.currentHealth <= 0f)
        {
            return;
        }

        Debug.Log("took damage");
        this.currentHealth -= (int)obj["amount"];

        if (this.currentHealth <= 0f)
        {
            DoDeath();
        }

    }

    internal virtual void DoDeath()
    {
        return;
    }

    public float FracHealth
    {
        get
        {
            return currentHealth * 1.0f / maxHealth;
        }
    }
}