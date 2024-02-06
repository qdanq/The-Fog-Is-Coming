using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestInteraction : Interaction
{
    public override void OnFocus()
    {
        print("look at " + gameObject.name);
    }

    public override void OnInteract()
    {
        print("Interacted with " + gameObject.name);
    }

    public override void OnLoseFocus()
    {
        print("stopped looking at " + gameObject.name);
    }
}
