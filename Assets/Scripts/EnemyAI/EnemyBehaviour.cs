using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBehaviour : MonoBehaviour
{
    public float speed = 1f;
    public float followSpeedMultiplayer = .3f;
    public float rotSpeed = 1f;
    bool moving = true;
    public GameObject patrolPointsParent;
    List<GameObject> patrolPoints = new List<GameObject>();
    [SerializeField]
    EnemyStates enemyState = EnemyStates.patrol;

    [SerializeField]
    Transform enemyMesh;

    [SerializeField]
    float alertDistance;

    [SerializeField]
    float followDistance = 20;

    GameObject player;

    [SerializeField]
    ProjectileSpawner projectileSpawner;

    int currentTargetIndex;
    Transform currentTargetTransform;
    // Start is called before the first frame update

    public enum EnemyStates
    {
        patrol = 0,
        attacking = 1,
        dead = 2
    }

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        foreach (Transform point in patrolPointsParent.transform)
        {
            patrolPoints.Add(point.gameObject);
        }
        currentTargetTransform = patrolPoints[0].transform;
        StartCoroutine(EnemyBehaviourCor());
    }

    // Update is called once per frame
    void Update()
    {
        if (isPlayerClose())
        {
            ChangeEnemyState(EnemyStates.attacking);
        }
        else
        {
            ChangeEnemyState(EnemyStates.patrol);
        }
    }


    IEnumerator EnemyBehaviourCor()
    {
        while (moving)
        {

            while (enemyState == EnemyStates.patrol)
            {
                projectileSpawner.SwitchSpawning(false);
                yield return StartCoroutine(Patrol());
                AssignNextTarget();
            }
            while (enemyState == EnemyStates.attacking)
            {
                projectileSpawner.SwitchSpawning(true);
                Vector3 D = player.transform.position - enemyMesh.transform.position;
                enemyMesh.transform.rotation = Quaternion.Slerp(enemyMesh.transform.rotation, Quaternion.LookRotation(D), rotSpeed * Time.deltaTime);
                FollowTarget(player.transform);
                yield return null;
            }
            while (enemyState == EnemyStates.dead)
            {
                yield return null;
            }
            yield return null;
        }
        yield return null;
    }


    IEnumerator Patrol()
    {
        float distance = 5000;
        while (distance > 1f && enemyState == EnemyStates.patrol)
        {
            MoveToTarget(currentTargetTransform);
            yield return null;
            distance = Vector3.Distance(enemyMesh.transform.position, currentTargetTransform.position);
        }
    }

    void MoveToTarget(Transform target, float speedMultiplayer = 1)
    {
        //enemyMesh.transform.LookAt(target);
        Vector3 D = target.position - enemyMesh.transform.position;
        enemyMesh.transform.rotation = Quaternion.Slerp(enemyMesh.transform.rotation, Quaternion.LookRotation(D), rotSpeed * Time.deltaTime);
        enemyMesh.transform.position = Vector3.Lerp(enemyMesh.transform.position, target.position, speedMultiplayer * speed * Time.deltaTime);
        Debug.Log("Move To target");
    }

    void AssignNextTarget()
    {
        if (patrolPoints.Count - 1 > currentTargetIndex)
        {
            currentTargetIndex++;
            currentTargetTransform = patrolPoints[currentTargetIndex].transform;
        }
        else
        {
            currentTargetIndex = 0;
            currentTargetTransform = patrolPoints[0].transform;
        }
    }

    public IEnumerator MoveToTargetOverSeconds(GameObject objectToMove, Vector3 end, float seconds)
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


    public void ChangeEnemyState(EnemyStates state)
    {
        enemyState = state;
    }


    bool isPlayerClose()
    {
        float distance = Vector3.Distance(enemyMesh.transform.position, player.transform.position);
        if (distance < alertDistance)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    void FollowTarget(Transform target)
    {
        float distance = Vector3.Distance(target.position, enemyMesh.position);
        if (distance > followDistance)
        {
            MoveToTarget(target, followSpeedMultiplayer);
        }
    }
}
