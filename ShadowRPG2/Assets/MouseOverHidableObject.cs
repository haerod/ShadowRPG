using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseOverHidableObject : MonoBehaviour
{
    [HideInInspector]
    public bool isOver;

    public void OnMouseOver()
    {
        if(!Camera.main.GetComponent<CombatCamera>().isActionMode)
            isOver = true;
        else
            isOver = false;
    }

    public void OnMouseExit()
    {
        isOver = false;
    }

}
