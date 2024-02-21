using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FogDamage : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            print("enter");
        }
    }
    void OnTriggerExit(Collider other)
    {
        if(other.tag == "Player")
        {
            print("exit");
        }
    }
}
