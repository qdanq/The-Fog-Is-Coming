using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FogDamage : MonoBehaviour
{
    bool isEntered = false;
    public int healthLossPerSecond = 1;
    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            isEntered = true;
            StartCoroutine(DealDamage());
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if(other.tag == "Player")
        {
            isEntered = false;
            StopCoroutine(DealDamage());
        }
    }
    private IEnumerator DealDamage()
    {
        while(isEntered)
        {
            FirstPersonController.OnTakeDamage(healthLossPerSecond);
            yield return new WaitForSeconds(1);
        }
    }
}
