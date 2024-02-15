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

            StartCoroutine(AutoClose());
        }
    }

    public override void OnLoseFocus()
    {
        
    }

    private void Animator_LockInteraction()
    {
        canBeInteractedWith = false;
    }
    private void Animator_UnlockInteraction()
    {
        canBeInteractedWith = true;
    }
    private IEnumerator AutoClose()
    {
        while (isOpen)
        {
            yield return new WaitForSeconds(8);

            if (Vector3.Distance(transform.position, FirstPersonController.instance.transform.position) > 5)
            {
                isOpen = false; 
                anim.SetFloat("dot", 0);
                anim.SetBool("isOpen", isOpen);
            } 
        }
    }

}
