using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class PlayerHide : MonoBehaviour
{
    public GameObject hideText, stopHideText;
    public GameObject normalPlayer, hidingPlayer;
    public GeneralEnemyAi monsterScript;
    public Transform monsterTransform;
    public float loseDist;
    bool isInteractable, isHiding;
    void OnTriggerStay(Collider other) 
    {
        if(other.CompareTag("MainCamera"))
        {
            hideText.SetActive(true);
            isInteractable = true;  
        }
    }
    void OnTriggerExit(Collider other) 
    {
        if(other.CompareTag("MainCamera"))
        {
            hideText.SetActive(false);
            isInteractable = false;
        }
    }
    void Start()
    {
        isInteractable = false;
        isHiding = false;
    }

    void Update()
    {
        if(isInteractable)
        {
            if(Input.GetKeyDown(KeyCode.E))
            {
                hideText.SetActive(false);
                hidingPlayer.SetActive(true);
                float dist = Vector3.Distance(monsterTransform.position, normalPlayer.transform.position);
                if (dist > loseDist)
                {
                    if (monsterScript.isChasing)
                    {
                        monsterScript.stopChase();
                    }
                }
            
                stopHideText.SetActive(true);
                isHiding = true;
                normalPlayer.SetActive(false);
                isInteractable = false;
            }
        }
        else if(isHiding)
        {
            if(Input.GetKeyDown(KeyCode.E))
            {
                stopHideText.SetActive(false);
                normalPlayer.SetActive(true);
                hidingPlayer.SetActive(false);
                isHiding = false;
            }
        }
    }
}
