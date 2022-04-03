using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrappleScript : MonoBehaviour
{
	public Rigidbody rb;
	[SerializeField]
	private bool tethered = false;
	private float tetherLength;
	private Vector3 tetherPoint;

	public bool IsTethered() {
		return tethered;
	}

	void OnDrawGizmos()
	{
		Gizmos.color = Color.red;
		Gizmos.DrawRay(transform.position, transform.forward);
	}

	void Update()
	{
		if (Input.GetKey(KeyCode.Mouse0))
		{
			if (!tethered)
			{
				BeginGrapple();
			}
			else
			{
				EndGrapple();
			}
		}

		if (tethered) ApplyGrapplePhysics();
	}

	void BeginGrapple()
	{
		if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, Mathf.Infinity))
		{
			tethered = true;
			tetherPoint = hit.point;
			tetherLength = Vector3.Distance(tetherPoint, transform.position);
		}
	}

	void EndGrapple()
	{
		tethered = false;
	}

	void ApplyGrapplePhysics()
	{
		Vector3 directionToGrapple = Vector3.Normalize(tetherPoint - transform.position);
		float currentDistanceToGrapple = Vector3.Distance(tetherPoint, transform.position);

		float speedTowardsGrapplePoint = Mathf.Round(Vector3.Dot(rb.velocity, directionToGrapple) * 100) / 100;

		if (speedTowardsGrapplePoint < 0)
		{
			if (currentDistanceToGrapple > tetherLength)
			{
				rb.velocity -= speedTowardsGrapplePoint * directionToGrapple;
				rb.position = tetherPoint - directionToGrapple * tetherLength;
			}
		}
	}
}