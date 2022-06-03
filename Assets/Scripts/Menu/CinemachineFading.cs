using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CinemachineFading : MonoBehaviour
{
    CinemachineVirtualCamera vcam;
    public float fovMultiplier = 1f;
    public float speed = 1f;
    // Start is called before the first frame update
    void Start()
    {
        vcam = GetComponent<CinemachineVirtualCamera>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            StartCoroutine(FadeInFOV());
        }
    }

    public IEnumerator FadeInFOV()
    {
        while (true)
        {
            vcam.m_Lens.FieldOfView += Time.deltaTime * fovMultiplier;
            transform.position = new Vector3(transform.position.x, transform.position.y + speed * Time.deltaTime, transform.position.z);
            yield return null;
        }
    }
}
