using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle : MonoBehaviour
{
    public bool movable;
    public Transform startPos;
    public Transform endPos;
    public float timeToDestination = 5f;
    public Material staticMat;
    public Material movableMat;
    MeshRenderer mRenderer;

    void Start()
    {
        mRenderer = GetComponent<MeshRenderer>();
        StartCoroutine(PingPongMovement());
    }
    
    public IEnumerator PingPongMovement()
    {
        while (true)
        {
            if (movable)
            {
                mRenderer.material = movableMat;
                yield return StartCoroutine(MoveOverSeconds(gameObject, endPos.position, timeToDestination));
                yield return StartCoroutine(MoveOverSeconds(gameObject, startPos.position, timeToDestination));
            }
            else
            {
                mRenderer.material = staticMat;
                yield return null;
            }
        }
    }


    public IEnumerator MoveOverSeconds(GameObject objectToMove, Vector3 end, float seconds)
    {
        float elapsedTime = 0;
        Vector3 startingPos = objectToMove.transform.position;
        while (elapsedTime < seconds)
        {
            transform.position = Vector3.Lerp(startingPos, end, (elapsedTime / seconds));
            elapsedTime += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        transform.position = end;
    }
}
