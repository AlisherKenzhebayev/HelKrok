using System.Collections.Generic;
using UnityEngine;

public class BulletDD : DamageDealer
{
    private Rigidbody rb;

    [SerializeField]
    [Tooltip("Force of impulse on projectile")]
    private float impulseForce = 40f;
    [SerializeField]
    private Transform placement = null;

    [SerializeField]
    private float timeExist = 5f;
    private float timeStart;

    public BulletDD()
    {
    }

    public BulletDD(Transform placement)
    {
        this.placement = placement;
    }

    public BulletDD(float impulse, Transform placement, float timeExist = 5f)
    {
        this.impulseForce = impulse;
        this.placement = placement;
        this.timeExist = timeExist;
    }

    void Awake()
    {
        timeStart = Time.time;
    }

    private void Update()
    {
        if (Time.time - timeStart > timeExist)
        {
            Destroy(this.gameObject);
        }
    }

    void Start() {

        this.gameObject.SetActive(true);

        this.rb = this.GetComponent<Rigidbody>();
        if (this.rb == null)
        {
            Debug.LogError("Error - no RigidBody component");
        }

        if (placement == null)
        {
            //this.transform.position = gameObject.transform.parent.position;
            //this.transform.rotation = gameObject.transform.parent.rotation;
        }
        else
        {
            this.transform.position = placement.transform.position;
            this.transform.rotation = placement.transform.rotation;
        }

        //Debug.Log(rb.transform.forward);

        rb.AddForce(rb.transform.forward * impulseForce, ForceMode.Impulse);
    }

    internal override void DoDealDamage(Collider other)
    {
        base.DoDealDamage(other);

        AudioManager.Play("MagicHit");
        Destroy(this.gameObject);
    }
}