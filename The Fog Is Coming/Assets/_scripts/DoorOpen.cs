using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Doors : Interaction
{
    private bool isOpen = false;
    private bool canBeInteractedWith = false;
    private Animator anim;

    private void Start()
    {
        anim = GetComponent<Animator>();
    }
    public override void OnFocus()
    {

    }

    public override void OnInteract()
    {
        if (canBeInteractedWith)
        {
            isOpen = !isOpen;

            Vector3 doorTransformDi = transform.TransformDirection(Vector3.forward);

            Vector3 playerTransformDir;
        }
    }

    public override void OnLoseFocus()
    {

    }
}
