using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHide : MonoBehaviour
{
    public GameObject hideText, stopHideText;
    public GameObject normalPlayer, hidingPlayer;
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
        if(isInteractable == true)
        {
            if(Input.GetKeyDown(KeyCode.E))
            {
                hideText.SetActive(false);
                hidingPlayer.SetActive(true);
            
                stopHideText.SetActive(true);
                isHiding = true;
                normalPlayer.SetActive(false);
                isInteractable = false;
            }
        }
        if(isHiding)
        {
            if(Input.GetKeyDown(KeyCode.Q))
            {
                stopHideText.SetActive(false);
                normalPlayer.SetActive(true);
                hidingPlayer.SetActive(false);
                isHiding = false;
            }
        }
    }
}
