using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class GeneralEnemyAi : MonoBehaviour
{
    public NavMeshAgent ai;
    public List<Transform> destinations;
    public float walkSpeed, chaseSpeed, minIdleTime, maxIdletime, idleTime, raycastDist;
    public bool walking, chasing;
    public Transform player;
    Transform currentDest;
    Vector3 dest;
    int randNum1, randNum2;
    public int destAmount;
    public Vector3 rayCastOffset;



    void Start()
    {
        walking = true;
        randNum1 = Random.Range(0, destAmount);
        currentDest = destinations[randNum1];
    }

    void Update()
    {
        Vector3 direction = player.position - transform.position.normalized;
        RaycastHit hit;
        if (Physics.Raycast(transform.position + rayCastOffset, direction, out hit, raycastDist))
        {
            if (hit.collider.gameObject.tag == "Player")
            {
                
            }
        }


        if (walking == true)
        {
            dest = currentDest.position;
            ai.destination = dest;            
            ai.speed = walkSpeed;
            if (ai.remainingDistance <= ai.stoppingDistance)
            {
                randNum2 = Random.Range(0, 2);
                if (randNum2 == 0)
                {
                    randNum1 = Random.Range(0, destAmount);
                    currentDest = destinations[randNum1];
                }
                else 
                {
                    StopCoroutine("stayIdle");
                    StartCoroutine("stayIdle");
                    walking = false;
                }
            }

        }   
    }
    IEnumerator stayIdle()
    {
        idleTime = Random.Range(minIdleTime, maxIdletime);
        yield return new WaitForSeconds(idleTime);
        randNum1 = Random.Range(0, destAmount);
        currentDest = destinations[randNum1];
        walking = true;

    }
}
