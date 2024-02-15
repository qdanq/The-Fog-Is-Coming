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
    public float aiDist;
    FirstPersonController playerScript;    

    void Start()
    {
        isWalking = true;
        randNum = Random.Range(0, destinations.Count);
        currentDest = destinations[randNum];
    }

    void Update()
    {
        if (FirstPersonController.isCrouching)
        {
            raycastDist /= 2;
        }
        if (FirstPersonController.isSprinting)
        {
            raycastDist *= 2;
        }
        Vector3 direction = (player.position - transform.position).normalized;
        RaycastHit hit;
        aiDist = Vector3.Distance(player.position, this.transform.position);
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
            if (aiDist <= catchDist)
            {
                FirstPersonController.OnTakeDamage(100); // Make health(sanity) loose a bit while enemy chasing
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

        if (FirstPersonController.isCrouching)
        {
            raycastDist *= 2;
        }
        if (FirstPersonController.isSprinting)
        {
            raycastDist /= 2;
        }
    }

    public void stopChase()
    {
        isWalking = true;
        isChasing = false;
        StopCoroutine("chasePlayer");
        randNum = Random.Range(0, destinations.Count);
        currentDest = destinations[randNum];
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
        FirstPersonController.OnTakeDamage(15); // Make health(sanity) loose a bit while enemy chasing
        isWalking = true;
        isChasing = false;
        randNum = Random.Range(0, destinations.Count);
        currentDest = destinations[randNum];
    }
}
