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
        Gizmos.DrawWireCube(this.transform.position, Vector3.Scale(boxSize, this.transform.localScale));
     
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(checkpointTransform.position, 0.5f);
    }
}
