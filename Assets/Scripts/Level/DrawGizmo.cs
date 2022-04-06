using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawGizmo : MonoBehaviour
{
    public Color color = Color.yellow;
    public float radius = 1.5f;
    // Start is called before the first frame update

    // Update is called once per frame
    private void OnDrawGizmos()
    {
        // Set the color of Gizmos to green
        Gizmos.color = color;

        Gizmos.DrawSphere(transform.position, radius);
    }
}
