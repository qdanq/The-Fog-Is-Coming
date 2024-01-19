using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class GeneralEnemyAi : MonoBehaviour
{
    public NavMeshAgent ai;
    public List<Transform> destinations;
    public float walkSpeed, chaseSpeed, minIdleTime, maxIdletime, idleTime;
    public float raycastDist, catchDist, minChaseTime, maxChaseTime;
    private float chaseTime;
    public bool isWalking, isChasing;
    public Transform player;
    Transform currentDest;
    Vector3 dest;
    int randNum;
    public Vector3 rayCastOffset;



    void Start()
    {
        isWalking = true;
        randNum = Random.Range(0, destinations.Count);
        currentDest = destinations[randNum];
    }

    void Update()
    {
        Vector3 direction = (player.position - transform.position).normalized;
        RaycastHit hit;
        if (Physics.Raycast(transform.position + rayCastOffset, direction, out hit, raycastDist))
        {
            if (hit.collider.gameObject.tag == "Player")
            {
                isWalking = false;
                StopCoroutine("stayIdle");
                StopCoroutine("chasePlayer");
                StartCoroutine("chasePlayer");
                isChasing = true;
            }
        }

        if (isChasing == true)
        {
            dest = player.position;
            ai.destination = dest;
            ai.speed = chaseSpeed;
            if (ai.remainingDistance <= catchDist)
            {
                player.gameObject.SetActive(false);
                isChasing = false;
            }
        }

        if (isWalking == true)
        {
            dest = currentDest.position;
            ai.destination = dest;            
            ai.speed = walkSpeed;
            if (ai.remainingDistance <= ai.stoppingDistance)
            {
                StopCoroutine("stayIdle");
                StartCoroutine("stayIdle");
                isWalking = false;
            }

        }   
    }
    IEnumerator stayIdle()
    {
        idleTime = Random.Range(minIdleTime, maxIdletime);
        yield return new WaitForSeconds(idleTime);
        randNum = Random.Range(0, destinations.Count);
        currentDest = destinations[randNum];
        isWalking = true;

    }

    IEnumerator chasePlayer()
    {   
        chaseTime = Random.Range(minChaseTime, maxChaseTime);
        yield return new WaitForSeconds(chaseTime);
        isWalking = true;
        isChasing = false;
        randNum = Random.Range(0, destinations.Count);
        currentDest = destinations[randNum];
    }
}
