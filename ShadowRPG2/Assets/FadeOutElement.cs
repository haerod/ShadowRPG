using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeOutElement : MonoBehaviour
{
    MeshRenderer[] rend = new MeshRenderer[0];
    Material[] mat = new Material[0];
    Color[] col = new Color[0];
    float alpha = 1f;
    bool isOver;
    float speed = 0.1f;
        
    void Awake ()
    {
        Transform objParent = transform.parent;

        rend = new MeshRenderer[objParent.childCount];
        mat = new Material[rend.Length];
        col = new Color[rend.Length];

        for (int i = 0; i < rend.Length; i++)
        {
            rend[i] = objParent.GetChild(i).GetComponent<MeshRenderer>();
            mat[i] = rend[i].material;
            col[i] = mat[i].color;
        }
    }
        
    void Update ()
    {
		if(isOver)
            alpha = Mathf.Lerp(alpha, 0.05f, speed);
        else
            alpha = Mathf.Lerp(alpha, 1, speed);

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

    void OnMouseOver()
    {
        isOver = true;
    }

    void OnMouseExit()
    {
        isOver = false;
    }
}
