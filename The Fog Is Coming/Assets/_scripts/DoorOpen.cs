using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Doors : Interaction
{
    private bool isOpen = false;
    private bool canBeInteractedWith = false;
    public override void OnFocus()
    {

    }

    public override void OnInteract()
    {
        throw new System.NotImplementedException();
    }

    public override void OnLoseFocus()
    {

    }
}
