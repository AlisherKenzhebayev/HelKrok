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
    [SerializeField]
    private float timeStartActive = 0.1f;
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

        if (Time.time - timeStart > timeStartActive)
        {
            this.gameObject.GetComponent<Collider>().enabled = true;
        }
    }

    void Start() {

        this.gameObject.SetActive(true);
        this.gameObject.GetComponent<Collider>().enabled = false;

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

    internal override bool OnCollisionEnter(Collision collision)
    {
        bool retVal = base.OnCollisionEnter(collision);

        if (retVal)
        {
            Destroy(this.gameObject);
        }

        return retVal;
    }

    internal override bool DoDealDamage(Collider other)
    {
        bool retVal = base.DoDealDamage(other);

        if (retVal)
        {
            AudioManager.Play("MagicHit");
            Destroy(this.gameObject);
        }

        return retVal;
    }
}