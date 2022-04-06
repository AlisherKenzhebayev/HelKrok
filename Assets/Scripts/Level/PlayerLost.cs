using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerLost : MonoBehaviour
{
    public GameObject debugText;
    Vector3 spawnPos;
    Quaternion spawnRot;
    // Start is called before the first frame update
    void Start()
    {
        spawnPos = transform.position;
        spawnRot = transform.rotation;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            GetComponent<Rigidbody>().velocity = Vector3.zero;
            transform.position = spawnPos;
            transform.rotation = spawnRot;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            debugText.SetActive(true);
        }
    }
}
