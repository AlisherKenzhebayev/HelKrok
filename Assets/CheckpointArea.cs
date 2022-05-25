using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class CheckpointArea : MonoBehaviour
{
    [SerializeField]
    private Transform checkpointTransform;

    [SerializeField]
    private Vector3 boxSize;

    private BoxCollider boxCollider;

    private void Start()
    {
        boxCollider = GetComponent<BoxCollider>();
        if (boxCollider == null)
        {
            Debug.LogError("Error - no BoxCollider component exists");
        }
        boxCollider.size = boxSize;
    }

    private void OnTriggerEnter(Collider other)
    {
        GameplayManager.ChangeSpawnPoint(checkpointTransform);
    }

    private void OnDrawGizmos()
    {    
        Gizmos.color = Color.red;
        Transform t = transform;
        //t.position = t.localPosition;
        Gizmos.matrix = transform.localToWorldMatrix;
        Gizmos.DrawWireCube(this.transform.localPosition, boxSize);
     
        Gizmos.color = Color.green;
        Gizmos.matrix = Matrix4x4.identity;
        Gizmos.DrawSphere(checkpointTransform.position, 0.5f);
    }
}
