using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Doors : Interaction
{
    private bool isOpen = false;
    private bool canBeInteractedWith = true;
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

            Vector3 doorTransformDir = transform.TransformDirection(Vector3.forward);

            Vector3 playerTransformDir = FirstPersonController.instance.transform.position - transform.position;
            float dot = Vector3.Dot(doorTransformDir, playerTransformDir);

            anim.SetFloat("dot", dot);
            anim.SetBool("isOpen", isOpen);
        }
    }

    public override void OnLoseFocus()
    {

    }
}
