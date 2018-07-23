using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeOutElement : MonoBehaviour
{
    //public bool isOver;
    //public bool isTest;

    MouseOverHidableObject[] childScriptList = new MouseOverHidableObject[0];
    MeshRenderer[] rend = new MeshRenderer[0];
    Material[] mat = new Material[0];
    Color[] col = new Color[0];
    float alpha = 1f;
    float speed = 0.1f;
        
    void Awake ()
    {
        childScriptList = new MouseOverHidableObject[transform.childCount];
        rend = new MeshRenderer[transform.childCount];
        mat = new Material[rend.Length];
        col = new Color[rend.Length];

        for (int i = 0; i < rend.Length; i++)
        {
            rend[i] = transform.GetChild(i).GetComponent<MeshRenderer>();
            mat[i] = rend[i].material;
            col[i] = mat[i].color;
            childScriptList[i] = transform.GetChild(i).GetComponent<MouseOverHidableObject>();
        }
    }
        
    void Update ()
    {
		if(IsMouseOverAChild())
        {
            alpha = Mathf.Lerp(alpha, 0.05f, speed);
        }
        else
        {
            alpha = Mathf.Lerp(alpha, 1, speed);
        }

        if (alpha > .99f)
            alpha = 1f;
        else if (alpha < .01f)
            alpha = 0f;

        for (int i = 0; i < rend.Length; i++)
        {
            Color newCol = col[i];
            newCol.a = alpha;
            rend[i].material.color = newCol;
        }
    }

    bool IsMouseOverAChild()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            if(childScriptList[i].isOver)
            {
                return (true);
            }
        }
        return false;
    }
}
