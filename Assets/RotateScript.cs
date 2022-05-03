using UnityEngine;

public class RotateScript : MonoBehaviour
{
    [SerializeField]
    [Range(0, 1)]
    private float rotationSpeed;
    [SerializeField]
    private Vector3 rotationVector;

    private void Update()
    {
        this.transform.rotation *= Quaternion.Euler(rotationVector * rotationSpeed * Time.deltaTime);
    }
}
