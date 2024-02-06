using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Interaction : MonoBehaviour
{
    public abstract void OnInteract();
    public abstract void OnFocus();
    public abstract void OnLoseFocus();
    void Start()
    {
        
    }

    void Update()
    {
        
    }
}
