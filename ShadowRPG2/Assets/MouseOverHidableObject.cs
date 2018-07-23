using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseOverHidableObject : MonoBehaviour
{
    [HideInInspector]
    public bool isOver;

    public void OnMouseOver()
    {
        isOver = true;
    }

    public void OnMouseExit()
    {
        isOver = false;
    }

}
